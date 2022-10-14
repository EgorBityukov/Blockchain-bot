﻿using Microsoft.Extensions.DependencyInjection;
using mmTransactionDB.DataAccess;
using mmTransactionDB.Models;
using mmTransactionDB.Services.Interfaces;

namespace mmTransactionDB.Services
{
    public class mmTransactionRepository : ImmTransactionRepository
    {
        private readonly mmTransactionDBContext _mmContext;
        public mmTransactionRepository(IServiceScopeFactory factory)
        {
            _mmContext = factory.CreateScope().ServiceProvider.GetRequiredService<mmTransactionDBContext>();
        }

        public async Task AddTransaction(mmTransaction mmTran)
        {
            await _mmContext.mmTransactions.AddAsync(mmTran);
            await _mmContext.SaveChangesAsync();
        }
    }
}