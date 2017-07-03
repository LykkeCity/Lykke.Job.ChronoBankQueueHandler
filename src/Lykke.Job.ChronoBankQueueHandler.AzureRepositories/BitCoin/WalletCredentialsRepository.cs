using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.ChronoBankQueueHandler.Core.Domain.BitCoin;

namespace Lykke.Job.ChronoBankQueueHandler.AzureRepositories.BitCoin
{
    public class WalletCredentialsRepository : IWalletCredentialsRepository
    {
        private readonly INoSQLTableStorage<WalletCredentialsEntity> _tableStorage;

        public WalletCredentialsRepository(INoSQLTableStorage<WalletCredentialsEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }
        
        public async Task<IWalletCredentials> GetByChronoBankContractAsync(string contract)
        {
            var partitionKey = WalletCredentialsEntity.ByChronoBankContract.GeneratePartitionKey();
            var rowKey = WalletCredentialsEntity.ByChronoBankContract.GenerateRowKey(contract);

            return await _tableStorage.GetDataAsync(partitionKey, rowKey);
        }
    }
}