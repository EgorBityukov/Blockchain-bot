using mm_bot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Services
{
    public class CommandService : ICommandService
    {
        private readonly ICleanUpService _cleanUpService;
        private readonly IWalletService _walletService;

        public CommandService(ICleanUpService cleanUpService,
                              IWalletService walletService)
        {
            _cleanUpService = cleanUpService;
            _walletService = walletService;
        }

        public async Task<CancellationTokenSource> ProcessCommandAsync(string command, CancellationTokenSource cancellationTokenSourceTransactions)
        {
            if (command.Equals("cleanup"))
            {
                cancellationTokenSourceTransactions.Cancel();
                await _cleanUpService.CleanUpAsync();
                return cancellationTokenSourceTransactions;
            }
            if (command.Equals("start"))
            {
                await _walletService.UpdateColdWalletsAsync();
                await _walletService.UpdateHotWalletAsync(true);
                cancellationTokenSourceTransactions = new CancellationTokenSource();
                return cancellationTokenSourceTransactions;
            }

            return cancellationTokenSourceTransactions;
        }
    }
}
