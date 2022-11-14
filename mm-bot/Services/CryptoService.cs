using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using mm_bot.Models;
using mm_bot.Models.ResponseModels;
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
        private readonly IOptions<ConfigSettings> _options;

        public CryptoService(IHttpClientFactory httpClientFactory,
                             ILogger<Worker> logger,
                             IOptions<ConfigSettings> options)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient("CryptoClient");
            _logger = logger;
            _options = options;
        }

        public async Task<JObject> CreateWalletAsync()
        {
            HttpResponseMessage response = await _httpClient.PostAsync("wallets", null);
            string responseBody = await response.Content.ReadAsStringAsync();

            JObject walletResponce = JObject.Parse(responseBody);

            if (walletResponce.Value<string>("status").Equals("error"))
            {
                _logger.LogError("CryptoService - Create Wallet Http Request Exception: {0}", (string)walletResponce.GetValue("error"));
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
                _logger.LogError("CryptoService - GetInfoAboutWalletAsync Http Request Exception: {0}", (string)walletInfoResponce.GetValue("error"));
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
            _httpClient.DefaultRequestHeaders.Add("X-Fee-Payer", privateKey);

            var parameters = new Dictionary<string, string>()
            {
                ["to"] = toPublicKey,
                ["sol"] = sol.ToString()
            };

            var requestUrl = QueryHelpers.AddQueryString($"wallets/send", parameters);

            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                _logger.LogError("TransferSolToAnotherWalletAsync response: {0}", responseBody);
                int k = 0;

                while (response.StatusCode != System.Net.HttpStatusCode.OK && k < 4)
                {
                    await Task.Delay(4000);
                    k++;
                    response = await _httpClient.GetAsync(requestUrl);
                    responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("TransferSolToAnotherWalletAsync response: {0}", responseBody);
                }
            }
            else if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError("TransferSolToAnotherWalletAsync response: {0}", responseBody);
                int k = 0;

                while (response.StatusCode != System.Net.HttpStatusCode.OK && k < 3)
                {
                    await Task.Delay(120000);
                    k++;
                    response = await _httpClient.GetAsync(requestUrl);
                    responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("TransferSolToAnotherWalletAsync response: {0}", responseBody);
                }
            }

            _httpClient.DefaultRequestHeaders.Remove("x-auth-token");
            _httpClient.DefaultRequestHeaders.Remove("X-Fee-Payer");

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

        public async Task<string> TransferTokenToAnotherWalletAsync(string privateKey, string mint, string toPublicKey, decimal count, bool isForbiddenToCloseAccount = true)
        {
            _httpClient.DefaultRequestHeaders.Add("x-auth-token", privateKey);
            _httpClient.DefaultRequestHeaders.Add("X-Fee-Payer", privateKey);

            var parameters = new Dictionary<string, string>()
            {
                ["to"] = toPublicKey,
            };

            if(mint.Equals(_options.Value.XTokenMint))
            {
                long countLong = 0;
                countLong = (long)(count * 10000);
                parameters.Add("count", countLong.ToString());
            }
            else
            {
                parameters.Add("count", count.ToString());
            }
            

            if (isForbiddenToCloseAccount)
            {
                parameters.Add("isForbiddenToCloseAccount", "true");
            }
            else
            {
                parameters.Add("isForbiddenToCloseAccount", "false");
            }

            var requestUrl = QueryHelpers.AddQueryString($"nft/token/{mint}/transfer", parameters);

            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                _logger.LogError("TransferTokenToAnotherWalletAsync response: {0}", responseBody);
                int k = 0;

                while (response.StatusCode != System.Net.HttpStatusCode.OK && k < 4)
                {
                    await Task.Delay(4000);
                    k++;
                    response = await _httpClient.GetAsync(requestUrl);
                    responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("TransferTokenToAnotherWalletAsync response: {0}", responseBody);
                }
            }
            else if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError("TransferTokenToAnotherWalletAsync response: {0}", responseBody);
                int k = 0;

                while (response.StatusCode != System.Net.HttpStatusCode.OK && k < 3)
                {
                    await Task.Delay(120000);
                    k++;
                    response = await _httpClient.GetAsync(requestUrl);
                    responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("TransferTokenToAnotherWalletAsync response: {0}", responseBody);
                }
            }   

            _httpClient.DefaultRequestHeaders.Remove("x-auth-token");
            _httpClient.DefaultRequestHeaders.Remove("X-Fee-Payer");

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
            string responseBody = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                _logger.LogError("GetInfoAboutTransactionAsync response: {0}", responseBody);
                int k = 0;

                while (response.StatusCode != System.Net.HttpStatusCode.OK && k < 4)
                {
                    await Task.Delay(4000);
                    k++;
                    response = await _httpClient.GetAsync(requestUrl);
                    responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetInfoAboutTransactionAsync response: {0}", responseBody);
                }
            }
            else if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError("GetInfoAboutTransactionAsync response: {0}", responseBody);
                int k = 0;

                while (response.StatusCode != System.Net.HttpStatusCode.OK && k < 2)
                {
                    await Task.Delay(120000);
                    k++;
                    response = await _httpClient.GetAsync(requestUrl);
                    responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetInfoAboutTransactionAsync response: {0}", responseBody);
                }
            }

            JObject transactionInfoResponce = JObject.Parse(responseBody);

            int i = 0;

            while (transactionInfoResponce.GetValue("result").ToString().Equals("") && i < 2)
            {
                i++;
                response = await _httpClient.GetAsync(requestUrl);
                responseBody = await response.Content.ReadAsStringAsync();
                transactionInfoResponce = JObject.Parse(responseBody);
                await Task.Delay(4000);
            }

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
            _httpClient.DefaultRequestHeaders.Add("X-Fee-Payer", privateKey);

            var parameters = new Dictionary<string, string>()
            {
                ["transaction"] = txid
            };

            var encodedContent = new FormUrlEncodedContent(parameters);

            HttpResponseMessage response = await _httpClient.PostAsync(requestUrl, encodedContent);

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                int k = 0;

                while (response.StatusCode != System.Net.HttpStatusCode.OK && k < 4)
                {
                    await Task.Delay(4000);
                    k++;
                    response = await _httpClient.PostAsync(requestUrl, encodedContent);
                }
            }
            else if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                int k = 0;

                while (response.StatusCode != System.Net.HttpStatusCode.OK && k < 3)
                {
                    await Task.Delay(120000);
                    k++;
                    response = await _httpClient.PostAsync(requestUrl, encodedContent);
                }
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            _httpClient.DefaultRequestHeaders.Remove("x-auth-token");
            _httpClient.DefaultRequestHeaders.Remove("X-Fee-Payer");

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
