namespace mytown.Models.DTO_s
{
    public class OrderConfirmationDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string DeliveryAddress { get; set; }

        public List<StoreOrderConfirmationDto> Stores { get; set; }
    }

    public class StoreOrderConfirmationDto
    {
        public int StoreOrderId { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }

        public string ShippingType { get; set; }
        public int EstimatedDays { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }

        public string ShippingStatus { get; set; }
    }

}
