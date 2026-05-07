namespace mytown.DTOs
{
    public class SenderOrderSummaryDto
    {
        // Sender
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string PickupAddress { get; set; }

        // Receiver
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverAddress { get; set; }

        // Product
        public string ProductName { get; set; }
        public decimal ProductCost { get; set; }
        public decimal? PackageWeight { get; set; }

        public bool IsFragile { get; set; }
        public bool IsPerishable { get; set; }

        // Transporter
        public string TransporterName { get; set; }
        public string TransporterEmail { get; set; }
        public string TransporterPhone { get; set; }

        public string VehicleType { get; set; }
        public string VehicleName { get; set; }

        // Charges
        public decimal BaseAmount { get; set; }
        public decimal GstAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}