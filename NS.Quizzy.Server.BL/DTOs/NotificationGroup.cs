using Newtonsoft.Json;

namespace NS.Quizzy.Server.BL.DTOs
{
    public class NotificationGroupPayloadDto
    {
        public string Name { get; set; }
        public List<Guid> UserIds { get; set; }
    }

    public class NotificationGroupDto : NotificationGroupPayloadDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }
    }
}
