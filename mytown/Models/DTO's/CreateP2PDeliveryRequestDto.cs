namespace mytown.Models.DTO_s
{
    public class CreateP2PDeliveryRequestDto
    {
        public int PlanId { get; set; }
        public int TransporterRegId { get; set; }
        public int? ShopperRegId { get; set; }

        public int? GuestRegId { get; set; }

        public bool IsGuestOrder { get; set; }
        public int OrderId { get; set; }
        public int StoreOrderId { get; set; }
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public decimal PackageWeightKg { get; set; }
        public int NumberOfPackages { get; set; }
        public decimal DeliveryFee { get; set; }
        public string PackageTags { get; set; }
    }
}