using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using mm_bot.Models;
using mm_bot.Models.ResponseModels;
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
        private readonly IOptions<ConfigSettings> _options;

        public JupService(IHttpClientFactory httpClientFactory,
                          ILogger<Worker> logger,
                          IOptions<ConfigSettings> options)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient("JupClient");
            _logger = logger;
            _options = options;
        }

        public async Task<JupQuoteResponseModel> GetQuoteAsync(string inputMint, string outputMint, decimal amount)
        {
            var parameters = new Dictionary<string, string>()
            {
                ["inputMint"] = inputMint,
                ["outputMint"] = outputMint
            };

            if (inputMint.Equals(_options.Value.XTokenMint))
            {
                parameters.Add("amount", ((long)(amount * 10000m)).ToString());
            }
            else
            {
                parameters.Add("amount", ((long)(amount * 1000000m)).ToString());
            }

            var requestUrl = QueryHelpers.AddQueryString("quote", parameters);

            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

            string responseBody = await response.Content.ReadAsStringAsync();

            JObject quoteResponce = JObject.Parse(responseBody);

            if (quoteResponce.ContainsKey("error"))
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

        public async Task<List<string>> GetMintsAsync()
        {
            string requestUrl = "indexed-route-map";

            var parameters = new Dictionary<string, string>()
            {
                ["onlyDirectRoutes"] = "true"
            };

            var encodedContent = new FormUrlEncodedContent(parameters);

            HttpResponseMessage response = await _httpClient.PostAsync(requestUrl, encodedContent);

            string responseBody = await response.Content.ReadAsStringAsync();

            JObject mintsResponce = JObject.Parse(responseBody);

            if (mintsResponce.Children().Contains("error"))
            {
                _logger.LogError("JupService - Get Quote Http Request Exception /n" +
                    "Message: {1}", mintsResponce.GetValue("message"));
                return null;
            }
            else
            {
                List<string> mints = mintsResponce.GetValue("mintKeys").ToObject<List<string>>();
                return mints;
            }
        }
    }
}
