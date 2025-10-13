using Newtonsoft.Json;

namespace NS.Quizzy.Server.BL.DTOs
{
    public class NotificationTemplatePayloadDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public class NotificationTemplateDto : NotificationTemplatePayloadDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }
    }
}
