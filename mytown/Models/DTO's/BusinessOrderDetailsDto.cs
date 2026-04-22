namespace mytown.Models.DTO_s
{
    public class BusinessOrderDetailsDto
{
    public int StoreOrderId { get; set; }
        public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
        public int TransactionId { get; set; }

    // Shopper info
    public int ShopperId { get; set; }
    public string ShopperName { get; set; }
    public string ShopperPhone { get; set; }

    // Store info
    public int StoreId { get; set; }
    public string StoreName { get; set; }
    public string StoreTown { get; set; }

    // Products
    public List<BusinessOrderProductDto> Products { get; set; } = new();

    public decimal ProductAmount { get; set; }

    // Shipping info
    public string ShippingMethod { get; set; }
    public string ShippingAddress { get; set; }
        public string ShippingStatus { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
    public string CourierService { get; set; }
    public string TrackingId { get; set; }

        // Optional buttons for frontend
        // Weight, Dimensions, Print Label, Print Invoice

        // Courier details (if courier handles shipping)
        public string CourierServiceName { get; set; }
        public string CourierBranchPhone { get; set; }
        public string CourierBranchContactname { get; set; }
        public string CourierEmail { get; set; }

        // Transporter details (if transporter handles shipping)
        public string TransporterName { get; set; }
        public string TransporterPhone { get; set; }
        public string TransporterEmail { get; set; }
    }

public class BusinessOrderProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }

    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }

        public decimal? Weight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }

        public string ProductImage { get; set; }
}

// Optional: Notification DTO in same file
public class BusinessNotificationDto
{
        internal string Title;

        public int NotificationId { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

    
}
