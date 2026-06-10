namespace mytown.Models.DTO_s
{
    public class TrackingResultDto
    {
        // Shipping Info
        public string TrackingId { get; set; }
        public string ShippingStatus { get; set; }
        public string ShippingType { get; set; }
        public int EstimatedDays { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string DeliveryAddress { get; set; }

        //  New - Expected delivery date
        public DateTime? ExpectedDeliveryDate { get; set; }

        // Order Info
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsGuestOrder { get; set; }

        //  New - Product details
        public string ProductName { get; set; }
        public string ProductImage { get; set; }

        // Customer Info
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
    }
}