using NS.Quizzy.Server.Common.Attributes;

namespace NS.Quizzy.Server.Common
{
    public class Enums
    {
        public enum AppSettingKeys
        {
            [DBStringValue(nameof(SavePasswordOnRememberMe))]
            SavePasswordOnRememberMe,

            [DBStringValue(nameof(CacheDataTTLMin))]
            CacheDataTTLMin,

            [DBStringValue(nameof(CacheLoginsTTLMin))]
            CacheLoginsTTLMin,

            [DBStringValue(nameof(CacheOTPTTLMin))]
            CacheOTPTTLMin,

            [DBStringValue(nameof(ServerInfoTTLMin))]
            ServerInfoTTLMin,

            [DBStringValue(nameof(Email))]
            Email,

            [DBStringValue(nameof(IdNumberEmailDomain))]
            IdNumberEmailDomain,

            [DBStringValue(nameof(IgnoreOTPValidationUserIds))]
            IgnoreOTPValidationUserIds,

            [DBStringValue(nameof(NotificationsGetLimitValue))]
            NotificationsGetLimitValue,

            [DBStringValue(nameof(GoogleCredentialJson))]
            GoogleCredentialJson,
        }
    }
}
