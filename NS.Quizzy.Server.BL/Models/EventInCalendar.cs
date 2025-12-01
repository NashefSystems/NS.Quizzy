namespace NS.Quizzy.Server.BL.Models
{
    public class EventInCalendar
    {
        public string? Id { get; set; }
        public string? CalendarName { get; set; }
        public string? Subject { get; set; }
        public string? TimeZone { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAllDay { get; set; }
        public string? Body { get; set; }
        public string? Location { get; set; }
    }
}
