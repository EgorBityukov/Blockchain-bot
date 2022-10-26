using mm_bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Services.Interfaces
{
    public interface ITransactionService
    {
        public Task ExchangeAllTokensOnUSDCAsync();
        public Task TransferAllUSDCToHotWalletAsync();
        public Task TransferAllSOLToHotWalletAsync();
        public Task<mmTransactionModel> GetInfoAboutTransactionAsync(string txid, string operationType, string publicKey, string recieveMint);
        public Task AddTransactionAsync(mmTransactionModel transaction);
        public Task ExchangeTokenAsync(WalletModel hotWalletFeePayer, WalletModel coldWallet, string inputMint, string outputMint, decimal amount);
        public Task TransferSolAsync(WalletModel fromWallet, WalletModel toWallet, long lamports, decimal sol);
        public Task TransferTokenAsync(WalletModel fromWallet, WalletModel toWallet, string mint, decimal count);
        public Task<decimal> GetDailyTradingVolumeInUSDCperXtokenAsync();
        public Task<bool> AllowedWalletExchange(WalletModel wallet, int delay);
        public Task<List<mmTransactionModel>> GetTodayTransactionsAsync();
    }
}
