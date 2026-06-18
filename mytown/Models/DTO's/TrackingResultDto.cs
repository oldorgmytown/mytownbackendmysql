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

        // Expected Delivery
        public DateTime? ExpectedDeliveryDate { get; set; }

        // Order Info
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsGuestOrder { get; set; }

        // Customer Info
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }

        // Store Details
        public int? StoreId { get; set; }
        public string? StoreName { get; set; }
        public string? StoreLogo { get; set; }
        public string? StoreBanner { get; set; }
        public string? StoreAddress { get; set; }
        public string? StorePhone { get; set; }
        public string? StoreEmail { get; set; }
        public string? StoreDescription { get; set; }

        // Transporter Details
        public int? TransporterRegId { get; set; }
        public string? TransporterName { get; set; }
        public string? TransporterPhone { get; set; }
        public string? TransporterEmail { get; set; }
        public string? TransporterAddress { get; set; }

        // Courier Details added
        public string? CourierName { get; set; }
        public string? BranchContactPerson { get; set; }
        public string? BranchEmail { get; set; }
        public string? BranchPhoneNumber { get; set; }


        // Travel Plan Details
        public string? VehicleType { get; set; }
        public string? VehicleName { get; set; }
        public string? PreferredRoute { get; set; }


        // Products
        public List<TrackingProductDto> Products { get; set; } = new();
    }

    public class TrackingProductDto
    {
        public int ProductId { get; set; }
        public int? SkuId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal ProductCost { get; set; }
    }
}