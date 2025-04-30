using Newtonsoft.Json;
using NS.Quizzy.Server.DAL.Entities;
using static NS.Quizzy.Server.DAL.DALEnums;

namespace NS.Quizzy.Server.BL.DTOs
{
    public class NotificationBasePayloadDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string>? Data { get; set; }
    }

    public class NotificationPayloadDto : NotificationBasePayloadDto
    {
        public NotificationTarget Target { get; set; }
        public List<Guid>? TargetIds { get; set; }
    }

    public class NotificationDto : NotificationPayloadDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }

        public DateTimeOffset CreatedTime { get; set; }
        public int TotalUsers { get; set; }
        public int TotalRead { get; set; }
        public double ReadPercentage { get; set; }
        public int NumberOfPushNotificationsReceived { get; set; }
        public double PushNotificationReceivedPercentage { get; set; }
    }

    public class MyNotificationItem : NotificationBasePayloadDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }

        public DateTimeOffset CreatedTime { get; set; }
        public string Author { get; set; }
        public bool Read { get; set; }
    }
}
