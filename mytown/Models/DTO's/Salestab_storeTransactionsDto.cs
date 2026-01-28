namespace mytown.Models.DTO_s
{
    public class Salestab_storeTransactionsDto
    {
        public int TransactionId { get; set; }   // PaymentId
        public DateTime PaymentDate { get; set; }
        public int ShopperId { get; set; }
      //  public string ShopperName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
    }
}
