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

            if (responseBody.Contains("error"))
            {
                _logger.LogError("JupService - Get Quote Http Request Exception /n" +
                    "Response body: {0}", responseBody);
                return null;
            }
            else
            {
                var ratesResponce = JArray.Parse(responseBody);
                List<ExchangeRatesResponseModel> rates = new List<ExchangeRatesResponseModel>();

                foreach (var rate in ratesResponce)
                {
                    rates.Add(rate.ToObject<ExchangeRatesResponseModel>());
                }

                return rates;
            }
        }
    }
}
