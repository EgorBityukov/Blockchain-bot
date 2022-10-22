﻿using mm_bot.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Services.Interfaces
{
    public interface IJupService
    {
        public Task<JupQuoteResponseModel> GetQuoteAsync(string inputMint, string outputMint, string amount);
        public Task<SwapTransactionsResponseModel> GetSwapTransactionsAsync(JupSwapRequestModel requestContent);
    }
}
