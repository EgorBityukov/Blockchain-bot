namespace mmTransactionDB.Models
{
    public class mmTransaction
    {
        public string Id { get; set; }
        public string txId { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string OperationType { get; set; }
        public string WalletAddress { get; set; }
        public string SendTokenMint { get; set; }
        public double SendTokenCount { get; set; }
        public string RecieveTokenMint { get; set; }
        public double RecieveTokenCount { get; set; }
        public double BalanceXToken { get; set; }
        public double BalanceUSDCToken { get; set; }
    }
}
