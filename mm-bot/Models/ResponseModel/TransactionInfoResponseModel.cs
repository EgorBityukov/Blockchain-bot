using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_bot.Models.ResponseModel
{
    public class TransactionInfoResponseModel
    {
        public Result result { get; set; }
    }
    public class Data
    {
        public string type { get; set; }
        public List<int> data { get; set; }
    }

    public class Instruction
    {
        public List<Key> keys { get; set; }
        public string programId { get; set; }
        public Data data { get; set; }
    }

    public class Key
    {
        public string pubkey { get; set; }
        public bool isSigner { get; set; }
        public bool isWritable { get; set; }
    }

    public class LoadedAddresses
    {
        public List<object> @readonly { get; set; }
        public List<object> writable { get; set; }
    }

    public class Meta
    {
        public object err { get; set; }
        public int fee { get; set; }
        public List<object> innerInstructions { get; set; }
        public LoadedAddresses loadedAddresses { get; set; }
        public List<string> logMessages { get; set; }
        public List<int> postBalances { get; set; }
        public List<PostTokenBalance> postTokenBalances { get; set; }
        public List<int> preBalances { get; set; }
        public List<PreTokenBalance> preTokenBalances { get; set; }
        public List<object> rewards { get; set; }
        public Status status { get; set; }
    }

    public class PostTokenBalance
    {
        public int accountIndex { get; set; }
        public string mint { get; set; }
        public string owner { get; set; }
        public string programId { get; set; }
        public UiTokenAmount uiTokenAmount { get; set; }
    }

    public class PreTokenBalance
    {
        public int accountIndex { get; set; }
        public string mint { get; set; }
        public string owner { get; set; }
        public string programId { get; set; }
        public UiTokenAmount uiTokenAmount { get; set; }
    }

    public class Result
    {
        public int blockTime { get; set; }
        public Meta meta { get; set; }
        public int slot { get; set; }
        public Transaction transaction { get; set; }
    }

    public class Signature
    {
        public Signature signature { get; set; }
        public string publicKey { get; set; }
    }

    public class Signature2
    {
        public string type { get; set; }
        public List<int> data { get; set; }
    }

    public class Status
    {
        public object Ok { get; set; }
        public Err Err { get; set; }

    }

    public class Err
    {
        public List<object> InstructionError { get; set; }
    }

    public class Transaction
    {
        public List<Signature> signatures { get; set; }
        public string feePayer { get; set; }
        public List<Instruction> instructions { get; set; }
        public string recentBlockhash { get; set; }
    }

    public class UiTokenAmount
    {
        public string amount { get; set; }
        public int decimals { get; set; }
        public double uiAmount { get; set; }
        public string uiAmountString { get; set; }
    }
}
