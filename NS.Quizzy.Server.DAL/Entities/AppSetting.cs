using NS.Shared.Logging.Attributes;
using NS.Shared.Logging;
using static NS.Quizzy.Server.DAL.DALEnums;

namespace NS.Quizzy.Server.DAL.Entities
{
    public class AppSetting : BaseEntity
    {
        public string Key { get; set; }
        public AppSettingTargets Target { get; set; }
        public AppSettingValueTypes ValueType { get; set; }
       
        [SensitiveData(SensitiveDataMode.Sha1)]
        public string? Value { get; set; }
        public bool IsSecured { get; set; }
    }
}
