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
        private readonly ITransactionService _transactionService;

        public CleanUpService(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<bool> CleanUpAsync()
        {
            await _transactionService.ExchangeAllTokensOnUSDCAsync();
            //await _transactionService.TransferAllUSDCToHotWalletAsync();
            //await _transactionService.TransferAllSOLToHotWalletAsync();

            return true;
        }
    }
}
