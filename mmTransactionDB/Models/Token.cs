using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmTransactionDB.Models
{
    public class Token
    {
        public string PublicKey { get; set; }
        public string Mint { get; set; }
        public string OwnerId { get; set; }
        public Wallet Owner { get; set; }
        public string Amount { get; set; }
        public decimal AmountDouble { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is Token token) return PublicKey == token.PublicKey;
            return false;
        }
        public override int GetHashCode() => PublicKey.GetHashCode();
    }
}
