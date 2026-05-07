namespace mytown.DTOs
{
    public class SenderOrderConfirmationDto
    {
        public int SenderOrderId { get; set; }

        public string ProductName { get; set; }

        public DateTime PickupDate { get; set; }

        public string PickupTime { get; set; }

        public DateTime EstimatedDeliveryDate { get; set; }

        public string TransporterName { get; set; }

        public string TransporterPhone { get; set; }
    }
}