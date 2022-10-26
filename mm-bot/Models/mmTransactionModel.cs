using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Models
{
    public class mmTransactionModel
    {
        public string txId { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string OperationType { get; set; }
        public string SendWalletAddress { get; set; }
        public string SendTokenMint { get; set; }
        public decimal SendTokenCount { get; set; }
        public string RecieveWalletAddress { get; set; }
        public string RecieveTokenMint { get; set; }
        public decimal RecieveTokenCount { get; set; }
        public decimal BalanceXToken { get; set; }
        public decimal BalanceUSDCToken { get; set; }
    }
}
