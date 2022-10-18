using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Models
{
    public class WalletModel
    {
        public Guid IdWallet { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public double Lamports { get; set; }
        public double SOL { get; set; }
        public double ApproximateMintPrice { get; set; }
        public List<TokenModel> Tokens { get; set; }
        public bool HotWallet { get; set; }
    }
}
