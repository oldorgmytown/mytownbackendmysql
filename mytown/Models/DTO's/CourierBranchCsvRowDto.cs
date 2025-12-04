namespace mytown.Models.DTO_s
{
    public class CourierBranchCsvRowDto
    {
        public int RowNumber { get; set; }
        public string CourierServiceName { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string BranchAddress { get; set; }
        public string BranchPhoneNumber { get; set; }
        public string BranchEmailId { get; set; }
        public string BranchContactPerson { get; set; }
        public string Destinations { get; set; }
        public string ShippingMode { get; set; }
        public string DistanceRange { get; set; }
        public string WeightRange { get; set; }
        public decimal Charges { get; set; }

        public bool IsValid { get; set; }
    }

}
