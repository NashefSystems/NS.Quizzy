using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.BL.Utils
{
    public static class AppUtils
    {
        public static Version? GetAppVersion()
        {
            return System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Version;
        }
        public static string? GetAppVersionAsString()
        {
            var appVersion = GetAppVersion();
            return appVersion == null ? null : $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}";
        }
    }
}
