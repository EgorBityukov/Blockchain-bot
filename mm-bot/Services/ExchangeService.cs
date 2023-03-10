using Microsoft.Extensions.Options;
using mm_bot.Helpers;
using mm_bot.Models;
using mm_bot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Services
{
    public class ExchangeService : IExchangeService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptions<ConfigSettings> _options;
        private readonly ITransactionService _transactionService;
        private readonly IWalletService _walletService;
        private readonly IRaydiumService _raydiumService;

        private RandomGenerator rnd = new RandomGenerator();

        public ExchangeService(ILogger<Worker> logger,
                               IOptions<ConfigSettings> options,
                               ITransactionService transactionService,
                               IWalletService walletService,
                               IRaydiumService raydiumService)
        {
            _logger = logger;
            _transactionService = transactionService;
            _walletService = walletService;
            _options = options;
            _raydiumService = raydiumService;
        }

        public async Task StartExchangeAsync(CancellationTokenSource cancellationTokenSource)
        {
            if (await CheckBaseLimitsOfColdWalletsAsync(cancellationTokenSource))
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                   if (await CheckDailyTradingVolumeInUSDCperXtokenAsync(cancellationTokenSource))
                    {
                        var coldWallets = await _walletService.GetColdWalletsAsync();
                        var hotWallet = await _walletService.GetHotWalletAsync();

                        var coldWallet = coldWallets[new Random().Next(coldWallets.Count)];

                        coldWallet = await _walletService.UpdateWalletInfoWithTokensAsync(coldWallet);

                        if (await _transactionService.AllowedWalletExchange(coldWallet, _options.Value.MinimumDelayInSecondsForOneTransactionPerWallet))
                        {
                            if (coldWallet.Tokens.Where(t => t.Mint != _options.Value.USDCmint && t.Mint != _options.Value.XTokenMint && t.AmountDouble > 0.0m).Any())
                            {
                                coldWallet = await UpdateEnoughSolBalanceForColdWallet(hotWallet, coldWallet);

                                foreach (var token in coldWallet.Tokens)
                                {
                                    if (token.Mint != _options.Value.USDCmint && token.Mint != _options.Value.XTokenMint && token.AmountDouble > 0.0m)
                                    {
                                        await _transactionService.ExchangeTokenAsync(coldWallet, token.Mint, _options.Value.USDCmint, token.AmountDouble);
                                    }
                                }
                            }
                            else
                            {
                                decimal randRes = (decimal)new Random().NextDouble();
                                var USDCtoken = coldWallet.Tokens.Where(t => t.Mint == _options.Value.USDCmint).FirstOrDefault();

                                if (randRes < _options.Value.PercentageOfRandomTransactionsForAddToken)
                                {
                                    if (USDCtoken != null && USDCtoken.AmountDouble >= 1m)
                                    {
                                        var mint = await _transactionService.GetRandomMintAsync();
                                        coldWallet = await UpdateEnoughSolBalanceForColdWallet(hotWallet, coldWallet);

                                        var count = rnd.NextDecimal(1m, USDCtoken.AmountDouble);

                                        await _transactionService.ExchangeTokenAsync(coldWallet, _options.Value.USDCmint, mint, count);
                                    }
                                }
                                else
                                {
                                    var rates = await _raydiumService.GetExchangeRatesAsync();

                                    if (rates != null)
                                    {
                                        var rate = rates.Where(r => r.name.Equals("ARTZ-USDC")).FirstOrDefault();

                                        if (rate != null)
                                        {
                                            decimal price = rate.price.HasValue ? rate.price.Value : 0m;
                                            var artzToken = coldWallet.Tokens.Where(t => t.Mint == _options.Value.XTokenMint).FirstOrDefault();

                                            decimal artzAmount = artzToken != null ? artzToken.AmountDouble : 0m;

                                            if (artzAmount * price + USDCtoken.AmountDouble >= 2m)
                                            {
                                                coldWallet = await UpdateEnoughSolBalanceForColdWallet(hotWallet, coldWallet);

                                                if (artzAmount * price >= USDCtoken.AmountDouble)
                                                {
                                                    var count = rnd.NextDecimal(1m, artzAmount);

                                                    await _transactionService.ExchangeTokenAsync(coldWallet, _options.Value.XTokenMint, _options.Value.USDCmint, count);
                                                }
                                                else
                                                {
                                                    var count = rnd.NextDecimal(1m, USDCtoken.AmountDouble);

                                                    await _transactionService.ExchangeTokenAsync(coldWallet, _options.Value.USDCmint, _options.Value.XTokenMint, count);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public async Task<bool> CheckDailyTradingVolumeInUSDCperXtokenAsync(CancellationTokenSource cancellationTokenSource)
        {
            var dailyTradingVolumeInUSDCperXtoken = await _transactionService.GetDailyTradingVolumeInUSDCperXtokenAsync();

            if (dailyTradingVolumeInUSDCperXtoken > _options.Value.DailyTradingVolumeInUSDCperXtoken)
            {
                _logger.LogError("System stop. Daily Trading Volume In USDC per X-Token: {0}", dailyTradingVolumeInUSDCperXtoken);
                cancellationTokenSource.Cancel();
                return false;
            }

            return true;
        }

        public async Task<bool> CheckBaseLimitsOfColdWalletsAsync(CancellationTokenSource cancellationTokenSource)
        {
            var coldWallets = await _walletService.GetColdWalletsAsync();
            var hotWallet = await _walletService.GetHotWalletAsync();

            foreach (var coldWallet in coldWallets)
            {
                if (coldWallet.SOL < _options.Value.BaseVolumeSOLperColdWallet)
                {
                    var neededCount = _options.Value.BaseVolumeSOLperColdWallet - coldWallet.SOL;

                    if (hotWallet.SOL < neededCount)
                    {
                        _logger.LogError("Not enough SOL balance on Hot Wallet. Balance: {0}, tried to transfer: {1}", hotWallet.SOL, neededCount);
                        cancellationTokenSource.Cancel();

                        return false;
                    }
                    else
                    {
                        await _transactionService.TransferSolAsync(hotWallet, coldWallet, neededCount);
                    }
                    
                }

                if (coldWallet.Tokens.Where(t => t.Mint == _options.Value.USDCmint).FirstOrDefault() == null ||
                    coldWallet.Tokens.Where(t => t.Mint == _options.Value.USDCmint).FirstOrDefault().AmountDouble < _options.Value.BaseVolumeUSDCperColdWallet)
                {
                    var hotWalletUSDCcount = hotWallet.Tokens.Where(t => t.Mint == _options.Value.USDCmint).FirstOrDefault() == null ? 0m : hotWallet.Tokens.Where(t => t.Mint == _options.Value.USDCmint).FirstOrDefault().AmountDouble;
                    var tokenCount = coldWallet.Tokens.Where(t => t.Mint == _options.Value.USDCmint).FirstOrDefault() == null ? 0m : coldWallet.Tokens.Where(t => t.Mint == _options.Value.USDCmint).FirstOrDefault().AmountDouble;
                    var neededCount = _options.Value.BaseVolumeUSDCperColdWallet - tokenCount;

                    if (hotWalletUSDCcount < neededCount)
                    {
                        _logger.LogError("Not enough USDC balance on Hot Wallet. Balance: {0}, tried to transfer: {1}", hotWalletUSDCcount, neededCount);
                        cancellationTokenSource.Cancel();

                        return false;
                    }
                    else
                    {
                        await _transactionService.TransferTokenAsync(hotWallet, coldWallet, _options.Value.USDCmint, neededCount);
                    }
                }
            }

            return true;
        }

        public async Task<WalletModel> UpdateEnoughSolBalanceForColdWallet(WalletModel hotWallet, WalletModel coldWallet)
        {
            if (coldWallet.SOL < _options.Value.BaseVolumeSOLperColdWallet)
            {
                var neededCount = _options.Value.BaseVolumeSOLperColdWallet - coldWallet.SOL;
                await _transactionService.TransferSolAsync(hotWallet, coldWallet, neededCount);
                var wallet = await _walletService.UpdateWalletInfoWithoutTokensAsync(coldWallet);
                return wallet;
            }

            return coldWallet;
        }
    }
}
