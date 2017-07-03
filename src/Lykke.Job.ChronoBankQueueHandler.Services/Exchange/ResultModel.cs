namespace Lykke.Job.ChronoBankQueueHandler.Services.Exchange
{
    public class ResultModel
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public string TransactionId { get; set; }
    }
}