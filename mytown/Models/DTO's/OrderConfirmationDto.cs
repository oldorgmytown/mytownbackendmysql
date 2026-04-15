namespace mytown.Models.DTO_s
{
    public class OrderConfirmationDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public int ShopperRegId { get; set; }
        public string ShopperName { get; set; } = string.Empty;
        public string ShopperEmail { get; set; } = string.Empty;
        public string ShopperPhone { get; set; } = string.Empty;

        public string DeliveryAddress { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public int TransactionId { get; set; }

        //  Always initialize collections
        public List<StoreOrderConfirmationDto> Stores { get; set; } = new();
    }


    public class StoreOrderConfirmationDto
    {
        public int StoreOrderId { get; set; }
        public int StoreId { get; set; }

        public string StoreName { get; set; } = string.Empty;
        public string BusinessEmail { get; set; } = string.Empty;
        public string BusinessPhone { get; set; } = string.Empty;

        public string StoreAddress { get; set; } = string.Empty;

        //courier email
        public string CourierName { get; set; } = string.Empty;
        public string CourierEmail { get; set; } = string.Empty;
        public string CourierPhone { get; set;  } = string.Empty;

        // Transporter details

        public string TransporterName {  get; set; } = string.Empty;
        public string TransporterPhone { get; set; } = string.Empty;
        public string TransporterEmail { get; set; } = string.Empty;
        public string ShippingType { get; set; } = string.Empty;
        public decimal ShippingAmount { get; set; }

        public int EstimatedDays { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string ShippingStatus { get; set; } = string.Empty;

    

        //  Store-wise totals (important for business email)
        public decimal StoreItemsTotal { get; set; }

        // Computed property (no DB mapping needed)
        public decimal StoreTotal => StoreItemsTotal + ShippingAmount;

        // Items must belong to store
        public List<OrderItemDto> Items { get; set; } = new();
    }


    public class OrderItemDto
    {
        public string ProductName { get; set; } = string.Empty;

        public string Productdesc {  get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountAmount { get; set; }

        public string? ImageUrl { get; set; }

        // Optional but useful for emails/UI
        public decimal ItemTotal => Quantity * FinalPrice;
    }

  
}
