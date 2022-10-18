using mm_bot.Models.ResponseModel;
using mm_bot.Services.Interfaces;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace mm_bot.Services
{
    public class CryptoService : ICryptoService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        public CryptoService(IHttpClientFactory httpClientFactory,
                             ILogger<Worker> logger)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient("CryptoClient");
            _logger = logger;
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

            _httpClient.DefaultRequestHeaders.Remove("x-auth-token");

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
        //dodelat' ??????????????????????????????????????????????????
        public async Task<string> TransferLamportsToAnotherWallet(string privateKey, string toPublicKey, double lamports, double sol)
        {
            _httpClient.DefaultRequestHeaders.Add("x-auth-token", privateKey);
            HttpResponseMessage response = (await _httpClient.GetAsync("wallets/send")).EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            JObject walletInfoResponce = JObject.Parse(responseBody);

            if (walletInfoResponce.Value<string>("status").Equals("error"))
            {
                _logger.LogError("CryptoService - Create Wallet Http Request Exception: {0} /n" +
                    "Transaction Id: {1}", walletInfoResponce.GetValue("error"), walletInfoResponce.GetValue("txid"));
                return walletInfoResponce.GetValue("txid").ToString();
            }
            else
            {
                return walletInfoResponce.GetValue("txid").ToString();
            }
        }

        public async Task<List<WalletTokenResponseModel>> GetWalletTokensAsync(string publicKey)
        {
            HttpResponseMessage response = (await _httpClient.GetAsync($"wallets/{publicKey}/tokens")).EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            JObject tokenInfoResponce = JObject.Parse(responseBody);

            if (tokenInfoResponce.Value<string>("status").Equals("error"))
            {
                _logger.LogError("CryptoService - Create Wallet Http Request Exception: {0} /n" +
                    "Transaction Id: {1}", tokenInfoResponce.GetValue("error"), tokenInfoResponce.GetValue("txid"));
                return null;
            }
            else
            {
                List<WalletTokenResponseModel> tokens = tokenInfoResponce.GetValue("data").ToObject<List<WalletTokenResponseModel>>();
                return tokens;
            }
        }
    }
}
