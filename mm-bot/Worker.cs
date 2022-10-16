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

        private WalletModel HotWallet;
        private List<WalletModel> ColdWallets;

        public Worker(ILogger<Worker> logger, 
            IWalletService walletService,
            IOptions<ConfigSettings> options,
            ICommandService commandService)
        {
            _logger = logger;
            _walletService = walletService;
            _options = options;
            _commandService = commandService;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start");
            await base.StartAsync(cancellationToken);
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

            _ = Task.Run(() => ListenForInput(cancellationToken));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                

                _logger.LogInformation("Worker in time {time}", DateTime.Now);
                await Task.Delay(3000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping");
            await Task.CompletedTask;
        }

        void ListenForInput(CancellationToken cancellationToken)
        {
            while (true)
            {
                string userInput = Console.ReadLine();
                if (!String.IsNullOrWhiteSpace(userInput))
                {
                    _logger.LogInformation($"Executing user command {userInput}...");
                    _commandService.ProcessCommand(userInput);
                }    
            }
        }
    }
}