namespace mytown.DTOs
{
    public class ConfirmSenderPaymentDto
    {
        public int SenderOrderId { get; set; }

        public string StripePaymentIntentId { get; set; }

        public string PaymentMethod { get; set; }
public int TransporterRegId { get; set; }
public int TransporterPlanId { get; set; }
    }
}