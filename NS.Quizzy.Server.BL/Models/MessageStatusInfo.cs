namespace NS.Quizzy.Server.BL.Models
{
    internal class MessageStatusInfo
    {
        public bool IsCompleted { get; set; }
        public bool IsSuccess { get; set; }
        public double? ProgressPercentage { get; set; }
        public int DownloadCounter { get; set; }
        public string? Error { get; set; }
        public List<string> ContextIds { get; set; }

        public MessageStatusInfo()
        {
            ContextIds = [];
        }

        public MessageStatusInfo AddContextId(string? contextId)
        {
            if (!string.IsNullOrWhiteSpace(contextId) && !ContextIds.Contains(contextId))
            {
                ContextIds.Add(contextId);
            }
            return this;
        }

    }
}
