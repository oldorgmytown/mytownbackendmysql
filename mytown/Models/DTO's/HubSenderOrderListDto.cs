namespace mytown.Models.DTO_s
{
    public class HubSenderOrderListDto
    {
        public int SenderOrderId { get; set; }
        public string OrderStatus { get; set; }          // New / In Progress / Completed

        public int TransporterRegId { get; set; }
        public string TransporterName { get; set; }

        public DateTime? PickupDate { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }

        public int SenderRegId { get; set; }
        public string SenderName { get; set; }
        public string SenderLocation { get; set; }       // Pickup town, city

        public string? HubStatus { get; set; }           // later
    }
}
