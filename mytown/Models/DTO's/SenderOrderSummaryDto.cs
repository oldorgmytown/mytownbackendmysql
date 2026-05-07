namespace mytown.DTOs
{
    public class SenderOrderSummaryDto
    {
        // Product
        public string ProductName { get; set; }
        public decimal ProductCost { get; set; }

        public decimal? PackageLength { get; set; }
        public decimal? PackageWidth { get; set; }
        public decimal? PackageHeight { get; set; }
        public decimal? PackageWeight { get; set; }

        // Sender
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string PickupAddress { get; set; }
        public DateTime PickupDate { get; set; }
        public string PickupTime { get; set; }

        // Receiver
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverAddress { get; set; }

        // Transporter
        public int TransporterRegId { get; set; }
        public string TransporterName { get; set; }
        public string TransporterPhone { get; set; }

        public string VehicleType { get; set; }
        public string StartLocation { get; set; }
        public string Destination { get; set; }

        // Pricing
        public decimal TransportCharge { get; set; }
        public decimal GstAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}