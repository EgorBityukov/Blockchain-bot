using mmTransactionDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmTransactionDB.Repository.Interfaces
{
    public interface IWalletRepository
    {
        public Task AddWalletAsync(Wallet walletModel);
        public Task AddWalletListAsync(List<Wallet> wallets);
    }
}
