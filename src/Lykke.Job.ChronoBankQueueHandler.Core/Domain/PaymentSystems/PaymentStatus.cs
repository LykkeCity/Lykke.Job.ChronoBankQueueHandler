namespace Lykke.Job.ChronoBankQueueHandler.Core.Domain.PaymentSystems
{
    public enum PaymentStatus
    {
        Created,
        NotifyProcessed,
        NotifyDeclined,
        Processing
    }
}