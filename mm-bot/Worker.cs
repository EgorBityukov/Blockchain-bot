using Microsoft.Extensions.Options;
using mm_bot.Models;
using mm_bot.Services.Interfaces;

namespace mm_bot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IWalletService _walletService;
        private readonly IOptions<ConfigSettings> _options;
        private readonly ICommandService _commandService;
        private readonly IExchangeService _exchangeService;

        CancellationTokenSource cancellationTokenSourceTransactions = new CancellationTokenSource();

        public Worker(ILogger<Worker> logger,
                      IWalletService walletService,
                      IOptions<ConfigSettings> options,
                      ICommandService commandService,
                      IExchangeService exchangeService)
        {
            _logger = logger;
            _walletService = walletService;
            _options = options;
            _commandService = commandService;
            _exchangeService = exchangeService;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start");

            var args = Environment.GetCommandLineArgs();

            if (args.Length > 0)
            {
                if (args.Contains("cleanup"))
                {
                    _logger.LogInformation("CleanUp command START");

                    var result = await _commandService.ProcessCommandAsync("cleanup", cancellationTokenSourceTransactions);

                    _logger.LogInformation("CleanUp command END");

                    lock (cancellationTokenSourceTransactions)
                    {
                        cancellationTokenSourceTransactions = result;
                    }
                }
            }

            if (!cancellationTokenSourceTransactions.IsCancellationRequested)
            {
                await base.StartAsync(cancellationToken);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Generation wallets {genWallets}", _options.Value.AutomaticGenerationOfWallets);

            //Generate wallets
            if (_options.Value.AutomaticGenerationOfWallets)
            {
                _logger.LogInformation("Start wallets generation (Count: {count})", _options.Value.ColdWalletsCount);
                await _walletService.DeleteAllWalletsAsync();
                await _walletService.GenerateWalletsAsync(_options.Value.ColdWalletsCount);
                _options.Value.AutomaticGenerationOfWallets = false;
            }
            else
            {
                await _walletService.AddColdWalletsFromConfigAsync(_options.Value.ColdWallet);
                await _walletService.UpdateColdWalletsAsync();
            }

            //Will not add hot wallet, if already exsist, update it
            await _walletService.AddHotWalletFromConfigAsync(_options.Value.HotWallet);

            //Monitoring Sol balance on Hot Wallet
            _ = Task.Run(() => _walletService.MonitoringSolBalanceAsync(cancellationTokenSourceTransactions, stoppingToken));

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!cancellationTokenSourceTransactions.IsCancellationRequested)
                {
                    _logger.LogInformation("Start Exchange");
                    //Start exchange transactions
                    await _exchangeService.StartExchangeAsync(cancellationTokenSourceTransactions);
                    _logger.LogInformation("Stop Exchange");
                }
                else
                {
                    _logger.LogInformation("Program stopped");
                }

                // 5 minut delay
                await Task.Delay(300000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping");
            await Task.CompletedTask;
        }

        //private async Task ListenForInput(CancellationToken cancellationToken)
        //{
        //    while (!cancellationToken.IsCancellationRequested)
        //    {
        //        string userInput = Console.ReadLine();

        //        if (!String.IsNullOrWhiteSpace(userInput))
        //        {
        //            _logger.LogInformation($"Executing user command {userInput}...");

        //            cancellationTokenSourceTransactions.Cancel();
        //            var result = await _commandService.ProcessCommandAsync(userInput, cancellationTokenSourceTransactions);

        //            _logger.LogInformation($"End user command {userInput}");

        //            lock (cancellationTokenSourceTransactions)
        //            {
        //                cancellationTokenSourceTransactions = result;
        //            }
        //        }
        //    }
        //}
    }
}