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
    public class CleanUpService : ICleanUpService
    {
        private readonly IWalletService _walletService;
        private readonly ICryptoService _cryptoService;
        private readonly IOptions<ConfigSettings> _options;
        private readonly ITransactionService _transactionService;

        public CleanUpService(IWalletService walletService,
                              ICryptoService cryptoService,
                              IOptions<ConfigSettings> options,
                              ITransactionService transactionService)
        {
            _walletService = walletService;
            _cryptoService = cryptoService;
            _options = options;
            _transactionService = transactionService;
        }

        public async Task<bool> CleanUpAsync()
        {
            await _transactionService.ExchangeAllTokensToHotWalletAsync();
            await _transactionService.TransferAllUSDCToHotWalletAsync();
            await _transactionService.TransferAllSOLToHotWalletAsync();

            return true;
        }
    }
}
