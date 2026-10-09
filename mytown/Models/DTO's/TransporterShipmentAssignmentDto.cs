namespace mytown.Models.DTO_s
{
    public class TransporterShipmentAssignmentDto
    {
        public int SenderOrderId { get; set; }

        public string ProductName { get; set; }

        public string SenderName { get; set; }

        public string SenderPhone { get; set; }

        public string PickupAddress { get; set; }

        public DateTime PickupDate { get; set; }

        public string PickupTime { get; set; }

        public DateTime DeliveryDate { get; set; }
    }
}