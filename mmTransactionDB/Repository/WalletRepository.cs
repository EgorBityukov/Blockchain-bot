using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using mmTransactionDB.DataAccess;
using mmTransactionDB.Models;
using mmTransactionDB.Repository.Interfaces;

namespace mmTransactionDB.Repository
{
    public class WalletRepository : IWalletRepository
    {
        private readonly mmTransactionDBContext _mmContext;

        public WalletRepository(IServiceScopeFactory factory)
        {
            _mmContext = factory.CreateScope().ServiceProvider.GetRequiredService<mmTransactionDBContext>();
        }

        public async Task AddWalletAsync(Wallet walletModel)
        {
            await _mmContext.Wallets.AddAsync(walletModel);
            await _mmContext.SaveChangesAsync();
        }

        public async Task AddWalletListAsync(List<Wallet> wallets)
        {
            await _mmContext.AddRangeAsync(wallets);
            await _mmContext.SaveChangesAsync();
        }

        public async Task UpdateWalletAsync(Wallet updateWallet)
        {
            var databaseWallet = _mmContext.Wallets.Where(w => w.PublicKey == updateWallet.PublicKey).First();
            databaseWallet.Lamports = updateWallet.Lamports;
            databaseWallet.SOL = updateWallet.SOL;
            databaseWallet.Tokens = updateWallet.Tokens;

            await _mmContext.SaveChangesAsync();
        }

        public async Task<bool> CheckWalletNotExistAsync(string privateKey)
        {
            return !(await _mmContext.Wallets.AnyAsync(w => w.PrivateKey.Equals(privateKey)));
        }

        public async Task DeleteAllWalletsAsync()
        {
            _mmContext.Wallets.RemoveRange(_mmContext.Wallets);
            await _mmContext.SaveChangesAsync();
        }

        public async Task<List<Wallet>> GetColdWalletsAsync()
        {
            return await _mmContext.Wallets
                .Include(w => w.Tokens)
                .Where(w => w.HotWallet == false)
                .ToListAsync();
        }

        public async Task<Wallet> GetHotWalletAsync()
        {
            return await _mmContext.Wallets
                .Include(w => w.Tokens)
                .Where(w => w.HotWallet == true)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Wallet>> GetWalletsAsync()
        {
            return await _mmContext.Wallets
                .Include(w => w.Tokens)
                .ToListAsync();
        }


    }
}
