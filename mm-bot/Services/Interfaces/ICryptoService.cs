using mm_bot.Models.ResponseModel;
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
        public Task<string> TransferLamportsToAnotherWalletAsync(string privateKey, string toPublicKey, double lamports, double sol);
        public Task<string> TransferTokenToAnotherWalletAsync(string privateKey, string mint, string toPublicKey, string count);
        public Task<TransactionInfoResponseModel> GetInfoAboutTransactionAsync(string txid);
    }
}
