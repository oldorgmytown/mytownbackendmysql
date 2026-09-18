namespace mytown.Models.DTO_s
{
    public class SenderOrdersTabDto
    {
        public int SenderOrderId { get; set; }

        public string ProductName { get; set; }

        public DateTime BookingDate { get; set; }

        public string PickupLocation { get; set; }

        public string DeliveryLocation { get; set; }

        public string DeliveryStatus { get; set; }

        public string OrderType { get; set; }

        public string? TransporterName { get; set; }

        public string? TransporterPhone { get; set; }

        public string? VehicleType { get; set; }
        public string? TrackingId { get; set; } 
    }
}