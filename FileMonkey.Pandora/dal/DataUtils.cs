using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using System.Collections;

namespace FileMonkey.Pandora.dal
{
    internal class DataUtils
    {
        DataContext context { get; set; }
        
        public DataUtils(DataContext context)
        {
            this.context = context;
        }

        internal void AttachObject(Object value)
        {
            ExecuteMethodInValue("Attach", value);
        }

        internal void AddObject(Object value)
        {
            ExecuteMethodInValue("Add", value);
        }

        internal void UpdateObject(Object value)
        {
            context.Entry(value).State = System.Data.EntityState.Modified;            
        }

        internal void DeleteObject(Object value)
        {
            DoRemoveProcess(value);                
        }

        private void ExecuteMethodInValue(String sMethod, Object value)
        {
            Object efSet = GetEfSet(value);

            MethodInfo method = null;

            method = efSet.GetType().GetMethod(sMethod);
            method.Invoke(efSet, new object[] { value });
        }

        private void DeleteSubObjects(Object value)
        {
            if (value is IEnumerable)
            {
                List<Object> aux = new List<Object>();

                foreach (var elto in (IEnumerable) value)
                {
                    aux.Add(elto);
                }

                foreach (var elto in aux)
                {
                    DoRemoveProcess(elto);
                }
            }
        }

        private void AttachSubObjects(Object value)
        {            
            if (value is IEnumerable)
            {
                List<Object> aux = new List<Object>();

                foreach (var elto in (IEnumerable)value)
                {
                    aux.Add(elto);
                }

                foreach (var elto in aux)
                {
                    if (context.Entry(elto).State == System.Data.EntityState.Detached)
                    {
                        AttachObject(elto);                                                
                    }

                    AttachReferences(elto);
                }
            }            
        }

        private void DoAttachProcess(Object singleValue)
        {
            AttachReferences(singleValue);
            AttachObject(singleValue);            
        }

        private void DoRemoveProcess(Object singleValue)
        {
            AttachObject(singleValue);

            DeleteReferences(singleValue);            
            ExecuteMethodInValue("Remove", singleValue);       
        }       

        private void AttachReferences(Object value)
        {
            var virtualProps = value.GetType()
                .GetProperties(
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public)
                .Where(
                    propInfo => propInfo.Module.ScopeName.Equals("EntityProxyModule"))
                .ToList();

            virtualProps.ForEach(
                propInfo => AttachSubObjects(propInfo.GetValue(value, null)));
        }

        private void DeleteReferences(Object value)
        {
            var virtualProps = value.GetType()
                .GetProperties(
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public)
                .Where(
                    propInfo => propInfo.Module.ScopeName.Equals("EntityProxyModule"))
                .ToList();

            virtualProps.ForEach(
                propInfo => DeleteSubObjects(propInfo.GetValue(value, null)));
        }
        
        private Object GetEfSet(Object dEntry)
        {
            Type entityBaseType = dEntry.GetType();

            while(!entityBaseType.BaseType.Equals(typeof(Object)))
            {
                entityBaseType = entityBaseType.BaseType;
            }

            var membersEFSet = context.GetType()
                .GetProperties(
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                .Where(
                    propInfo => propInfo.PropertyType.Name.Contains("DbSet") && 
                    propInfo.PropertyType.FullName.Contains(entityBaseType.FullName));

            return membersEFSet.SingleOrDefault().GetValue(context, null);
        }
    }
}
