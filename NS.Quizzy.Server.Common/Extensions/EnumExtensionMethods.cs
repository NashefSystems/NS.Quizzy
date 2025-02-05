using NS.Quizzy.Server.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.Common.Extensions
{
    public static class EnumExtensionMethods
    {
        public static string GetDBStringValue(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DBStringValueAttribute attribute = (DBStringValueAttribute)Attribute.GetCustomAttribute(field, typeof(DBStringValueAttribute));
            return attribute?.Value ?? value.ToString();
        }
    }
}
