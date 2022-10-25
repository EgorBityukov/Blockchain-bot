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
    public class ExchangeService : IExchangeService
    {
        private readonly IOptions<ConfigSettings> _options;
        private readonly ITransactionService _transactionService;
        private readonly IWalletService _walletService;

        public ExchangeService(IOptions<ConfigSettings> options,
                               ITransactionService transactionService,
                               IWalletService walletService)
        {
            _transactionService = transactionService;
            _walletService = walletService;
        }

        public async Task StartExchangeAsync(CancellationTokenSource cancellationTokenSource)
        {
            //ThreadPool.QueueUserWorkItem(async () =>{);

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                await CheckDailyTradingVolumeInUSDCperXtokenAsync(cancellationTokenSource);

                var coldWallets = await _walletService.GetColdWalletsAsync();
                var hotWallet = await _walletService.GetHotWalletAsync();

                foreach (var coldWallet in coldWallets)
                {
                    if (await _transactionService.AllowedWalletExchange(coldWallet, _options.Value.MinimumDelayInSecondsForOneTransactionPerWallet))
                    {

                    }
                }
            }
        }

        public async Task CheckDailyTradingVolumeInUSDCperXtokenAsync(CancellationTokenSource cancellationTokenSource)
        {
            var dailyTradingVolumeInUSDCperXtoken = await _transactionService.GetDailyTradingVolumeInUSDCperXtokenAsync();

            if (dailyTradingVolumeInUSDCperXtoken > _options.Value.DailyTradingVolumeInUSDCperXtoken)
            {
                cancellationTokenSource.Cancel();
            }
        }
    }
}
