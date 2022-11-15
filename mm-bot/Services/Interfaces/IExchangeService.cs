using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Services.Interfaces
{
    public interface IExchangeService
    {
        public Task StartExchangeAsync(CancellationTokenSource cancellationTokenSource);
        public Task<bool> CheckDailyTradingVolumeInUSDCperXtokenAsync(CancellationTokenSource cancellationTokenSource);
    }
}
