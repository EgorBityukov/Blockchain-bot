using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Models
{
    public class mmTransactionModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string OperationType { get; set; }
        public string WallerAddress { get; set; }
        public double SendTokenCount { get; set; }
        public string RecieveTokenName { get; set; }
        public double RecieveTokenCount { get; set; }
        public double BalanceXToken { get; set; }
        public double BalanceUSDCToken { get; set; }
    }
}
