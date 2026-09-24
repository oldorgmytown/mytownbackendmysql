namespace mytown.Models.DTO_s
{
    public class ReadyToShipDto
    {

        public int StoreOrderId { get; set; }

        public decimal? PackageLength { get; set; }
        public decimal? PackageWidth { get; set; }
        public decimal? PackageHeight { get; set; }
        public decimal? PackageWeight { get; set; }

        public string? DimensionUnit { get; set; } = "cm";
        public string? WeightUnit { get; set; } = "kg";
    }
}
