using NS.Quizzy.Server.DAL.Attributes;

namespace NS.Quizzy.Server.DAL.Entities
{
    public class Device
    {
        [DBColumnOrder(1)]
        public required string ID { get; set; }

        [DBColumnOrder(2)]
        public string? AppVersion { get; set; }

        [DBColumnOrder(3)]
        public string? AppBuildNumber { get; set; }

        [DBColumnOrder(4)]
        public string? OS { get; set; }

        [DBColumnOrder(5)]
        public string? OSVersion { get; set; }

        [DBColumnOrder(6)]
        public bool IsTV { get; set; }

        [DBColumnOrder(7)]
        public bool IsTesting { get; set; }

        [DBColumnOrder(8)]
        public bool IsIOS { get; set; }

        [DBColumnOrder(9)]
        public bool IsAndroid { get; set; }

        [DBColumnOrder(10)]
        public bool IsWindows { get; set; }

        [DBColumnOrder(11)]
        public bool IsMacOS { get; set; }

        [DBColumnOrder(12)]
        public bool IsWeb { get; set; }

        [DBColumnOrder(13)]
        public string? SerialNumber { get; set; }

        [DBColumnOrder(14)]
        public string? UniqueId { get; set; }

        [DBColumnOrder(15)]
        public DateTimeOffset CreatedTime { get; set; }

        [DBColumnOrder(16)]
        public DateTimeOffset LastHeartBeat { get; set; }
    }
}
