namespace mytown.Models.DTO_s
{
    public class ShopperOrderDetailsDto
    {
        public int StoreOrderId { get; set; }
        public int OrderId { get; set; }
        public int TransactionId { get; set; }
        public DateTime OrderDate { get; set; }

        public int ShopperId { get; set; }

        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreTown { get; set; }

        public List<OrderProductItemDto> Products { get; set; }

        public decimal ProductAmount { get; set; }
        public decimal CourierAmount { get; set; }
        public decimal TotalAmount => ProductAmount + CourierAmount;

        public string ShippingMethod { get; set; }
        public string ShippingAddress { get; set; }

        public DateTime ExpectedDeliveryDate { get; set; }
        public string CourierService { get; set; }
        public string TrackingId { get; set; }
    }
}
