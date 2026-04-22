namespace mytown.Models.DTO_s
{
    public class CourierOrderDetailDto
    {
        // Order Info
        public int StoreOrderId { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        // Shopper Info
        public int ShopperId { get; set; }
        public string ShopperName { get; set; }
        public string ShopperPhone { get; set; }

        // Store Info
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreTown { get; set; }

        public string StoreAddress { get; set; }

        // Shipping Info
        public string ShippingMethod { get; set; }
        public decimal ShippingCost { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingStatus { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string CourierServiceName { get; set; }
        public string TrackingId { get; set; }

        public decimal? PackageLength { get; set; }
        public decimal? PackageWidth { get; set; }
        public decimal? PackageHeight { get; set; }
        public decimal? PackageWeight { get; set; }

        public string DimensionUnit { get; set; }
        public string WeightUnit { get; set; }

        // Products (nested list)
        public List<CourierOrderProductDto> Products { get; set; }

        // Amounts
        public decimal TotalProductAmount { get; set; }
        public decimal TotalShippingAmount { get; set; }
        public decimal FinalTotalAmount { get; set; }

    }

    public class CourierOrderProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int VariantId { get; set; }
        public string VariantImage { get; set; }
        public decimal VariantCost { get; set; }
        public int Quantity { get; set; }

        // Weight & Dimensions
        public decimal? Weight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
    }

}
