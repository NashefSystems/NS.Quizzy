using NS.Quizzy.Server.BL.Models;

namespace NS.Quizzy.Server.BL.Interfaces
{
    internal interface IOutlookCalendarService
    {
        Task<EventInCalendar?> CreateOrUpdateEventInCalendar(EventInCalendar eventItem);
        Task DeleteEventInCalendar(string eventId, string? calendarName = null);
    }
}
