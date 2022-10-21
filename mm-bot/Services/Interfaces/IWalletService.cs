using mm_bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Services.Interfaces
{
    public interface IWalletService
    {
        public Task<List<WalletModel>> GenerateWalletsAsync(int CountWallets);
        public Task DeleteAllWalletsAsync();
        public Task<List<WalletModel>> GetWalletsAsync();
        public Task AddColdWalletsFromConfigAsync(List<MainWalletInfo> mainWalletInfos);
        public Task AddHotWalletFromConfigAsync(MainWalletInfo mainWalletInfo);
        public Task<WalletModel> GetHotWalletAsync();
        public Task<List<WalletModel>> GetColdWalletsAsync();
        public Task<WalletModel> GetInfoAboutWalletAsync(string privateKey);
        public Task<List<TokenModel>> GetWalletTokensAsync(string publicKey);
        public Task UpdateWalletInfoAsync(WalletModel wallet);
    }
}
