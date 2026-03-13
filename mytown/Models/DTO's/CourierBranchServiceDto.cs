namespace mytown.Models.DTO_s
{
    public class CourierBranchServiceDto
    {
        public int BranchServiceId { get; set; }

        public string Destinations { get; set; }

        public string ShippingMode { get; set; }

        public string DistanceRange { get; set; }

        public string WeightRange { get; set; }

        public decimal Charges { get; set; }

        public int? EstimateDays { get; set; }
    
}
}
