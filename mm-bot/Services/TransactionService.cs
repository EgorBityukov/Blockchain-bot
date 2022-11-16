using AutoMapper;
using Microsoft.Extensions.Options;
using mm_bot.Models;
using mm_bot.Models.ResponseModels;
using mm_bot.Services.Interfaces;
using mmTransactionDB.Models;
using mmTransactionDB.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IWalletService _walletService;
        private readonly IOptions<ConfigSettings> _options;
        private readonly ICryptoService _cryptoService;
        private readonly ImmTransactionRepository _mmTransactionRepository;
        private readonly IMapper _mapper;
        private readonly IJupService _jupService;

        public TransactionService(ILogger<Worker> logger,
                                  IMapper mapper,
                                  IWalletService walletService,
                                  IOptions<ConfigSettings> options,
                                  ICryptoService cryptoService,
                                  IJupService jupService,
                                  ImmTransactionRepository mmTransactionRepository)
        {
            _logger = logger;
            _walletService = walletService;
            _options = options;
            _cryptoService = cryptoService;
            _mmTransactionRepository = mmTransactionRepository;
            _jupService = jupService;
            _mapper = mapper;
        }

        public async Task ExchangeTokenAsync(WalletModel wallet, string inputMint, string outputMint, decimal amount)
        {
            var quotes = await _jupService.GetQuoteAsync(inputMint, outputMint, amount);

            JupSwapRequestModel jupSwapRequestModel = new JupSwapRequestModel();

            if (quotes != null)
            {
                jupSwapRequestModel.route = quotes.data[0];
                jupSwapRequestModel.feeAccount = wallet.PublicKey;
                jupSwapRequestModel.userPublicKey = wallet.PublicKey;
                jupSwapRequestModel.wrapUnwrapSOL = true;

                var swapTransactons = await _jupService.GetSwapTransactionsAsync(jupSwapRequestModel);

                string transactionId;

                if (swapTransactons.setupTransaction != null)
                {
                    transactionId = await _cryptoService.SignTransactionAsync(wallet.PrivateKey, swapTransactons.setupTransaction);

                    if (transactionId != null)
                    {
                        await CreateTransactionAsync(transactionId, "Setup", wallet, wallet, inputMint, outputMint, amount);
                    }
                }
                if (swapTransactons.swapTransaction != null)
                {
                    transactionId = await _cryptoService.SignTransactionAsync(wallet.PrivateKey, swapTransactons.swapTransaction);

                    if (transactionId != null)
                    {
                        await CreateTransactionAsync(transactionId, "Exchange", wallet, wallet, inputMint, outputMint, amount);
                    }
                }
                if (swapTransactons.cleanupTransaction != null)
                {
                    transactionId = await _cryptoService.SignTransactionAsync(wallet.PrivateKey, swapTransactons.cleanupTransaction);

                    if (transactionId != null)
                    {
                        await CreateTransactionAsync(transactionId, "CleanUp", wallet, wallet, inputMint, outputMint, amount);
                    }
                }
            }
        }

        public async Task TransferSolAsync(WalletModel fromWallet, WalletModel toWallet, decimal sol)
        {
            string txid;

            txid = await _cryptoService.TransferSolToAnotherWalletAsync(fromWallet.PrivateKey, toWallet.PublicKey, sol);

            await CreateTransactionAsync(txid, "Transfer", fromWallet, toWallet, "SOL", "SOL", sol);
        }

        public async Task TransferTokenAsync(WalletModel fromWallet, WalletModel toWallet, string mint, decimal count)
        {
            string txid;

            txid = await _cryptoService.TransferTokenToAnotherWalletAsync(fromWallet.PrivateKey, mint, toWallet.PublicKey, count);

            await CreateTransactionAsync(txid, "Transfer", fromWallet, toWallet, mint, mint, count);
        }

        public async Task AddTransactionAsync(mmTransactionModel transaction)
        {
            await _mmTransactionRepository.AddTransaction(_mapper.Map<mmTransaction>(transaction));
        }

        public async Task ExchangeAllTokensOnUSDCAsync()
        {
            var hotWallet = await _walletService.GetHotWalletAsync();
            var coldWallets = await _walletService.GetColdWalletsAsync();

            //var outer = Task.Factory.StartNew(() =>
            //{
            foreach (var coldWallet in coldWallets)
            {
                foreach (var token in coldWallet.Tokens)
                {
                    if (!token.Mint.Equals(_options.Value.USDCmint) && token.AmountDouble != 0.0m)
                    {
                        //_ = Task.Factory.StartNew(() =>
                        await ExchangeTokenAsync(coldWallet, token.Mint, _options.Value.USDCmint, token.AmountDouble);
                        //, TaskCreationOptions.AttachedToParent);
                    }
                }
            }
            //});

            //outer.Wait();
        }

        public async Task TransferAllUSDCToHotWalletAsync()
        {
            string USDCmint = _options.Value.USDCmint;
            var hotWallet = await _walletService.GetHotWalletAsync();
            var coldWallets = await _walletService.GetColdWalletsAsync();

            //var outer = Task.Factory.StartNew(() =>
            //{
            foreach (var coldWallet in coldWallets)
            {
                if (coldWallet.Tokens.Where(t => t.Mint == USDCmint).FirstOrDefault() != null
                && coldWallet.Tokens.Where(t => t.Mint == USDCmint).FirstOrDefault().AmountDouble != 0.0m)
                {
                    //_ = Task.Factory.StartNew(() =>
                    await TransferTokenAsync(coldWallet, hotWallet, USDCmint,
                       coldWallet.Tokens.Where(t => t.Mint == USDCmint).First().AmountDouble);
                    //, TaskCreationOptions.AttachedToParent);
                }
            }
            //});

            //outer.Wait();
        }

        public async Task TransferAllSOLToHotWalletAsync()
        {
            var hotWallet = await _walletService.GetHotWalletAsync();
            var coldWallets = await _walletService.GetColdWalletsAsync();

            //var outer = Task.Factory.StartNew(() =>
            //{
            foreach (var coldWallet in coldWallets)
            {
                if (coldWallet.SOL != 0)
                {
                    //await CloseTokenAccountsAsync(coldWallet, hotWallet);
                    //_ = Task.Factory.StartNew(() =>
                    if (coldWallet.SOL > 0.0025m)
                    {
                        await TransferSolAsync(coldWallet, hotWallet, coldWallet.SOL - 0.0025m);
                    }
                    //, TaskCreationOptions.AttachedToParent);
                }
            }
            //});

            //outer.Wait();
        }

        public async Task CloseTokenAccountsAsync(WalletModel coldWallet, WalletModel hotWallet)
        {
            foreach (var token in coldWallet.Tokens)
            {
                await _cryptoService.TransferTokenToAnotherWalletAsync(coldWallet.PrivateKey, token.Mint, hotWallet.PublicKey, 0, false);
            }
        }

        public async Task<mmTransactionModel> GetInfoAboutTransactionAsync(string txid, string operationType, string publicKey, string recieveMint)
        {
            if (txid == null)
            {
                return null;
            }

            TransactionInfoResponseModel transactionInfo = await _cryptoService.GetInfoAboutTransactionAsync(txid);
            mmTransactionModel transaction;

            try
            {
                if (transactionInfo != null && transactionInfo.result != null)
                {
                    transaction = new mmTransactionModel();
                    transaction.Status = transactionInfo.result.meta.status.Err == null ? "Ok" : "Error";

                    if (transaction.Status.Equals("Ok"))
                    {
                        if (operationType.Equals("Exchange"))
                        {
                            if (transactionInfo.result.meta.preTokenBalances != null && transactionInfo.result.meta.postTokenBalances != null)
                            {
                                if (transactionInfo.result.meta.preTokenBalances.Where(b => b.owner == publicKey && b.mint == recieveMint).Any() &&
                                    transactionInfo.result.meta.postTokenBalances.Where(b => b.owner == publicKey && b.mint == recieveMint).Any())
                                {
                                    transaction.RecieveTokenCount = transactionInfo.result.meta.postTokenBalances
                                                                .Where(b => b.owner == publicKey && b.mint == recieveMint).FirstOrDefault()
                                                                .uiTokenAmount.uiAmount.GetValueOrDefault(0) - transactionInfo.result.meta.preTokenBalances
                                                                .Where(b => b.owner == publicKey && b.mint == recieveMint).FirstOrDefault()
                                                                .uiTokenAmount.uiAmount.GetValueOrDefault(0);
                                }
                            }
                            else if (transactionInfo.result.meta.postTokenBalances != null)
                            {
                                if (transactionInfo.result.meta.postTokenBalances.Where(b => b.owner == publicKey && b.mint == recieveMint).Any())
                                {
                                    transaction.RecieveTokenCount = transactionInfo.result.meta.postTokenBalances
                                                                .Where(b => b.owner == publicKey && b.mint == recieveMint).FirstOrDefault()
                                                                .uiTokenAmount.uiAmount.GetValueOrDefault(0);
                                }
                            }
                            else if (recieveMint.Equals("SOL"))
                            {
                                try
                                {
                                    transaction.RecieveTokenCount = (decimal)(transactionInfo.result.meta.postBalances.FirstOrDefault()
                                                                        - transactionInfo.result.meta.preBalances.FirstOrDefault())
                                                                        / 1000000000;
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                    }
                    else
                    {
                        transaction.RecieveTokenCount = -1m;
                    }
                }
                else
                {
                    _logger.LogError("Transaction Service - GetInfoAboutTransactionAsync Exception: None, Transaction Info == null, txid = {0}", txid);
                    transaction = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Transaction Service - GetInfoAboutTransactionAsync Exception: {0}", ex.Message);
                transaction = null;
            }

            return transaction;
        }

        public async Task<mmTransactionModel> CreateTransactionAsync(string txid, string operationType,
            WalletModel sendWallet, WalletModel recieveWallet, string sendTokenMint, string recieveTokenMint, decimal sendAmount)
        {
            var tranInfo = await GetInfoAboutTransactionAsync(txid, operationType, recieveWallet.PublicKey, recieveTokenMint);

            var transaction = new mmTransactionModel()
            {
                txId = txid,
                Date = DateTime.UtcNow,
                OperationType = operationType,
                SendWalletAddress = sendWallet.PublicKey,
                SendTokenMint = sendTokenMint,
                SendTokenCount = sendAmount,
                RecieveWalletAddress = recieveWallet.PublicKey,
                RecieveTokenMint = recieveTokenMint
            };

            if (txid != null)
            {
                if (tranInfo != null && tranInfo.Status.Equals("Ok"))
                {
                    if (operationType.Equals("Transfer"))
                    {
                        transaction.RecieveTokenCount = sendAmount;
                    }
                    else if (operationType.Equals("Exchange"))
                    {
                        transaction.RecieveTokenCount = tranInfo.RecieveTokenCount;
                    }

                    transaction.Status = "Ok";

                    var updatedSendWallet = await _walletService.UpdateWalletInfoWithTokensAsync(sendWallet);

                    var tokens = updatedSendWallet.Tokens;
                    transaction.BalanceUSDCToken = tokens.Where(t => t.Mint.Equals(_options.Value.USDCmint)).Select(t => t.AmountDouble).FirstOrDefault();
                    transaction.BalanceXToken = tokens.Where(t => t.Mint.Equals(_options.Value.XTokenMint)).Select(t => t.AmountDouble).FirstOrDefault();

                    if (sendWallet.PublicKey != recieveWallet.PublicKey)
                    {
                        await _walletService.UpdateWalletInfoWithTokensAsync(recieveWallet);
                    }

                    if (!sendWallet.HotWallet && !recieveWallet.HotWallet)
                    {
                        await _walletService.UpdateHotWalletAsync(false);
                    }
                }
                else
                {
                    transaction.Status = "Error";
                    var tokens = recieveWallet.Tokens;
                    transaction.BalanceUSDCToken = tokens.Where(t => t.Mint.Equals(_options.Value.USDCmint)).Select(t => t.AmountDouble).FirstOrDefault();
                    transaction.BalanceXToken = tokens.Where(t => t.Mint.Equals(_options.Value.XTokenMint)).Select(t => t.AmountDouble).FirstOrDefault();
                    transaction.RecieveTokenCount = 0m;
                }
            }
            else
            {
                transaction.Status = "Error";
            }

            await AddTransactionAsync(transaction);

            return transaction;
        }

        public async Task<decimal> GetDailyTradingVolumeInUSDCperXtokenAsync()
        {
            var todayTran = await GetTodayTransactionsAsync();
            decimal dailyTradingVolumeInUSDCperXtoken;
            decimal dailyTradingVolumeInUSDCperXtokenSend = todayTran
                                                       .Where(t => t.SendTokenMint == _options.Value.USDCmint
                                                                    && t.OperationType == "Exchange"
                                                                    && t.Status == "Ok")
                                                       .Sum(p => p.SendTokenCount);
            decimal dailyTradingVolumeInUSDCperXtokenRecieve = todayTran
                                                                .Where(t => t.RecieveTokenMint == _options.Value.USDCmint
                                                                            && t.OperationType == "Exchange"
                                                                            && t.Status == "Ok")
                                                                .Sum(p => p.RecieveTokenCount);
            dailyTradingVolumeInUSDCperXtoken = dailyTradingVolumeInUSDCperXtokenSend + dailyTradingVolumeInUSDCperXtokenRecieve;

            return dailyTradingVolumeInUSDCperXtoken;
        }

        public async Task<bool> AllowedWalletExchange(WalletModel wallet, int delay)
        {
            var todayTran = await GetTodayTransactionsAsync();
            return !todayTran.Where(t => t.SendWalletAddress == wallet.PublicKey
                                 && t.OperationType == "Exchange"
                                 && t.Date > DateTime.UtcNow.AddSeconds(-delay)).Any();

        }

        public async Task<List<mmTransactionModel>> GetTodayTransactionsAsync()
        {
            return _mapper.Map<List<mmTransactionModel>>(await _mmTransactionRepository.GetTodayTransactionsAsync());
        }

        public async Task<string> GetRandomMintAsync()
        {
            string randomMint;

            var mints = await _jupService.GetMintsAsync();
            randomMint = mints[new Random().Next(mints.Count)];

            if (randomMint == _options.Value.USDCmint)
            {
                randomMint = mints[new Random().Next(mints.Count)];
            }

            return randomMint;
        }
    }
}
