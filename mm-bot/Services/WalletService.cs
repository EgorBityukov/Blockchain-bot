using mm_bot.Services.Interfaces;
using mm_bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mmTransactionDB.Repository.Interfaces;
using AutoMapper;
using mmTransactionDB.Models;

namespace mm_bot.Services
{
    public class WalletService : IWalletService
    {
        private readonly ICryptoService _cryptoService;
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;

        public WalletService(ICryptoService cryptoService,
                             IWalletRepository walletRepository,
                             IMapper mapper)
        {
            _cryptoService = cryptoService;
            _walletRepository = walletRepository;
            _mapper = mapper;
        }

        public async Task MonitoringSolBalanceAsync(CancellationTokenSource cancellationTokenSourceTransactions, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var hotWallet = await GetHotWalletAsync();
                await UpdateWalletInfoWithoutTokensAsync(hotWallet);

                if (hotWallet.SOL < 0.1m)
                {
                    cancellationTokenSourceTransactions.Cancel();
                }

                await Task.Delay(10800000, cancellationToken);
            }
        }

        public async Task<List<WalletModel>> GenerateWalletsAsync(int countWallets)
        {
            List<WalletModel> wallets = new List<WalletModel>();

            for (int i = 0; i < countWallets; i++)
            {
                var newWalletJson = await _cryptoService.CreateWalletAsync();

                var walletPrivateKey = newWalletJson.Value<string>("private_key");

                var newWallet = await GetInfoAboutWalletAsync(walletPrivateKey);
                newWallet.Tokens = new List<TokenModel>();
                newWallet.HotWallet = false;

                wallets.Add(newWallet);
            }

            await _walletRepository.AddWalletListAsync(_mapper.Map<List<Wallet>>(wallets));

            return wallets;
        }

        public async Task AddColdWalletsFromConfigAsync(List<MainWalletInfo> mainWalletInfos)
        {
            List<WalletModel> wallets = new List<WalletModel>();

            foreach (var mainWalletInfo in mainWalletInfos)
            {
                if (await _walletRepository.CheckWalletNotExistAsync(mainWalletInfo.PrivateKey))
                {
                    var coldWallet = await GetInfoAboutWalletAsync(mainWalletInfo.PrivateKey);
                    coldWallet.Tokens = await GetWalletTokensAsync(mainWalletInfo.PublicKey);
                    coldWallet.HotWallet = false;

                    wallets.Add(coldWallet);
                }
            }

            if (wallets.Count > 0)
            {
                await _walletRepository.AddWalletListAsync(_mapper.Map<List<Wallet>>(wallets));
            }
        }

        public async Task AddHotWalletFromConfigAsync(MainWalletInfo mainWalletInfo)
        {
            if (await _walletRepository.CheckWalletNotExistAsync(mainWalletInfo.PrivateKey))
            {
                var hotWallet = await GetInfoAboutWalletAsync(mainWalletInfo.PrivateKey);
                hotWallet.Tokens = await GetWalletTokensAsync(mainWalletInfo.PublicKey);
                hotWallet.HotWallet = true;

                await _walletRepository.AddWalletAsync(_mapper.Map<Wallet>(hotWallet));
            }
        }

        public async Task<WalletModel> GetInfoAboutWalletAsync(string privateKey)
        {
            WalletModel walletInfo = new WalletModel();

            var walletInfoJson = await _cryptoService.GetInfoAboutWalletAsync(privateKey);

            walletInfo.PrivateKey = privateKey;
            walletInfo.Lamports = walletInfoJson.Value<long>("lamports");
            walletInfo.SOL = walletInfoJson.Value<decimal>("sol");
            walletInfo.ApproximateMintPrice = walletInfoJson.Value<decimal>("approximate_mint_price");
            walletInfo.PublicKey = walletInfoJson.Value<string>("public_key");

            return walletInfo;
        }

        public async Task<List<TokenModel>> GetWalletTokensAsync(string publicKey)
        {
            List<TokenModel> tokens = new List<TokenModel>();
            var tokensResponce = await _cryptoService.GetWalletTokensAsync(publicKey);

            foreach (var token in tokensResponce)
            {
                    tokens.Add(new TokenModel
                    {
                        PublicKey = token.pubkey,
                        Mint = token.info.mint,
                        OwnerId = token.info.owner,
                        Amount = token.info.amount
                    });
            }

            return tokens;
        }

        public async Task UpdateWalletInfoWithoutTokensAsync(WalletModel wallet)
        {
            var updatedWallet = await GetInfoAboutWalletAsync(wallet.PrivateKey);
            wallet.Lamports = updatedWallet.Lamports;
            wallet.SOL = updatedWallet.SOL;
            await _walletRepository.UpdateWalletAsync(_mapper.Map<Wallet>(wallet));
        }

        public async Task UpdateWalletInfoWithTokensAsync(WalletModel wallet)
        {
            var updatedWallet = await GetInfoAboutWalletAsync(wallet.PrivateKey);
            wallet.Lamports = updatedWallet.Lamports;
            wallet.SOL = updatedWallet.SOL;
            wallet.Tokens = await GetWalletTokensAsync(wallet.PublicKey);
            await _walletRepository.UpdateWalletAsync(_mapper.Map<Wallet>(wallet));
        }

        public async Task UpdateHotWalletAsync(bool updateTokens)
        {
            var hotWallet = await GetHotWalletAsync();

            if (updateTokens)
            {
                await UpdateWalletInfoWithTokensAsync(hotWallet);
            }
            else
            {
                await UpdateWalletInfoWithoutTokensAsync(hotWallet);
            }
        }

        public async Task DeleteAllWalletsAsync()
        {
            await _walletRepository.DeleteAllWalletsAsync();
        }

        public async Task<List<WalletModel>> GetWalletsAsync()
        {
            return _mapper.Map<List<WalletModel>>(await _walletRepository.GetWalletsAsync());
        }

        public async Task<WalletModel> GetHotWalletAsync()
        {
            return _mapper.Map<WalletModel>(await _walletRepository.GetHotWalletAsync());
        }

        public async Task<List<WalletModel>> GetColdWalletsAsync()
        {
            return _mapper.Map<List<WalletModel>>(await _walletRepository.GetColdWalletsAsync());
        }
    }
}
