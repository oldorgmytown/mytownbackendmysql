namespace mytown.Models.DTO_s
{
    public class HubStoreOrderListDto
    {
        public int StoreOrderId { get; set; }
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }        // New / In Progress / Completed

        public int TransporterRegId { get; set; }
        public string TransporterName { get; set; }

        public DateTime? PickupDate { get; set; }      // from transporter travel plan

        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreLocation { get; set; }      // Town, City

        public string? PackageSpecs { get; set; }      // "10 × 10 × 5 cm"
        public string? HubStatus { get; set; }         // later

            
        public decimal? PackageWeight { get; set; }
        public string? WeightUnit { get; set; }
    }
}
