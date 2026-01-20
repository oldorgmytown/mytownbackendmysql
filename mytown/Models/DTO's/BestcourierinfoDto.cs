namespace mytown.Models.DTO_s
{
    public class BestcourierinfoDto
    {
        public int BranchId { get; set; }
        public string ShippingMode { get; set; }
        public decimal Cost { get; set; }
        public decimal MaxWeight { get; set; }
        public int MaxDistance { get; set; }

        // ✅ NEW
        public int MaxDeliveryDays { get; set; }
        public string DeliveryDaysRange { get; set; }
        public string EstimatedDeliveryDate { get; set; }
    }
}
