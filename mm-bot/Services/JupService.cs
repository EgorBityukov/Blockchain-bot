using Microsoft.AspNetCore.WebUtilities;
using mm_bot.Models.ResponseModel;
using mm_bot.Services.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Services
{
    public class JupService : IJupService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        public JupService(IHttpClientFactory httpClientFactory,
                             ILogger<Worker> logger)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient("JupClient");
            _logger = logger;
        }

        public async Task<JupQuoteResponseModel> GetQuoteAsync(string inputMint, string outputMint, string amount)
        {
            var parameters = new Dictionary<string, string>()
            {
                ["inputMint"] = inputMint,
                ["outputMint"] = outputMint,
                ["amount"] = amount
            };

            var requestUrl = QueryHelpers.AddQueryString("quote", parameters);

            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

            string responseBody = await response.Content.ReadAsStringAsync();

            JObject quoteResponce = JObject.Parse(responseBody);

            if (quoteResponce.Children().Contains("error"))
            {
                _logger.LogError("JupService - Get Quote Http Request Exception /n" +
                    "Message: {1}", quoteResponce.GetValue("message"));
                return null;
            }
            else
            {
                JupQuoteResponseModel quotes = quoteResponce.ToObject<JupQuoteResponseModel>();
                return quotes;
            }
        }

        public async Task<SwapTransactionsResponseModel> GetSwapTransactionsAsync(JupSwapRequestModel requestContent)
        {
            var content = new StringContent(JObject.FromObject(requestContent).ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("swap", content);

            string responseBody = await response.Content.ReadAsStringAsync();

            JObject swapResponce = JObject.Parse(responseBody);

            if (swapResponce.Children().Contains("error"))
            {
                _logger.LogError("JupService - Get Quote Http Request Exception /n" +
                    "Message: {1}", swapResponce.GetValue("message"));
                return null;
            }
            else
            {
                SwapTransactionsResponseModel swapTransactions = swapResponce.ToObject<SwapTransactionsResponseModel>();
                return swapTransactions;
            }
        }
    }
}
