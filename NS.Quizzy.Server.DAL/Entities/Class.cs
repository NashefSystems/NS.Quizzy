using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.DAL.Entities
{
    public class Class : BaseEntity
    {
        public string Name { get; set; }
        public Guid? ParentId { get; set; }

        public virtual Class Parent { get; set; }
        public virtual IList<Class> Children { get; set; }
        public virtual IList<ClassExam> ClassExams { get; set; }
    }
}
