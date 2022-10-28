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
    public class RaydiumService : IRaydiumService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        public RaydiumService(IHttpClientFactory httpClientFactory,
                              ILogger<Worker> logger)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient("RaydiumClient");
            _logger = logger;
        }

        public async Task<List<ExchangeRatesResponseModel>> GetExchangeRatesAsync()
        {
            var requestUrl = "main/pairs";

            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

            string responseBody = await response.Content.ReadAsStringAsync();

            JObject ratesResponce = JObject.Parse(responseBody);

            if (ratesResponce.ContainsKey("error"))
            {
                _logger.LogError("JupService - Get Quote Http Request Exception /n" +
                    "Message: {1}", ratesResponce.GetValue("message"));
                return null;
            }
            else
            {
                List<ExchangeRatesResponseModel> rates = ratesResponce.ToObject<List<ExchangeRatesResponseModel>>();
                return rates;
            }
        }
    }
}
