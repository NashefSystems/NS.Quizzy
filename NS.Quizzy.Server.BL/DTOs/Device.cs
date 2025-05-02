using Newtonsoft.Json;

namespace NS.Quizzy.Server.BL.DTOs
{
    public class DevicePayloadDto
    {
        public string SerialNumber { get; set; }
        public string UniqueId { get; set; }
        public string AppVersionName { get; set; }
        [JsonProperty("os")]
        public string OS{ get; set; }
        [JsonProperty("osVersion")]
        public string OSVersion { get; set; }
        public bool IsTV { get; set; }
        public bool IsTesting { get; set; }
        public bool IsIOS { get; set; }
        public bool IsAndroid { get; set; }
        public bool IsWindows { get; set; }
        public bool IsMacOS { get; set; }
        public bool IsWeb { get; set; }
    }

    public class DeviceDto : DevicePayloadDto
    {
        public string Key { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastHeartBeat { get; set; }
    }
}
