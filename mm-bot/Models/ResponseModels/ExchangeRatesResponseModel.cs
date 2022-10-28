using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Models.ResponseModels
{
    public class ExchangeRatesResponseModel
    {
        public string name { get; set; }
        public string ammId { get; set; }
        public string lpMint { get; set; }
        public string market { get; set; }
        public decimal liquidity { get; set; }
        public decimal volume24h { get; set; }
        public decimal volume24hQuote { get; set; }
        public decimal fee24h { get; set; }
        public decimal fee24hQuote { get; set; }
        public decimal volume7d { get; set; }
        public decimal volume7dQuote { get; set; }
        public decimal fee7d { get; set; }
        public decimal fee7dQuote { get; set; }
        public decimal volume30d { get; set; }
        public decimal volume30dQuote { get; set; }
        public decimal fee30d { get; set; }
        public decimal fee30dQuote { get; set; }
        public decimal? price { get; set; }
        public decimal lpPrice { get; set; }
        public decimal tokenAmountCoin { get; set; }
        public decimal tokenAmountPc { get; set; }
        public decimal tokenAmountLp { get; set; }
        public decimal apr24h { get; set; }
        public decimal apr7d { get; set; }
        public decimal apr30d { get; set; }
    }
}
