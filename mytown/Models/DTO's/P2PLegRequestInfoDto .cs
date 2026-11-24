namespace mytown.Models.DTO_s
{
    public class P2PLegRequestInfoDto
    {
        public int PlanId { get; set; }
        public int TransporterRegId { get; set; }
        public string PickupLocation { get; set; } = string.Empty;
        public string DropoffLocation { get; set; } = string.Empty;
        public decimal DeliveryFee { get; set; }
        public string LegText { get; set; } = string.Empty;
    }
}
