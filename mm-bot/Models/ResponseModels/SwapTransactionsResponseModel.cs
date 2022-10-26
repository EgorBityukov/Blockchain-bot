using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Models.ResponseModels
{
    public class SwapTransactionsResponseModel
    {
        public string setupTransaction { get; set; }
        public string swapTransaction { get; set; }
        public string cleanupTransaction { get; set; }
    }
}
