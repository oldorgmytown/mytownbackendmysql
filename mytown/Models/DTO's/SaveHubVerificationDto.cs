namespace mytown.Models.DTO_s
{
    public class SaveHubVerificationDto
    {
        public int HubId { get; set; }
        public bool PackageVerified { get; set; }
        public bool SecurityCheck { get; set; }
        public bool TravelPlanVerified { get; set; }
        public bool TransporterVerified { get; set; }
        public bool PackageHandedOver { get; set; }
        public string? Remarks { get; set; }
    }
}
