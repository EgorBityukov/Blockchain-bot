using Microsoft.AspNetCore.WebUtilities;
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
            HttpResponseMessage response = await _httpClient.PostAsync("wallets", null);
            string responseBody = await response.Content.ReadAsStringAsync();

            JObject walletResponce = JObject.Parse(responseBody);

            if (walletResponce.Value<string>("status").Equals("error"))
            {
                _logger.LogError("CryptoService - Create Wallet Http Request Exception: {0}", walletResponce.GetValue("error"));
                return null;
            }
            else
            {
                return (JObject)walletResponce.GetValue("data");
            }
        }

        public async Task<JObject> GetInfoAboutWalletAsync(string privateKey)
        {
            _httpClient.DefaultRequestHeaders.Add("x-auth-token", privateKey);

            HttpResponseMessage response = await _httpClient.GetAsync("wallets");
            string responseBody = await response.Content.ReadAsStringAsync();

            _httpClient.DefaultRequestHeaders.Remove("x-auth-token");

            JObject walletInfoResponce = JObject.Parse(responseBody);

            if (walletInfoResponce.Value<string>("status").Equals("error"))
            {
                _logger.LogError("CryptoService - Create Wallet Http Request Exception: {0}", walletInfoResponce.GetValue("error"));
                return null;
            }
            else
            {
                return (JObject)walletInfoResponce.GetValue("data");
            }
        }
        public async Task<List<WalletTokenResponseModel>> GetWalletTokensAsync(string publicKey)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"wallets/{publicKey}/tokens");
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

        public async Task<string> TransferSolToAnotherWalletAsync(string privateKey, string toPublicKey, decimal sol)
        {
            _httpClient.DefaultRequestHeaders.Add("x-auth-token", privateKey);

            var parameters = new Dictionary<string, string>()
            {
                ["to"] = toPublicKey,
                ["sol"] = sol.ToString()
            };

            var requestUrl = QueryHelpers.AddQueryString($"wallets/send", parameters);

            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                int k = 0;

                while (response.StatusCode != System.Net.HttpStatusCode.OK || k == 4)
                {
                    await Task.Delay(4000);
                    k++;
                    response = await _httpClient.GetAsync(requestUrl);
                }
            }
            else if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                int k = 0;

                while (response.StatusCode != System.Net.HttpStatusCode.OK || k == 3)
                {
                    await Task.Delay(120000);
                    k++;
                    response = await _httpClient.GetAsync(requestUrl);
                }
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            _httpClient.DefaultRequestHeaders.Remove("x-auth-token");

            JObject transferLamportsResponce = JObject.Parse(responseBody);

            if (transferLamportsResponce.Value<string>("status").Equals("error"))
            {
                _logger.LogError("CryptoService - Transfer Lamports/Sol Http Request Exception: {0}",
                    transferLamportsResponce.GetValue("error"));
                return null;
            }
            else
            {
                return transferLamportsResponce.GetValue("data").Value<string>("txid");
            }
        }

        public async Task<string> TransferTokenToAnotherWalletAsync(string privateKey, string mint, string toPublicKey, decimal count)
        {
            _httpClient.DefaultRequestHeaders.Add("x-auth-token", privateKey);

            var parameters = new Dictionary<string, string>()
            {
                ["to"] = toPublicKey,
                ["count"] = count.ToString(),
                ["isForbiddenToCloseAccount"] = "true"
            };

            var requestUrl = QueryHelpers.AddQueryString($"nft/token/{mint}/transfer", parameters);

            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                int k = 0;

                while (response.StatusCode != System.Net.HttpStatusCode.OK || k == 4)
                {
                    await Task.Delay(4000);
                    k++;
                    response = await _httpClient.GetAsync(requestUrl);
                }
            }
            else if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                int k = 0;

                while (response.StatusCode != System.Net.HttpStatusCode.OK || k == 3)
                {
                    await Task.Delay(120000);
                    k++;
                    response = await _httpClient.GetAsync(requestUrl);
                }
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            _httpClient.DefaultRequestHeaders.Remove("x-auth-token");

            JObject transferTokenResponce = JObject.Parse(responseBody);

            if (transferTokenResponce.Value<string>("status").Equals("error"))
            {
                _logger.LogError("CryptoService - Tranfer Tokens Http Request Exception: {0}",
                    transferTokenResponce.GetValue("error"));
                return null;
            }
            else
            {
                return transferTokenResponce.GetValue("data").Value<string>("txid");
            }
        }

        public async Task<TransactionInfoResponseModel> GetInfoAboutTransactionAsync(string txid)
        {
            var requestUrl = $"transactions/{txid}/";

            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                int k = 0;

                while (response.StatusCode != System.Net.HttpStatusCode.OK || k == 4)
                {
                    await Task.Delay(4000);
                    k++;
                    response = await _httpClient.GetAsync(requestUrl);
                }
            }
            else if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                int k = 0;

                while (response.StatusCode != System.Net.HttpStatusCode.OK || k == 3)
                {
                    await Task.Delay(120000);
                    k++;
                    response = await _httpClient.GetAsync(requestUrl);
                }
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            JObject transactionInfoResponce = JObject.Parse(responseBody);

            if (transactionInfoResponce.ContainsKey("status"))
            {
                _logger.LogError("CryptoService - Get Transaction info Http Request Exception: {0} /n" +
                    "Transaction Id: {1}", transactionInfoResponce.GetValue("error"));
                return null;
            }
            else
            {
                TransactionInfoResponseModel transactionInfo = transactionInfoResponce.ToObject<TransactionInfoResponseModel>();
                return transactionInfo;
            }
        }

        public async Task<string> SignTransactionAsync(string privateKey, string txid)
        {
            string requestUrl = "transactions/sign";
            _httpClient.DefaultRequestHeaders.Add("x-auth-token", privateKey);

            var parameters = new Dictionary<string, string>()
            {
                ["transaction"] = txid
            };

            var encodedContent = new FormUrlEncodedContent(parameters);

            HttpResponseMessage response = await _httpClient.PostAsync(requestUrl, encodedContent);

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                int k = 0;

                while (response.StatusCode != System.Net.HttpStatusCode.OK || k == 4)
                {
                    await Task.Delay(4000);
                    k++;
                    response = await _httpClient.PostAsync(requestUrl, encodedContent);
                }
            }
            else if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                int k = 0;

                while (response.StatusCode != System.Net.HttpStatusCode.OK || k == 3)
                {
                    await Task.Delay(120000);
                    k++;
                    response = await _httpClient.PostAsync(requestUrl, encodedContent);
                }
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            _httpClient.DefaultRequestHeaders.Remove("x-auth-token");

            JObject txidResponce = JObject.Parse(responseBody);

            if (txidResponce.Value<string>("status").Equals("error"))
            {
                _logger.LogError("CryptoService - Sign Transaction Http Request Exception: {0}", txidResponce.GetValue("error"));
                return null;
            }
            else
            {
                return txidResponce.GetValue("data").Value<string>("txid");
            }
        }
    }
}
