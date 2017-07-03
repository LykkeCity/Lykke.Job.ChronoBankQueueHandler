using System.Threading.Tasks;

namespace Lykke.Job.ChronoBankQueueHandler.Core.Domain.BitCoin
{
    public interface IWalletCredentialsRepository
    {
        Task<IWalletCredentials> GetByChronoBankContractAsync(string contract);
    }
}