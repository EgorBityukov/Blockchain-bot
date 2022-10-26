using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Models.ResponseModels
{
    public class WalletTokenResponseModel
    {
        public string pubkey { get; set; }
        public Account account { get; set; }
        public Info info { get; set; }
    }

    public class Account
    {
        public object data { get; set; }
        public bool executable { get; set; }
        public int lamports { get; set; }
        public string owner { get; set; }
        public int rentEpoch { get; set; }
    }

    public class Info
    {
        public string mint { get; set; }
        public string owner { get; set; }
        public string amount { get; set; }
        public int delegateOption { get; set; }
        public object @delegate { get; set; }
        public int state { get; set; }
        public int isNativeOption { get; set; }
        public bool isNative { get; set; }
        public string delegatedAmount { get; set; }
        public int closeAuthorityOption { get; set; }
        public object closeAuthority { get; set; }
        public bool isInitialized { get; set; }
        public bool isFrozen { get; set; }
        public object rentExemptReserve { get; set; }
    }
}
