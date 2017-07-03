using System.Threading.Tasks;

namespace Lykke.Job.ChronoBankQueueHandler.Core.Domain.PaymentSystems
{
    public interface IPaymentSystemsRawLog
    {
        Task RegisterEventAsync(IPaymentSystemRawLogEvent evnt);
    }
}