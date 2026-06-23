namespace mytown.Models.DTO_s
{
    public class SenderOrderTrackingDto
    {
        public int SenderOrderId { get; set; }

        public string? TrackingId { get; set; }

        public string ProductName { get; set; }
        public decimal ProductCost { get; set; }

        public decimal? PackageLength { get; set; }
        public decimal? PackageWidth { get; set; }
        public decimal? PackageHeight { get; set; }
        public decimal? PackageWeight { get; set; }

        public bool IsFragile { get; set; }
        public bool IsPerishable { get; set; }

        public string? SpecialInstructions { get; set; }

        public string PickupAddress { get; set; }
        public string PickupTown { get; set; }
        public string PickupCity { get; set; }
        public string PickupState { get; set; }
        public string PickupCountry { get; set; }
        public string PickupPincode { get; set; }

        public DateTime PickupDate { get; set; }
        public string PickupTime { get; set; }

        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverTown { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverState { get; set; }
        public string ReceiverCountry { get; set; }
        public string ReceiverPincode { get; set; }

        public string OrderStatus { get; set; }
        public string DeliveryStatus { get; set; }

        public string? TransporterName { get; set; }
        public string? TransporterPhone { get; set; }
        public string? TransporterEmail { get; set; }

        public string? VehicleType { get; set; }
        public string? VehicleName { get; set; }
        public string? PreferredRoute { get; set; }
    }
}