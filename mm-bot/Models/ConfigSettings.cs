using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Models
{
    public class ConfigSettings
    {
        public MainWalletInfo HotWallet { get; set; }
        public List<MainWalletInfo> ColdWallet { get; set; }
        public string MainExchangeToken { get; set; }
        public decimal BaseVolumeSOLperColdWallet { get; set; }
        public decimal BaseVolumeUSDCperColdWallet { get; set; }
        public decimal DailyTradingVolumeInUSDCperXtoken { get; set; }
        public decimal PercentageOfRandomTransactionsForAddToken { get; set; }
        public int MinimumDelayInSecondsForOneTransactionPerWallet { get; set; }
        public bool AutomaticGenerationOfWallets { get; set; }
        public int ColdWalletsCount { get; set; }
        public string USDCmint { get; set; }
        public string XTokenMint { get; set; }
    }
}
