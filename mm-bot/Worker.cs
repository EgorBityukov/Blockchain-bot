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
        private readonly ICommandService  _commandService;
        private readonly ITransactionService _transactionService;

        CancellationTokenSource cancellationTokenSourceTransactions = new CancellationTokenSource();

        public Worker(ILogger<Worker> logger, 
            IWalletService walletService,
            IOptions<ConfigSettings> options,
            ICommandService commandService,
            ITransactionService transactionService)
        {
            _logger = logger;
            _walletService = walletService;
            _options = options;
            _commandService = commandService;
            _transactionService = transactionService;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start");
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
            }

            await _walletService.AddHotWalletFromConfigAsync(_options.Value.HotWallet);

            _ = Task.Run(() => ListenForInput(cancellationTokenSourceTransactions));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _transactionService.StartTransationsAsync(cancellationTokenSourceTransactions.Token);

                _logger.LogInformation("Worker in time {time}", DateTime.Now);
                await Task.Delay(3000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping");
            await Task.CompletedTask;
        }

        private async Task ListenForInput(CancellationTokenSource cancellationTokenSourceTransactions)
        {
            while (true)
            {
                string userInput = Console.ReadLine();
                if (!String.IsNullOrWhiteSpace(userInput))
                {
                    _logger.LogInformation($"Executing user command {userInput}...");
                    var result = await _commandService.ProcessCommandAsync(userInput, cancellationTokenSourceTransactions);
                    cancellationTokenSourceTransactions = result;
                }    
            }
        }
    }
}