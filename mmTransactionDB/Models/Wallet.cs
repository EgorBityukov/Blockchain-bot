
namespace mmTransactionDB.Models
{
    public class Wallet
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public long Lamports { get; set; }
        public decimal SOL { get; set; }
        public decimal ApproximateMintPrice { get; set; }
        public List<Token> Tokens { get; set; }
        public bool HotWallet { get; set; }
    }
}
