namespace mytown.Models.DTO_s
{
    public class AdminSenderOrderDetailDto
    {
        // Header
        public int SenderOrderId { get; set; }
        public DateTime PickupDate { get; set; }
        public int? TransactionId { get; set; }              // SenderPaymentId
        public string? StripePaymentIntentId { get; set; }
        public string? DeliveryStatus { get; set; }
        public string? TrackingId { get; set; }

        // Sender
        public int SenderRegId { get; set; }
        public string? SenderName { get; set; }
        public string? SenderAddress { get; set; }
        public string? SenderPhone { get; set; }
        public string? SenderEmail { get; set; }

        // Pickup
        public string? PickupAddress { get; set; }
        public string? PickupTime { get; set; }

        // Receiver
        public string? ReceiverName { get; set; }
        public string? ReceiverPhone { get; set; }
        public string? ReceiverAddress { get; set; }

        // Product
        public string? ProductName { get; set; }
        public decimal ProductCost { get; set; }
        public decimal? PackageLength { get; set; }
        public decimal? PackageWidth { get; set; }
        public decimal? PackageHeight { get; set; }
        public decimal? PackageWeight { get; set; }
        public bool IsFragile { get; set; }
        public bool IsPerishable { get; set; }
        public string? SpecialInstructions { get; set; }

        // Transporter (null until assigned)
        public string? TransporterName { get; set; }
        public string? TransporterPhone { get; set; }
        public string? VehicleNumber { get; set; }
        public decimal? DeliveryCost { get; set; }
        public int? DeliveryDays { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public PayoutInfoDto? Payout { get; set; }
    }
}