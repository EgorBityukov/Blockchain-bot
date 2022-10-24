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

        public async Task StartTransationsAsync(CancellationToken cancellationToken)
        {
            //ThreadPool.QueueUserWorkItem(async () =>{);

            while (!cancellationToken.IsCancellationRequested)
            {

            }
        }

        public async Task ExchangeTokenAsync(WalletModel hotWalletFeePayer, WalletModel coldWallet, string inputMint, string outputMint, string amount)
        {
            var quotes = await _jupService.GetQuoteAsync(inputMint, outputMint, amount);

            JupSwapRequestModel jupSwapRequestModel = new JupSwapRequestModel();

            jupSwapRequestModel.route = quotes.data[0];
            jupSwapRequestModel.feeAccount = hotWalletFeePayer.PublicKey;
            jupSwapRequestModel.userPublicKey = coldWallet.PublicKey;
            jupSwapRequestModel.destinationWallet = hotWalletFeePayer.PublicKey;
            jupSwapRequestModel.wrapUnwrapSOL = true;

            var swapTransactons = await _jupService.GetSwapTransactionsAsync(jupSwapRequestModel);

            string transactionId;

            if (swapTransactons.setupTransaction != null)
            {
                transactionId = await _cryptoService.SignTransactionAsync(coldWallet.PrivateKey, swapTransactons.setupTransaction);
                await CreateTransactionAsync(transactionId, "Setup");
            }
            if (swapTransactons.swapTransaction != null)
            {
                transactionId = await _cryptoService.SignTransactionAsync(coldWallet.PrivateKey, swapTransactons.swapTransaction);
                await CreateTransactionAsync(transactionId, "Exchange");
            }
            if (swapTransactons.cleanupTransaction != null)
            {
                transactionId = await _cryptoService.SignTransactionAsync(coldWallet.PrivateKey, swapTransactons.cleanupTransaction);
                await CreateTransactionAsync(transactionId, "CleanUp");
            }
        }

        public async Task TransferSolAsync(WalletModel fromWallet, WalletModel toWallet, long lamports, double sol)
        {
            string txid;

            txid = await _cryptoService.TransferSolToAnotherWalletAsync(fromWallet.PrivateKey, toWallet.PublicKey, fromWallet.SOL);

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
                        if (!token.Mint.Equals(_options.Value.USDCmint) && token.AmountDouble != 0.0)
                        {
                            _ = Task.Factory.StartNew(() =>
                            ExchangeTokenAsync(hotWallet, coldWallet, token.Mint, _options.Value.USDCmint, token.Amount), TaskCreationOptions.AttachedToParent);
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
                    && coldWallet.Tokens.Where(t => t.Mint == USDCmint).FirstOrDefault().AmountDouble != 0.0)
                    {
                    //_ = Task.Factory.StartNew(() =>
                    await TransferTokenAsync(coldWallet, hotWallet, USDCmint,
                        coldWallet.Tokens.Where(t => t.Mint == USDCmint).First().Amount);//, TaskCreationOptions.AttachedToParent);
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
                    //_ = Task.Factory.StartNew(() =>
                    await TransferSolAsync(coldWallet, hotWallet, coldWallet.Lamports, coldWallet.SOL);//, TaskCreationOptions.AttachedToParent);
                    }
                }
            //});

            //outer.Wait();
        }

        public async Task<mmTransactionModel> GetInfoAboutTransactionAsync(string txid)
        {
            TransactionInfoResponseModel transactionInfo = await _cryptoService.GetInfoAboutTransactionAsync(txid);
            mmTransactionModel transaction;
            try
            {
                transaction = new mmTransactionModel();
                transaction.Status = transactionInfo.result.meta.status.Err == null ? "Ok" : "Error";
                transaction.Date = DateTime.Now;
                transaction.WalletAddress = transactionInfo.result.transaction.feePayer;
                transaction.SendTokenMint = transactionInfo.result.meta.preTokenBalances
                 .First().mint;
                transaction.SendTokenCount = (double)(transactionInfo.result.meta.postTokenBalances
                 .First().uiTokenAmount.uiAmount.GetValueOrDefault(0) - transactionInfo.result.meta.preTokenBalances.First().uiTokenAmount.uiAmount.GetValueOrDefault(0));
                    transaction.RecieveTokenMint = transactionInfo.result.meta.postTokenBalances
                    .Last().mint;
                transaction.RecieveTokenCount = (double)(transactionInfo.result.meta.preTokenBalances
                .Last().uiTokenAmount.uiAmount.GetValueOrDefault(0) - transactionInfo.result.meta.postTokenBalances.Last().uiTokenAmount.uiAmount.GetValueOrDefault(0));
                   transaction.BalanceXToken = (double)transactionInfo.result.meta.postBalances.First() / 1000000000;
               
            }catch (Exception ex)
            {
                transaction = null;
            }

            return transaction;
        }

        public async Task<mmTransactionModel> CreateTransactionAsync(string txid, string operationType)
        {
            var tx = await GetInfoAboutTransactionAsync(txid);


            if (tx.Status.Equals("Ok"))
            {
                //Update USDC Token Balance
                var tokens = await _walletService.GetWalletTokensAsync(tx.WalletAddress);
                tx.BalanceUSDCToken = tokens.Where(t => t.Mint.Equals(_options.Value.USDCmint)).Select(t => t.AmountDouble).FirstOrDefault();
            }

            tx.OperationType = operationType;

            await AddTransactionAsync(tx);

            return tx;
        }
    }
}
