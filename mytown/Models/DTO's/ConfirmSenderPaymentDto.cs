namespace mytown.DTOs
{
    public class ConfirmSenderPaymentDto
    {
        public int SenderOrderId { get; set; }

       public string RazorPayPaymentIntentId { get; set; } //Will storeRazor pay Id

      //  public string PaymentMethod { get; set; }
public int TransporterRegId { get; set; }
public int TransporterPlanId { get; set; }


        // NEW
        public string RazorpayOrderId { get; set; }
        public string RazorpaySignature { get; set; }
    }
}

public class SenderRazorpayOrderResponseDto
{
    public string RazorpayOrderId { get; set; }
    public long Amount { get; set; }      // in paise
    public string Currency { get; set; }
    public string KeyId { get; set; }     // frontend needs this to open checkout
}

