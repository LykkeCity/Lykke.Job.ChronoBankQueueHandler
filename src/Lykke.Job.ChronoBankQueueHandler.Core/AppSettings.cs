namespace Lykke.Job.ChronoBankQueueHandler.Core
{
    public class AppSettings
    {
        public ChronoBankQueueHandlerSettings ChronoBankQueueHandlerJob { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }

        public class ChronoBankQueueHandlerSettings
        {
            public DbSettings Db { get; set; }
            public string TriggerQueueConnectionString { get; set; }
        }

        public class DbSettings
        {
            public string LogsConnString { get; set; }
        }

        public class SlackNotificationsSettings
        {
            public AzureQueueSettings AzureQueue { get; set; }

            public int ThrottlingLimitSeconds { get; set; }
        }

        public class AzureQueueSettings
        {
            public string ConnectionString { get; set; }

            public string QueueName { get; set; }
        }
    }
}