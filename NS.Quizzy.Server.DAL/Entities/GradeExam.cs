using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.DAL.Entities
{
    public class GradeExam : BaseEntity
    {
        public Guid ExamId { get; set; }
        public virtual Exam Exam { get; set; }

        public Guid GradeId { get; set; }
        public virtual Grade Grade { get; set; }
    }
}
