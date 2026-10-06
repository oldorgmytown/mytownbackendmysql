namespace mytown.Models.DTO_s
{
    public class HubStoreOrderDetailsDto
    {
        public int StoreOrderId { get; set; }
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string HubStatus { get; set; }

        public PackageDetailsDto? Package { get; set; }
        public BusinessDetailsDto Business { get; set; }
        public TransporterDetailsDto Transporter { get; set; }
        public TravelDetailsDto? Travel { get; set; }
        public ChecklistDto Checklist { get; set; }
    }

    public class PackageDetailsDto
    {
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string? DimensionUnit { get; set; }
        public decimal? Weight { get; set; }
        public string? WeightUnit { get; set; }
    }

    public class BusinessDetailsDto
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string Address { get; set; }
        public string? Phone { get; set; }
    }

    public class TransporterDetailsDto
    {
        public int TransporterRegId { get; set; }
        public string TransporterName { get; set; }
        public string Address { get; set; }
        public string? Phone { get; set; }
        public string? Status { get; set; }
    }

    public class TravelDetailsDto
    {
        // Start = assigned hub (from hubdetails)
        public string? StartLocationName { get; set; }      // e.g. "Jayanagar Hub"
        public string? StartLocationAddress { get; set; }   // e.g. "Bengaluru, Karnataka"

        // End + vehicle = from transporter travel plan
        public string? EndLocationName { get; set; }
        public string? EndLocationAddress { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EtaDate { get; set; }

        public string? VehicleName { get; set; }
        public string? VehicleNumber { get; set; }
    }
    public class ChecklistDto
    {
        public bool PackageVerified { get; set; }
        public bool SecurityCheck { get; set; }
        public bool TravelPlanVerified { get; set; }
        public bool TransporterVerified { get; set; }
        public bool PackageHandedOver { get; set; }
    }
}
