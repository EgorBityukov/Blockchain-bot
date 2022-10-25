using mmTransactionDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmTransactionDB.Repository.Interfaces
{
    public interface ImmTransactionRepository
    {
        public Task AddTransaction(mmTransaction mmTran);
        public Task<List<mmTransaction>> GetTodayTransactionsAsync();
    }
}
