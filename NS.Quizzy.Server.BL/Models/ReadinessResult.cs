using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NS.Quizzy.Server.BL.Models
{

    public class ReadinessResult
    {
        public HealthStatus Status { get; set; }
        public Dictionary<string, HealthCheckResult> Checks { get; set; }
    }
}
