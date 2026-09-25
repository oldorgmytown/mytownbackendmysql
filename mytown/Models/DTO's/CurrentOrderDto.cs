namespace mytown.Models.DTO_s
{
    public class CurrentOrderDto
    {
        public int StoreOrderId { get; set; }
        public int OrderId { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public string ShippingStatus { get; set; }
        public string TrackingId { get; set; }
    }
}
