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

        public CommandService(ICleanUpService cleanUpService)
        {
            _cleanUpService = cleanUpService;
        }

        public async Task<CancellationTokenSource> ProcessCommandAsync(string command, CancellationTokenSource cancellationTokenSourceTransactions)
        {
            if (command.Equals("-cleanup"))
            {
                cancellationTokenSourceTransactions.Cancel();
                var result = await _cleanUpService.CleanUpAsync();
                cancellationTokenSourceTransactions = new CancellationTokenSource();
                return cancellationTokenSourceTransactions;
            }

            return cancellationTokenSourceTransactions;
        }
    }
}
