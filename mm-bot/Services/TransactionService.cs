using AutoMapper;
using Microsoft.Extensions.Options;
using mm_bot.Models;
using mm_bot.Models.ResponseModel;
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

        public TransactionService(ILogger<Worker> logger,
                                  IMapper mapper,
                                  IWalletService walletService,
                                  IOptions<ConfigSettings> options,
                                  ICryptoService cryptoService,
                                  ImmTransactionRepository mmTransactionRepository)
        {
            _logger = logger;
            _walletService = walletService;
            _options = options;
            _cryptoService = cryptoService;
            _mmTransactionRepository = mmTransactionRepository;
        }

        public async Task StartTransationsAsync(CancellationToken cancellationToken)
        {
            //ThreadPool.QueueUserWorkItem(async () =>{);

            while (!cancellationToken.IsCancellationRequested)
            {

            }
        }

        public async Task TransferSolAsync(WalletModel fromWallet, WalletModel toWallet, long lamports, double sol)
        {
            string txid;

            txid = await _cryptoService.TransferSolToAnotherWalletAsync(fromWallet.PrivateKey, toWallet.PublicKey, fromWallet.Lamports, fromWallet.SOL);

            var transaction = await CreateTransactionAsync(txid, "Transfer");

            if (transaction.Status.Equals("Ok"))
            {
                await _walletService.UpdateWalletInfoWithTokensAsync(fromWallet);
                await _walletService.UpdateWalletInfoWithTokensAsync(toWallet);
            }
        }

        public async Task TransferTokenAsync(WalletModel fromWallet, WalletModel toWallet, string mint, string count)
        {
            string txid;

            txid = await _cryptoService.TransferTokenToAnotherWalletAsync(fromWallet.PrivateKey, mint, toWallet.PublicKey, count);

            var transaction = await CreateTransactionAsync(txid, "Transfer");

            if (transaction.Status.Equals("Ok"))
            {
                await _walletService.UpdateWalletInfoWithTokensAsync(fromWallet);
                await _walletService.UpdateWalletInfoWithTokensAsync(toWallet);
            }
        }

        public async Task AddTransactionAsync(mmTransactionModel transaction)
        {
            await _mmTransactionRepository.AddTransaction(_mapper.Map<mmTransaction>(transaction));
        }

        public async Task ExchangeAllTokensToHotWalletAsync()
        {
            var hotWallet = await _walletService.GetHotWalletAsync();
            var coldWallets = await _walletService.GetColdWalletsAsync();
        }

        public async Task TransferAllUSDCToHotWalletAsync()
        {
            string USDCmint = _options.Value.USDCmint;
            var hotWallet = await _walletService.GetHotWalletAsync();
            var coldWallets = await _walletService.GetColdWalletsAsync();

            var outer = Task.Factory.StartNew(() =>
            {
                foreach (var coldWallet in coldWallets)
                {
                    if (coldWallet.Tokens.Where(t => t.Mint == USDCmint).FirstOrDefault() != null)
                    {
                        _ = Task.Factory.StartNew(() =>
                        TransferTokenAsync(coldWallet, hotWallet, USDCmint,
                            coldWallet.Tokens.Where(t => t.Mint == USDCmint).First().Amount), TaskCreationOptions.AttachedToParent);
                    }
                }
            });

            outer.Wait();
        }

        public async Task TransferAllSOLToHotWalletAsync()
        {
            var hotWallet = await _walletService.GetHotWalletAsync();
            var coldWallets = await _walletService.GetColdWalletsAsync();

            var outer = Task.Factory.StartNew(() =>
            {
                foreach (var coldWallet in coldWallets)
                {
                    if (coldWallet.SOL != 0)
                    {
                        _ = Task.Factory.StartNew(() =>
                        TransferSolAsync(coldWallet, hotWallet, coldWallet.Lamports, coldWallet.SOL), TaskCreationOptions.AttachedToParent);
                    }
                }
            });

            outer.Wait();
        }

        public async Task<mmTransactionModel> GetInfoAboutTransactionAsync(string txid)
        {
            TransactionInfoResponseModel transactionInfo = await _cryptoService.GetInfoAboutTransactionAsync(txid);
            mmTransactionModel transaction = new mmTransactionModel()
            {
                Status = transactionInfo.result.meta.status.Err == null ? "Ok" : "Error",
                Date = DateTime.Now,
                WalletAddress = transactionInfo.result.transaction.feePayer,
                SendTokenMint = transactionInfo.result.meta.preTokenBalances
                .Where(q => q.accountIndex == 1).First().mint,
                SendTokenCount = transactionInfo.result.meta.postTokenBalances
                .Where(q => q.accountIndex == 1).First().uiTokenAmount.uiAmount - transactionInfo.result.meta.preTokenBalances.Where(q => q.accountIndex == 1).First().uiTokenAmount.uiAmount,
                RecieveTokenMint = transactionInfo.result.meta.postTokenBalances
                .Where(q => q.accountIndex == 2).First().mint,
                RecieveTokenCount = transactionInfo.result.meta.preTokenBalances
                .Where(q => q.accountIndex == 2).First().uiTokenAmount.uiAmount - transactionInfo.result.meta.postTokenBalances.Where(q => q.accountIndex == 2).First().uiTokenAmount.uiAmount,
                BalanceXToken = (double)transactionInfo.result.meta.postBalances.First() / 1000000000
            };

            return transaction;
        }

        public async Task<mmTransactionModel> CreateTransactionAsync(string txid, string operationType)
        {
            var tx = await GetInfoAboutTransactionAsync(txid);


            if (tx.Status.Equals("Ok"))
            {
                var tokens = await _walletService.GetWalletTokensAsync(tx.WalletAddress);
                tx.BalanceUSDCToken = tokens.Where(t => t.Mint.Equals(_options.Value.USDCmint)).Select(t => t.AmountDouble).FirstOrDefault();
            }

            tx.OperationType = operationType;

            await AddTransactionAsync(tx);

            return tx;
        }
    }
}
