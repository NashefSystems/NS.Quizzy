using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.DAL.Entities
{
    public class ExamType : BaseEntity
    {
        public string Name { get; set; }
        public int ItemOrder { get; set; }
    }
}
