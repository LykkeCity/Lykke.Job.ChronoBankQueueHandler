using Lykke.Job.ChronoBankQueueHandler.Core.Services;

namespace Lykke.Job.ChronoBankQueueHandler.Services
{
    public class HealthService : IHealthService
    {
        public string GetHealthViolationMessage()
        {
            // TODO: Check gathered health statistics, and return appropriate health violation message, or NULL if job is ok
            return null;
        }
    }
}