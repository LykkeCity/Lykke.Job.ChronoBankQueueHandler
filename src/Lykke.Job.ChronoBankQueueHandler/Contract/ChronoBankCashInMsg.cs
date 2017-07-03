namespace Lykke.Job.ChronoBankQueueHandler.Contract
{
    public class ChronoBankCashInMsg
    {
        public string Contract { get; set; }
        public double Amount { get; set; }
        public string TransactionHash { get; set; }
    }
}