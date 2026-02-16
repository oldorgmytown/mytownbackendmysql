namespace mytown.Models.DTO_s
{
    public class CourierOrderDto
    {
        public int StoreOrderId { get; set; }

        public DateOnly Orderdate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }

        public string StoreName { get; set; }
        public string StoreTown { get; set; }
        public string StoreContact { get; set; }
        public string TrackingId { get; set; }
        public string ShippingStatus { get; set; }


    }
}
