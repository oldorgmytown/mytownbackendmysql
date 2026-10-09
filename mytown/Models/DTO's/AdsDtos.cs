using System.Text.Json;

namespace mytown.Models.DTO_s
{
    public class AdsCreatePromotionDto
    {
        public int BusRegId { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
        public string? Title { get; set; }
        public JsonElement? Content { get; set; }
        public JsonElement? Audience { get; set; }
        public string? Thumbnail { get; set; }
        public string? MediaUrl { get; set; }
        public string? Placement { get; set; }
        public int DurationDays { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? OrderId { get; set; }
        public int? Id { get; set; }   // set when re-saving an existing draft
    }

    public class AdsPaymentOrderRequestDto
    {
        public int BusRegId { get; set; }
        public int DurationDays { get; set; }
    }

    public class AdsPaymentConfirmDto
    {
        public int BusRegId { get; set; }
        public int OrderId { get; set; }
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string RazorpayPaymentId { get; set; } = string.Empty;
        public string RazorpaySignature { get; set; } = string.Empty;
    }
    
    public class AdsStripeConfirmDto
    {
        public int BusRegId { get; set; }
        public int OrderId { get; set; }
        public string PaymentIntentId { get; set; } = string.Empty;
    }
}