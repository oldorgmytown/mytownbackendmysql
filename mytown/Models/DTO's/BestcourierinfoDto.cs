namespace mytown.Models.DTO_s
{
    public class BestcourierinfoDto
    {
        public int BranchId { get; set; }
        public string ShippingMode { get; set; }
        public decimal Cost { get; set; }

        public int MaxDeliveryDays { get; set; }
        public string DeliveryDaysRange { get; set; }
        public string EstimatedDeliveryDate { get; set; }

        // ✅ NEW — only filled when ShippingMode == "P2P"
        public int? TransporterRegId { get; set; }
        public int? TransporterPlanId { get; set; }
        public string TransporterName { get; set; }   // show in UI "Going with: Ravi Kumar"
        public string VehicleType { get; set; }       // show in UI "Car / Train / Bus"
    }
}