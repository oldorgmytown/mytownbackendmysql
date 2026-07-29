namespace mytown.Models.DTO_s
{
    public class RazorpayOrderResponseDto
    {
        public string RazorpayOrderId { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string KeyId { get; set; }
    }
}
