using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Models
{
    public class TokenModel
    {
        public Guid IdToken { get; set; }
        public string PublicKey { get; set; }
        public string Mint { get; set; }
        public string OwnerId { get; set; }
        public WalletModel Owner { get; set; }
        public string Amount { 
            get { return Amount; }
            set { 
                Amount = value;
                AmountDouble = Convert.ToDouble(Amount) / 1000000;
            } 
        }
        public double AmountDouble { get; set; }
    }
}
