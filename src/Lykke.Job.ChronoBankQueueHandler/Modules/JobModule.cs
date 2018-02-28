using System;
using Autofac;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates.Index;
using Common.Log;
using Lykke.SettingsReader;
using Lykke.Service.ExchangeOperations.Client;
using Lykke.Job.ChronoBankQueueHandler.AzureRepositories.BitCoin;
using Lykke.Job.ChronoBankQueueHandler.AzureRepositories.PaymentSystems;
using Lykke.Job.ChronoBankQueueHandler.Core.Domain.BitCoin;
using Lykke.Job.ChronoBankQueueHandler.Core.Domain.PaymentSystems;
using Lykke.Job.ChronoBankQueueHandler.Core.Services;
using Lykke.Job.ChronoBankQueueHandler.Services;

namespace Lykke.Job.ChronoBankQueueHandler.Modules
{
    public class JobModule : Module
    {
        private readonly AppSettings _settings;
        private readonly IReloadingManager<AppSettings.DbSettings> _dbSettingsManager;
        private readonly ILog _log;
        
        public JobModule(
            AppSettings settings,
            IReloadingManager<AppSettings> settingsManager,
            ILog log)
        {
            _settings = settings;
            _dbSettingsManager = settingsManager.Nested(s => s.ChronoBankQueueHandlerJob.Db);
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings)
                .SingleInstance();

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterInstance<IHealthService>(new HealthService(
                allowIdling: true,
                maxHealthyMessageProcessingDuration: _settings.ChronoBankQueueHandlerJob.Health.MaxMessageProcessingDuration,
                maxHealthyMessageProcessingFailedInARow: _settings.ChronoBankQueueHandlerJob.Health.MaxMessageProcessingFailedInARow,
                maxHealthyMessageProcessingIdleDuration: TimeSpan.Zero));

            RegisterAzureRepositories(builder, _dbSettingsManager);
            RegisterServices(builder, _settings);
        }

        private static void RegisterServices(ContainerBuilder builder, AppSettings appSettings)
        {
            var exchangeOperationsService = new ExchangeOperationsServiceClient(appSettings.ExchangeOperationsServiceClient.ServiceUrl);
            builder.RegisterInstance(exchangeOperationsService).As<IExchangeOperationsServiceClient>().SingleInstance();
        }

        private void RegisterAzureRepositories(ContainerBuilder builder, IReloadingManager<AppSettings.DbSettings> settings)
        {
            builder.RegisterInstance<IBitCoinTransactionsRepository>(
                new BitCoinTransactionsRepository(
                    AzureTableStorage<BitCoinTransactionEntity>.Create(settings.ConnectionString(s => s.BitCoinQueueConnectionString), "BitCoinTransactions", _log)));

            builder.RegisterInstance<IWalletCredentialsRepository>(
                new WalletCredentialsRepository(
                    AzureTableStorage<WalletCredentialsEntity>.Create(settings .ConnectionString(s => s.ClientPersonalInfoConnString), "WalletCredentials", _log)));


            builder.RegisterInstance<IPaymentTransactionsRepository>(
                new PaymentTransactionsRepository(
                    AzureTableStorage<PaymentTransactionEntity>.Create(settings.ConnectionString(s => s .ClientPersonalInfoConnString), "PaymentTransactions", _log),
                    AzureTableStorage<AzureMultiIndex>.Create(settings.ConnectionString(s => s.ClientPersonalInfoConnString), "PaymentTransactions", _log)));

            builder.RegisterInstance<IPaymentSystemsRawLog>(
                new PaymentSystemsRawLog(
                    AzureTableStorage<PaymentSystemRawLogEventEntity>.Create(settings.ConnectionString(s => s.LogsConnString), "PaymentSystemsLog", _log)));
        }
    }
}