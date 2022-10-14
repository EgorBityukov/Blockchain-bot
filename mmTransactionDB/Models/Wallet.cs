using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmTransactionDB.Models
{
    public class Wallet
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public double Lamports { get; set; }
        public double SOL { get; set; }
        public double ApproximateMintPrice { get; set; }
    }
}
