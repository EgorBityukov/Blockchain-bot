using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Services.Interfaces
{
    public interface ITransactionService
    {
        public Task StartTransationsAsync(CancellationToken cancellationToken);
        public Task ExchangeAllTokensToHotWalletAsync();
        public Task TransferUSDCToHotWalletAsync();
        public Task TransferSOLToHotWalletAsync();
    }
}
