using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmTransactionDB.Models
{
    public class Token
    {
        public Guid IdToken { get; set; }
        public string PublicKey { get; set; }
        public string Mint { get; set; }
        public string OwnerId { get; set; }
        public Wallet Owner { get; set; }
        public string Amount { get; set; }
        public double AmountDouble { get; set; }
    }
}
