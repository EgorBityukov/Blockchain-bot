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
        public double ColdStorageBaseVolumeUSDC { get; set; }
        public double ColdStorageBaseVolumeSOL { get; set; }
        public string MainExchangeToken { get; set; }
        public double DailyTradingVolumeUSDC { get; set; }
        public double PercentageOfRandomTransactionsForAddToken { get; set; }
        public int MinimumDelayInSecondsForOneTransactionPerWallet { get; set; }
        public bool AutomaticGenerationOfWallets { get; set; }
        public int ColdWalletsCount { get; set; }
        public string USDCmint { get; set; }
    }
}
