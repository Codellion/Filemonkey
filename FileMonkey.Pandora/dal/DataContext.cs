using System;
using System.Data.Entity;
using FileMonkey.Pandora.dal.entities;
using System.Reflection;

namespace FileMonkey.Pandora.dal
{
    public class DataContext : DbContext
    {
        static DataContext()
        {
            Database.SetInitializer<DataContext>(new CreateDatabaseIfNotExists<DataContext>());
        }

        public void InicializeContext()
        {
            using (var db = this)
            {
                db.SaveChanges();
            }
        }

        private DataUtils dUtil;
        private DataUtils DUtil
        {
            get
            {
                if (dUtil == null)
                {
                    dUtil = new DataUtils(this);
                }

                return dUtil;
            }
        }
        
        public DbSet<Inspector> Inspectors { get; set; }
        public DbSet<RuleFile> Rules { get; set; }

        public void AttachEntity(Object entity)
        {
            DUtil.AttachObject(entity);
        }

        public void PersistEntity(Object entity)
        {
            DUtil.AttachObject(entity);
            
            Type entityBaseType = entity.GetType();

            while (!entityBaseType.BaseType.Equals(typeof(Object)))
            {
                entityBaseType = entityBaseType.BaseType;
            }

            PropertyInfo propId = entityBaseType.GetProperty(entityBaseType.Name + "Id");

            int id = (int) propId.GetValue(entity, null);

            if (id == 0)
            {
                DUtil.AddObject(entity);
            }
            else
            {
                DUtil.UpdateObject(entity);
            }
        }

        public void RemoveEntity(Object entity)
        {
            DUtil.DeleteObject(entity);
        }
    }
}
