using mm_bot.Models.ResponseModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Services.Interfaces
{
    public interface ICryptoService
    {
        public Task<JObject> CreateWalletAsync();
        public Task<JObject> GetInfoAboutWalletAsync(string privateKey);
        public Task<List<WalletTokenResponseModel>> GetWalletTokensAsync(string publicKey);
        public Task<string> TransferSolToAnotherWalletAsync(string privateKey, string toPublicKey, decimal sol);
        public Task<string> TransferTokenToAnotherWalletAsync(string privateKey, string mint, string toPublicKey, decimal count, bool isForbiddenToCloseAccount = true);
        public Task<TransactionInfoResponseModel> GetInfoAboutTransactionAsync(string txid);
        public Task<string> SignTransactionAsync(string privateKey, string txid);
    }
}
