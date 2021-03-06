﻿using System;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using Lykke.Job.ChronoBankQueueHandler.Core.Domain.PaymentSystems;

namespace Lykke.Job.ChronoBankQueueHandler.AzureRepositories.PaymentSystems
{
    public class PaymentTransactionsRepository : IPaymentTransactionsRepository
    {
        private readonly INoSQLTableStorage<PaymentTransactionEntity> _tableStorage;
        private readonly INoSQLTableStorage<AzureMultiIndex> _tableStorageIndices;

        private const string IndexPartitinKey = "IDX";

        public PaymentTransactionsRepository(INoSQLTableStorage<PaymentTransactionEntity> tableStorage,
            INoSQLTableStorage<AzureMultiIndex> tableStorageIndices)
        {
            _tableStorage = tableStorage;
            _tableStorageIndices = tableStorageIndices;
        }

        public async Task CreateAsync(IPaymentTransaction src)
        {

            var commonEntity = PaymentTransactionEntity.Create(src);
            commonEntity.PartitionKey = PaymentTransactionEntity.IndexCommon.GeneratePartitionKey();
            await _tableStorage.InsertAndGenerateRowKeyAsDateTimeAsync(commonEntity, src.Created);

            var entityByClient = PaymentTransactionEntity.Create(src);
            entityByClient.PartitionKey = PaymentTransactionEntity.IndexByClient.GeneratePartitionKey(src.ClientId);
            entityByClient.RowKey = PaymentTransactionEntity.IndexByClient.GenerateRowKey(src.Id);


            var index = AzureMultiIndex.Create(IndexPartitinKey, src.Id, commonEntity, entityByClient);


            await Task.WhenAll(
                _tableStorage.InsertAsync(entityByClient),
                _tableStorageIndices.InsertAsync(index)
            );

        }

        public async Task<IPaymentTransaction> TryCreateAsync(IPaymentTransaction paymentTransaction)
        {
            if (paymentTransaction == null) throw new ArgumentNullException(nameof(paymentTransaction));

            var existingRecord =
                await
                    _tableStorage.GetDataAsync(
                        PaymentTransactionEntity.IndexByClient.GeneratePartitionKey(paymentTransaction.ClientId),
                        PaymentTransactionEntity.IndexByClient.GenerateRowKey(paymentTransaction.Id));

            if (existingRecord != null)
                return null;

            await CreateAsync(paymentTransaction);

            return paymentTransaction;
        }

        public async Task<IPaymentTransaction> SetAsOkAsync(string id, double depositedAmount, double? rate)
        {
            return await _tableStorageIndices.MergeAsync(IndexPartitinKey, id, _tableStorage, entity =>
            {
                entity.SetPaymentStatus(PaymentStatus.NotifyProcessed);
                entity.DepositedAmount = depositedAmount;
                entity.Rate = rate;
                return entity;
            });
        }
    }
}