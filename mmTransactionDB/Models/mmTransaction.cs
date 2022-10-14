namespace mmTransactionDB.Models
{
    public class mmTransaction
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
