using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Models
{
    public class WalletModel
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public long Lamports { get; set; }
        public decimal SOL { get; set; }
        public decimal ApproximateMintPrice { get; set; }
        public List<TokenModel> Tokens { get; set; }
        public bool HotWallet { get; set; }
    }
}
