namespace NS.Quizzy.Server.BL.Models
{
    public class PushNotificationRequest
    {
        public string DeviceToken { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public object Data { get; set; }
    }
}
