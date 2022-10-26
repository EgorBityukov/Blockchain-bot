using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Models
{
    public class TokenModel
    {
        public string PublicKey { get; set; }
        public string Mint { get; set; }
        public string OwnerId { get; set; }
        public WalletModel Owner { get; set; }
        public string Amount { 
            get { return amount; }
            set {
                amount = value;
                AmountDouble = Convert.ToDecimal(value) / 1000000;
            } 
        }
        private string amount;
        public decimal AmountDouble { get; set; }
    }
}
