using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.DAL.Entities
{
    public class ClassExam : BaseEntity
    {
        public Guid ExamId { get; set; }
        public virtual Exam Exam { get; set; }

        public Guid ClassId { get; set; }
        public virtual Class Class { get; set; }      
    }
}
