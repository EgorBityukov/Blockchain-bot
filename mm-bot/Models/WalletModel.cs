using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Models
{
    public class WalletModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public double Lamports { get; set; }
        public double SOL { get; set; }
        public double ApproximateMintPrice { get; set; }
        public double Tokens { get; set; }
    }
}
