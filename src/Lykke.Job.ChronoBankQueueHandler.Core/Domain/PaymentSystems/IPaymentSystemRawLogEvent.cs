using System;

namespace Lykke.Job.ChronoBankQueueHandler.Core.Domain.PaymentSystems
{
    public interface IPaymentSystemRawLogEvent
    {
        DateTime DateTime { get; }
        string PaymentSystem { get; }

        string EventType { get; }
        string Data { get; }

    }
}