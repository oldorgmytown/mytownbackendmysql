namespace mytown.Models.DTO_s
{
    public class PayoutInfoDto
    {
        public string Status { get; set; } = "Pending";   // "Paid" if a payout row exists
        public decimal Amount { get; set; }               // store: StoreTotalAmount, courier/transporter: Cost
        public DateTime? SettledDate { get; set; }        // null while pending
        public string? CashfreeReferenceId { get; set; }
        public string? BeneficiaryId { get; set; }
        public string? BankName { get; set; }
        public string? AccountLast4 { get; set; }         // frontend shows "•••••••• 4092"
    }

    public class AdminOrderItemDto
    {
        public string? ProductName { get; set; }
        public long SkuId { get; set; }
        public decimal? Weight { get; set; }
        public string? MeasurementUnit { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class AdminOrderDetailDto
    {
        // Top header
        public int StoreOrderId { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public long? TransactionId { get; set; }
        public string? ShippingStatus { get; set; }

        // Business details
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public string? OwnerName { get; set; }
        public string? StoreAddress { get; set; }
        public string? StorePhone { get; set; }
        public PayoutInfoDto StorePayout { get; set; } = new();

        // Shipping details
        public string? ShippingMethod { get; set; }
        public string? ProviderName { get; set; }
        public string? ProviderPhone { get; set; }
        public string? VehicleNumber { get; set; }
        public PayoutInfoDto? ProviderPayout { get; set; }

        // Items and totals
        public List<AdminOrderItemDto> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }

        // Delivery details
        public string? ShopperName { get; set; }
        public bool IsGuestOrder { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? ShopperPhone { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string? TrackingId { get; set; }
    }
}