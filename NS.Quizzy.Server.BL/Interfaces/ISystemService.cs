namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface ISystemService
    {
        Task<object> ReQueueDlqMessagesAsync(string queueName);
    }
}
