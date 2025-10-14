namespace NS.Quizzy.Server.BL.Models
{
    internal class MessageStatusInfo
    {
        public bool IsCompleted { get; set; }
        public bool IsSuccess { get; set; }
        public double? ProgressPercentage { get; set; }
        public int DownloadCounter { get; set; }
        public string? Error { get; set; }
    }
}
