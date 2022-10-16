
namespace mmTransactionDB.Models
{
    public class Wallet
    {
        public Guid Id { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public double Lamports { get; set; }
        public double SOL { get; set; }
        public double ApproximateMintPrice { get; set; }
        public double Tokens { get; set; }
        public bool HotWallet { get; set; }
    }
}
