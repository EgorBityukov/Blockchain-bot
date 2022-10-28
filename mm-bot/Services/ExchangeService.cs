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
        private readonly IOptions<ConfigSettings> _options;
        private readonly ITransactionService _transactionService;
        private readonly IWalletService _walletService;
        private readonly IRaydiumService _raydiumService;

        private RandomGenerator rnd = new RandomGenerator();

        public ExchangeService(IOptions<ConfigSettings> options,
                               ITransactionService transactionService,
                               IWalletService walletService,
                               IRaydiumService raydiumService)
        {
            _transactionService = transactionService;
            _walletService = walletService;
            _options = options;
            _raydiumService = raydiumService;
        }

        public async Task StartExchangeAsync(CancellationTokenSource cancellationTokenSource)
        {
            //ThreadPool.QueueUserWorkItem(async () =>{);
            await CheckBaseLimitsOfColdWalletsAsync();

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                await CheckDailyTradingVolumeInUSDCperXtokenAsync(cancellationTokenSource);

                var coldWallets = await _walletService.GetColdWalletsAsync();
                var hotWallet = await _walletService.GetHotWalletAsync();

                var coldWallet = coldWallets[new Random().Next(coldWallets.Count)];

                if (await _transactionService.AllowedWalletExchange(coldWallet, _options.Value.MinimumDelayInSecondsForOneTransactionPerWallet))
                {
                    if (coldWallet.Tokens.Where(t => t.Mint != _options.Value.USDCmint && t.AmountDouble > 0.0m).Any())
                    {
                        coldWallet = await UpdateEnoughSolBalanceForColdWallet(hotWallet, coldWallet);

                        foreach (var token in coldWallet.Tokens)
                        {
                            if (token.AmountDouble > 0.0m)
                            {
                                await _transactionService.ExchangeTokenAsync(hotWallet, coldWallet, token.Mint, _options.Value.USDCmint, token.AmountDouble);
                            }
                        }
                    }
                    else
                    {
                        decimal randRes = (decimal)new Random().NextDouble();
                        var USDCtoken = coldWallet.Tokens.Where(t => t.Mint == _options.Value.USDCmint).FirstOrDefault();

                        if (randRes < _options.Value.PercentageOfRandomTransactionsForAddToken)
                        {
                            if (USDCtoken != null && USDCtoken.AmountDouble >= 1.0m)
                            {
                                var mint = await _transactionService.GetRandomMintAsync();
                                coldWallet = await UpdateEnoughSolBalanceForColdWallet(hotWallet, coldWallet);

                                var count = rnd.NextDecimal(1, USDCtoken.AmountDouble);

                                await _transactionService.ExchangeTokenAsync(hotWallet, coldWallet, USDCtoken.Mint, mint, count);
                            }
                        }
                        else
                        {
                            var rates = await _raydiumService.GetExchangeRatesAsync();

                            if (rates != null)
                            {
                                var rate = rates.Where(r => r.name.Equals("SOL-USDC")).FirstOrDefault();
                                
                                if (rate != null)
                                {
                                    decimal price = rate.price.HasValue ? rate.price.Value : 0m;

                                    if (coldWallet.SOL * price + USDCtoken.AmountDouble >= 2m)
                                    {
                                        coldWallet = await UpdateEnoughSolBalanceForColdWallet(hotWallet, coldWallet);
                                    }

                                    if (coldWallet.SOL * price >= USDCtoken.AmountDouble)
                                    {
                                        var count = rnd.NextDecimal(1, coldWallet.SOL);

                                        await _transactionService.ExchangeTokenAsync(hotWallet, coldWallet, _options.Value.XTokenMint ,USDCtoken.Mint, count);
                                    }
                                    else
                                    {
                                        var count = rnd.NextDecimal(1, USDCtoken.AmountDouble);

                                        await _transactionService.ExchangeTokenAsync(hotWallet, coldWallet, USDCtoken.Mint, _options.Value.XTokenMint, count);
                                    }
                                }
                                
                            }
                        }
                    }
                }
            }
        }

        public async Task CheckDailyTradingVolumeInUSDCperXtokenAsync(CancellationTokenSource cancellationTokenSource)
        {
            var dailyTradingVolumeInUSDCperXtoken = await _transactionService.GetDailyTradingVolumeInUSDCperXtokenAsync();

            if (dailyTradingVolumeInUSDCperXtoken > _options.Value.DailyTradingVolumeInUSDCperXtoken)
            {
                cancellationTokenSource.Cancel();
            }
        }

        public async Task CheckBaseLimitsOfColdWalletsAsync()
        {
            var coldWallets = await _walletService.GetColdWalletsAsync();
            var hotWallet = await _walletService.GetHotWalletAsync();

            foreach (var coldWallet in coldWallets)
            {
                if (coldWallet.SOL < _options.Value.BaseVolumeSOLperColdWallet)
                {
                    var neededCount = _options.Value.BaseVolumeSOLperColdWallet - coldWallet.SOL;
                    await _transactionService.TransferSolAsync(hotWallet, coldWallet, neededCount);
                }

                if (coldWallet.Tokens.Where(t => t.Mint == _options.Value.USDCmint).FirstOrDefault() == null ||
                    coldWallet.Tokens.Where(t => t.Mint == _options.Value.USDCmint).FirstOrDefault().AmountDouble < _options.Value.BaseVolumeUSDCperColdWallet)
                {
                    var tokenCount = coldWallet.Tokens.Where(t => t.Mint == _options.Value.USDCmint).FirstOrDefault() == null ? 0m : coldWallet.Tokens.Where(t => t.Mint == _options.Value.USDCmint).FirstOrDefault().AmountDouble;
                    var neededCount = _options.Value.BaseVolumeUSDCperColdWallet - tokenCount;
                    await _transactionService.TransferSolAsync(hotWallet, coldWallet, neededCount);
                }
            }
        }

        public async Task<WalletModel> UpdateEnoughSolBalanceForColdWallet(WalletModel hotWallet, WalletModel coldWallet)
        {
            if (coldWallet.SOL < 0.005m)
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
