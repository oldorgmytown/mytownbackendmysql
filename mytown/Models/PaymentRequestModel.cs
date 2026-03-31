namespace mytown.Models
{
    public class PaymentRequestModel
    {
        public int OrderId { get; set; }
      //  public decimal AmountPaid { get; set; }
        public string PaymentMethod { get; set; }
        public string StripePaymentIntentId { get; set; }
    }
}