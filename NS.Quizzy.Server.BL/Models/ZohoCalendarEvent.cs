namespace NS.Quizzy.Server.BL.Models
{
    internal class ZohoCalendarEvent
    {
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsAllDay { get; set; }
        public string? TimeZone { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public List<string> Emails { get; set; }
    }
}
