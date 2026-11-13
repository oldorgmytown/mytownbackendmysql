namespace mytown.Models.DTO_s
{
    public class HubStoreVerificationDto
    {
        public int VerificationId { get; set; }
        public int StoreOrderId { get; set; }
        public int HubId { get; set; }
        public bool PackageVerified { get; set; }
        public bool SecurityCheck { get; set; }
        public bool TravelPlanVerified { get; set; }
        public bool TransporterVerified { get; set; }
        public bool PackageHandedOver { get; set; }
        public string? Remarks { get; set; }
        public string HubStatus { get; set; }      // New Intake / Handed Over
        public DateTime UpdatedAt { get; set; }
    }
}
