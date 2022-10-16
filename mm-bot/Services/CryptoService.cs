using mm_bot.Services.Interfaces;
using Newtonsoft.Json.Linq;

namespace mm_bot.Services
{
    public class CryptoService : ICryptoService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient _httpClient;

        public CryptoService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient("CryptoClient");
        }

        public async Task<JObject> CreateWalletAsync()
        {
            HttpResponseMessage response = (await _httpClient.PostAsync("wallets", null)).EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            JObject walletResponce = JObject.Parse(responseBody);

            if (walletResponce.Value<string>("status").Equals("error"))
            {
                //_logger.LogError("CryptoService - Create Wallet Http Request Exception: {0}", walletResponce.GetValue("error"));
                throw new HttpRequestException();
            }
            else
            {
                return (JObject)walletResponce.GetValue("data");
            }
        }

        public async Task<JObject> GetInfoAboutWalletAsync(string privateKey)
        {
            _httpClient.DefaultRequestHeaders.Add("x-auth-token", privateKey);
            HttpResponseMessage response = (await _httpClient.GetAsync("wallets")).EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            JObject walletInfoResponce = JObject.Parse(responseBody);

            if (walletInfoResponce.Value<string>("status").Equals("error"))
            {
                //_logger.LogError("CryptoService - Create Wallet Http Request Exception: {0}", walletInfoResponce.GetValue("error"));
                throw new HttpRequestException();
            }
            else
            {
                return (JObject)walletInfoResponce.GetValue("data");
            }
        }
    }
}
