namespace mytown.Models.DTO_s
{
    public class RazorpayVerifyDto
    {
        public int OrderId { get; set; }
        public string RazorpayOrderId { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpaySignature { get; set; } = string.Empty;
    }
}
