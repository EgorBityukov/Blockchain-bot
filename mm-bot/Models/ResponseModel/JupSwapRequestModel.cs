using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Models.ResponseModel
{
    public class JupSwapRequestModel
    {
        public Datum route { get; set; }
        public string userPublicKey { get; set; }
        public bool wrapUnwrapSOL { get; set; }
        public string feeAccount { get; set; }
        public string destinationWallet { get; set; }
    }
}
