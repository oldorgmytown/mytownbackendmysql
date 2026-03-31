namespace mytown.Models.DTO_s
{
    public class CourierCompletedDeliveryDto
    {
        public int StoreOrderId { get; set; }
        public DateTime DeliveredDate { get; set; }
        public string TrackingId { get; set; }
    }
}
