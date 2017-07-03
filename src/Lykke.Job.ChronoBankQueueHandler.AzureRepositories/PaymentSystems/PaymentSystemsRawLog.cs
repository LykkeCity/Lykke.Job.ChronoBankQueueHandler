using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.ChronoBankQueueHandler.Core.Domain.PaymentSystems;

namespace Lykke.Job.ChronoBankQueueHandler.AzureRepositories.PaymentSystems
{
    public class PaymentSystemsRawLog : IPaymentSystemsRawLog
    {
        private readonly INoSQLTableStorage<PaymentSystemRawLogEventEntity> _tableStorage;

        public PaymentSystemsRawLog(INoSQLTableStorage<PaymentSystemRawLogEventEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task RegisterEventAsync(IPaymentSystemRawLogEvent evnt)
        {
            var newEntity = PaymentSystemRawLogEventEntity.Create(evnt);
            await _tableStorage.InsertAndGenerateRowKeyAsDateTimeAsync(newEntity, evnt.DateTime);

        }
    }
}