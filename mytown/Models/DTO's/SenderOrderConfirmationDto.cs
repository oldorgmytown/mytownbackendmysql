namespace mytown.Models.DTO_s
{
    public class SenderOrderConfirmationDto
    {
        // Order
        public int SenderOrderId { get; set; }
        public DateTime BookingDate { get; set; }

        // Package
        public string ProductName { get; set; }
        public string PackageType { get; set; }
        public string Dimensions { get; set; }
        public string Weight { get; set; }
        public decimal DeclaredValue { get; set; }

        // Pickup
        public string PickupAddress { get; set; }

        public string PickupTown { get; set; }
        public string PickupCity { get; set; }
        public string PickupState { get; set; }
        public string PickupCountry { get; set; }

        public DateTime PickupDate { get; set; }
        public string PickupTime { get; set; }

        // Receiver
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string DeliveryAddress { get; set; }

        // Delivery
        public DateTime EstimatedDeliveryDate { get; set; }

        // Transporter
        public string TransporterName { get; set; }
        public string TransporterPhone { get; set; }
        public string VehicleType { get; set; }

        // Payment
        public decimal TransportationCharge { get; set; }
        public string PaymentMethod { get; set; }

        // Sender info (needed for transporter email)
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }

        public HubAddressDto? HubAddress { get; set; }
    }
}