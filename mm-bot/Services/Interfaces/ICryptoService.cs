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
    }
}
