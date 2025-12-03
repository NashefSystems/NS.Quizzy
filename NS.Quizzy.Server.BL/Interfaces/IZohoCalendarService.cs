using NS.Quizzy.Server.BL.Models;

namespace NS.Quizzy.Server.BL.Interfaces
{
    internal interface IZohoCalendarService
    {
        Task<string> CreateOrUpdateEventAsync(ZohoCalendarEvent calendarEvent, string? eventId = null);
        Task DeleteEventAsync(string eventId);
        Task<List<ZohoCalendarEvent>> GetEventsAsync(DateTime fromDate, DateTime toDate);
    }
}