using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace FileMonkey.Pandora.dal.utils
{
    public class EntityUtils
    {
        public static void InstanceIfNull(Object value)
        {
            if (value == null)
            {
                ConstructorInfo constructor = value.GetType().GetConstructor(null);

                value = constructor.Invoke(null);
            }
        }
    }
}
