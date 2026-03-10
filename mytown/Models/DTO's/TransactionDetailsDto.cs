namespace mytown.Models.DTO_s
{
    public class TransactionDetailsDto
    {
        public int TransactionId { get; set; }
        public int OrderId { get; set; }
        public int ShopperId { get; set; }
        public string ShopperName { get; set; }
        public decimal TotalPayment { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
    }
}
