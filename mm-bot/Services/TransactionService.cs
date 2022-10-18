using Microsoft.Extensions.Options;
using mm_bot.Models;
using mm_bot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IWalletService _walletService;
        private readonly IOptions<ConfigSettings> _options;

        public TransactionService(ILogger<Worker> logger,
                                  IWalletService walletService,
                                  IOptions<ConfigSettings> options)
        {
            _logger = logger;
            _walletService = walletService;
            _options = options;
        }

        public async Task ExchangeAllTokensToHotWalletAsync()
        {
            var hotWallet = await _walletService.GetHotWalletAsync();
            var coldWallets = await _walletService.GetColdWalletsAsync();
        }

        public async Task TransferUSDCToHotWalletAsync()
        {

        }

        public async Task TransferSOLToHotWalletAsync()
        {

        }

        public async Task StartTransationsAsync(CancellationToken cancellationToken)
        {
            //ThreadPool.QueueUserWorkItem(async () =>{);

            while (!cancellationToken.IsCancellationRequested)
            {

            }
        }

        public async Task MonitoringSolBalanceAsync(CancellationTokenSource cancellationTokenSourceTransactions)
        {
            bool hotWalletBalanceEnough = true;

            if (!hotWalletBalanceEnough)
            {
                cancellationTokenSourceTransactions.Cancel();
            }
        }
    }
}
