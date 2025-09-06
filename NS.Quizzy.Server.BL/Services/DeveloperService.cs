using NS.Quizzy.Server.BL.Interfaces;

namespace NS.Quizzy.Server.BL.Services
{
    internal class DeveloperService : IDeveloperService
    {
        private readonly IFcmService _fcmService;

        public DeveloperService(IFcmService fcmService)
        {
            _fcmService = fcmService;
        }

        public async Task<object> TestAsync()
        {
            var request = new Models.PushNotificationRequest()
            {
                DeviceToken = "edMJVnD4TWaD4eSY1pMIQF:APA91bH2M61IwIEZh6AwXrftOLQkr9B4l2rijOZ6VfZYQ9hNHOeHzMs7aUvZkfFJnfqicRenbGoQiqAL-Tom06U07pSfSsmaHCK2LUeez8rITaNgg1ef2Ok",
                Body = "Test",
                Title = "Test message"
            };
            return await _fcmService.SendPushNotificationAsync(request);
        }
    }
}
