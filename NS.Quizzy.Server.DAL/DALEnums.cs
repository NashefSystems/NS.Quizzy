using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.DAL
{
    public static class DALEnums
    {
        public enum AppSettingTargets
        {
            Server,
            Client,
            All
        }

        public enum AppSettingValueTypes
        {
            Integer,
            Double,
            Float,
            Boolean,
            Char,
            String,
            Object,
            Json,
        }

        public enum Roles
        {
            Student,
            Teacher,
            Admin,
            Developer,
            SuperAdmin,
        }

        public enum NotificationTarget
        {
            User,
            Class,
            Grade,
            Role,
            All,
        }
    }
}
