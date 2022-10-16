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

        public CleanUpService(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<bool> StartCleanUpAsync()
        {
            return true;
        }
    }
}
