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

        public async Task<List<WalletModel>> GenerateWallets(int countWallets)
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

                var walletInfoJson = await _cryptoService.GetInfoAboutWallet(wallet.PrivateKey);
                wallet.Lamports = walletInfoJson.Value<double>("lamports");
                wallet.SOL = walletInfoJson.Value<double>("sol");
                wallet.ApproximateMintPrice = walletInfoJson.Value<double>("approximate_mint_price");
                wallet.Tokens = walletInfoJson.Value<double>("tokens");

                wallets.Add(wallet);
            }

            await _walletRepository.AddWalletListAsync(_mapper.Map<List<Wallet>>(wallets));

            return wallets;
        }
    }
}
