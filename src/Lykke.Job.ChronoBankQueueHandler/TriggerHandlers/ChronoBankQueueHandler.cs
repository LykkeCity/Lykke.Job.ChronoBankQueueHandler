using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Job.ChronoBankQueueHandler.Contract;
using Lykke.Job.ChronoBankQueueHandler.Core;
using Lykke.Job.ChronoBankQueueHandler.Core.Domain.BitCoin;
using Lykke.Job.ChronoBankQueueHandler.Core.Domain.PaymentSystems;
using Lykke.Job.ChronoBankQueueHandler.Services.Exchange;
using Lykke.JobTriggers.Triggers.Attributes;

namespace Lykke.Job.ChronoBankQueueHandler.TriggerHandlers
{
    public class ChronoBankQueueHandler
    {
        private readonly IWalletCredentialsRepository _walletCredentialsRepository;
        private readonly ILog _log;
        private readonly ExchangeOperationsService _exchangeOperationsService;
        private readonly IPaymentSystemsRawLog _paymentSystemsRawLog;
        private readonly IPaymentTransactionsRepository _paymentTransactionsRepository;

        public ChronoBankQueueHandler(IWalletCredentialsRepository walletCredentialsRepository, ILog log,
            ExchangeOperationsService exchangeOperationsService, IPaymentSystemsRawLog paymentSystemsRawLog,
            IPaymentTransactionsRepository paymentTransactionsRepository)
        {
            _walletCredentialsRepository = walletCredentialsRepository;
            _log = log;
            _exchangeOperationsService = exchangeOperationsService;
            _paymentSystemsRawLog = paymentSystemsRawLog;
            _paymentTransactionsRepository = paymentTransactionsRepository;
        }

        [QueueTrigger("chronobank-in")]
        public async Task ProcessInMessage(ChronoBankCashInMsg msg)
        {
            var logTask = _log.WriteInfoAsync(nameof(ChronoBankQueueHandler), nameof(ProcessInMessage), msg.ToJson(),
                "Message received");

            try
            {
                var walletCreds = await _walletCredentialsRepository.GetByChronoBankContractAsync(msg.Contract);
                if (walletCreds == null)
                {
                    await
                        _log.WriteWarningAsync(nameof(ChronoBankQueueHandler), nameof(ProcessInMessage), msg.ToJson(),
                            "Wallet not found");
                    return;
                }

                await
                    _paymentSystemsRawLog.RegisterEventAsync(
                        PaymentSystemRawLogEvent.Create(CashInPaymentSystem.ChronoBank, "Msg received",
                            msg.ToJson()));

                var txId = $"{msg.TransactionHash}_{msg.Contract}";

                var pt = await _paymentTransactionsRepository.TryCreateAsync(PaymentTransaction.Create(
                    txId, CashInPaymentSystem.ChronoBank, walletCreds.ClientId, msg.Amount,
                    LykkeConstants.ChronoBankAssetId, LykkeConstants.ChronoBankAssetId, null));

                if (pt == null)
                {
                    await
                        _log.WriteWarningAsync(nameof(ChronoBankQueueHandler), nameof(ProcessInMessage), msg.ToJson(),
                            "Transaction already handled");
                    //return if was handled previously
                    return;
                }

                var result = await
                    _exchangeOperationsService.IssueAsync(walletCreds.ClientId, LykkeConstants.ChronoBankAssetId,
                        msg.Amount);

                if (!result.IsOk())
                {
                    await
                        _log.WriteWarningAsync(nameof(ChronoBankQueueHandler), msg.ToJson(), result.ToJson(),
                            "ME error");
                    return;
                }

                await
                    _paymentTransactionsRepository.SetAsOkAsync(pt.Id, msg.Amount, null);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(ChronoBankQueueHandler), nameof(ProcessInMessage), msg.ToJson(), ex);
            }
            finally
            {
                await logTask;
            }
        }
    }
}