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

        public enum NotificationTargetTypes
        {
            SpecificUsers,
            Classes,
            Grades,
            Students,
            Teachers,
            TeachersAndStudents,
            Admins,
            AllUsers,
            NotificationGroups,
        }
    }
}
