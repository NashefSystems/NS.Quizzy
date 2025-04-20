using Newtonsoft.Json;
using static NS.Quizzy.Server.DAL.DALEnums;

namespace NS.Quizzy.Server.BL.DTOs
{
    public class NotificationPayloadDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public NotificationTarget Target { get; set; }
        public List<Guid>? TargetIds { get; set; }
        public Dictionary<string, string>? Data { get; set; }
    }

    public class NotificationDto : NotificationPayloadDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }

        public int? TotalUsers { get; set; }
        public int? TotalSeen { get; set; }
    }
}
