using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface ITestService
    {
        Task<object> TestAsync();
    }
}
