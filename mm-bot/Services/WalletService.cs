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

        public async Task<List<WalletModel>> GenerateWalletsAsync(int countWallets)
        {
            List<WalletModel> wallets = new List<WalletModel>();

            for (int i = 0; i < countWallets; i++)
            {
                var newWalletJson = await _cryptoService.CreateWalletAsync();

                WalletModel wallet = new WalletModel
                {
                    PublicKey = newWalletJson.Value<string>("public_key"),
                    PrivateKey = newWalletJson.Value<string>("private_key")
                };

                var walletInfo = await GetInfoAboutWalletAsync(wallet.PrivateKey);
                wallet.PublicKey = walletInfo.PublicKey;
                wallet.Lamports = walletInfo.Lamports;
                wallet.SOL = walletInfo.SOL;
                wallet.ApproximateMintPrice = walletInfo.ApproximateMintPrice;
                wallet.Tokens = walletInfo.Tokens;
                wallet.HotWallet = false;

                wallets.Add(wallet);
            }

            await _walletRepository.AddWalletListAsync(_mapper.Map<List<Wallet>>(wallets));

            return wallets;
        }

        public async Task DeleteAllWalletsAsync()
        {
            await _walletRepository.DeleteAllWalletsAsync();
        }

        public async Task<List<WalletModel>> GetWalletsAsync()
        {
            return _mapper.Map<List<WalletModel>>(await _walletRepository.GetWalletsAsync());
        }

        public async Task AddColdWalletsFromConfigAsync(List<MainWalletInfo> mainWalletInfos)
        {
            List<WalletModel> wallets = new List<WalletModel>();

            foreach (var mainWalletInfo in mainWalletInfos)
            {
                if (await _walletRepository.CheckWalletNotExistAsync(mainWalletInfo.PrivateKey))
                {
                    WalletModel wallet = new WalletModel
                    {
                        PrivateKey = mainWalletInfo.PrivateKey
                    };

                    var walletInfo = await GetInfoAboutWalletAsync(wallet.PrivateKey);
                    wallet.PublicKey = walletInfo.PublicKey;
                    wallet.Lamports = walletInfo.Lamports;
                    wallet.SOL = walletInfo.SOL;
                    wallet.ApproximateMintPrice = walletInfo.ApproximateMintPrice;
                    wallet.Tokens = walletInfo.Tokens;
                    wallet.HotWallet = false;

                    wallets.Add(wallet);
                }
            }

            if(wallets.Count > 0)
            {
                await _walletRepository.AddWalletListAsync(_mapper.Map<List<Wallet>>(wallets));
            } 
        }

        public async Task<WalletModel> GetInfoAboutWalletAsync(string privateKey)
        {
            WalletModel walletInfo = new WalletModel();

            var walletInfoJson = await _cryptoService.GetInfoAboutWalletAsync(privateKey);

            walletInfo.Lamports = walletInfoJson.Value<double>("lamports");
            walletInfo.SOL = walletInfoJson.Value<double>("sol");
            walletInfo.ApproximateMintPrice = walletInfoJson.Value<double>("approximate_mint_price");
            walletInfo.Tokens = walletInfoJson.Value<double>("tokens");
            walletInfo.PublicKey = walletInfoJson.Value<string>("public_key");

            return walletInfo;
        }

        public async Task AddHotWalletFromConfigAsync(MainWalletInfo mainWalletInfo)
        {
            if (await _walletRepository.CheckWalletNotExistAsync(mainWalletInfo.PrivateKey))
            {
                WalletModel hotWallet = new WalletModel
                {
                    PrivateKey = mainWalletInfo.PrivateKey
                };

                var walletInfo = await GetInfoAboutWalletAsync(hotWallet.PrivateKey);
                hotWallet.PublicKey = walletInfo.PublicKey;
                hotWallet.Lamports = walletInfo.Lamports;
                hotWallet.SOL = walletInfo.SOL;
                hotWallet.ApproximateMintPrice = walletInfo.ApproximateMintPrice;
                hotWallet.Tokens = walletInfo.Tokens;
                hotWallet.HotWallet = true;

                await _walletRepository.AddWalletAsync(_mapper.Map<Wallet>(hotWallet));
            }
        }
    }
}
