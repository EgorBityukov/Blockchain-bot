using Microsoft.Extensions.DependencyInjection;
using mmTransactionDB.DataAccess;
using mmTransactionDB.Models;
using mmTransactionDB.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
