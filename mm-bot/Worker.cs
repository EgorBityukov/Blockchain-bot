

using mm_bot.Services.Interfaces;

namespace mm_bot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWalletService _walletService;

        public Worker(ILogger<Worker> logger, 
            IConfiguration configuration, 
            IWalletService walletService)
        {
            _logger = logger;
            _configuration = configuration;
            _walletService = walletService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //var list = await _walletService.GenerateWallets(1);
                var s = _configuration.GetSection("Settings");


                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}