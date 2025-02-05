using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DBStringValueAttribute : Attribute
    {
        public string Value { get; }

        public DBStringValueAttribute(string value)
        {
            Value = value;
        }
    }
}
