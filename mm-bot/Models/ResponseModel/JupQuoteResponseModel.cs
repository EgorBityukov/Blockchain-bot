using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Models.ResponseModel
{
    public class JupQuoteResponseModel
    {
        public List<Datum> data { get; set; }
        public int timeTaken { get; set; }
        public int contextSlot { get; set; }
    }
    public class Datum
    {
        public string inAmount { get; set; }
        public string outAmount { get; set; }
        public int priceImpactPct { get; set; }
        public List<MarketInfo> marketInfos { get; set; }
        public string amount { get; set; }
        public int slippageBps { get; set; }
        public string otherAmountThreshold { get; set; }
        public string swapMode { get; set; }
        public Fees fees { get; set; }
    }

    public class Fees
    {
        public int signatureFee { get; set; }
        public List<int> openOrdersDeposits { get; set; }
        public List<int> ataDeposits { get; set; }
        public int totalFeeAndDeposits { get; set; }
        public int minimumSOLForTransaction { get; set; }
    }

    public class LpFee
    {
        public string amount { get; set; }
        public string mint { get; set; }
        public int pct { get; set; }
    }

    public class MarketInfo
    {
        public string id { get; set; }
        public string label { get; set; }
        public string inputMint { get; set; }
        public string outputMint { get; set; }
        public bool notEnoughLiquidity { get; set; }
        public string inAmount { get; set; }
        public string outAmount { get; set; }
        public string minInAmount { get; set; }
        public string minOutAmount { get; set; }
        public int priceImpactPct { get; set; }
        public LpFee lpFee { get; set; }
        public PlatformFee platformFee { get; set; }
    }

    public class PlatformFee
    {
        public string amount { get; set; }
        public string mint { get; set; }
        public int pct { get; set; }
    }
}
