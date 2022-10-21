using AutoMapper;
using mm_bot.Models;
using mmTransactionDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.MappingProfiles
{
    public class WalletProfile : Profile
    {
        public WalletProfile()
        {
            CreateMap<Wallet, WalletModel>()
                .ReverseMap();

            CreateMap<Token, TokenModel>()
                .ReverseMap();

            CreateMap<mmTransaction, mmTransactionModel>()
                .ReverseMap();
        }
    }
}
