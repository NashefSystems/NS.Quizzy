using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NS.Quizzy.Server.DAL.DALEnums;

namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface IAppSettingsService
    {
        Task<object> GetByKeyAsync(string key);
        Task<Dictionary<string, object?>> GetByTargetAsync(AppSettingTargets target);
    }
}
