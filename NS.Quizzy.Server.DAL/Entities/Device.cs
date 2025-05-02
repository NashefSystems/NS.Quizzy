namespace NS.Quizzy.Server.DAL.Entities
{
    public class Device
    {
        public string Key { get; set; }
        public string SerialNumber { get; set; }
        public string UniqueId { get; set; }
        public string AppVersionName { get; set; }
        public string OS { get; set; }
        public string OSVersion { get; set; }
        public bool IsTV { get; set; }
        public bool IsTesting { get; set; }
        public bool IsIOS { get; set; }
        public bool IsAndroid { get; set; }
        public bool IsWindows { get; set; }
        public bool IsMacOS { get; set; }
        public bool IsWeb { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastHeartBeat { get; set; }
    }
}
