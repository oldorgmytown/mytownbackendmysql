namespace mytown.Models.DTO_s
{ 

    // Request payload: store id + shopper destination — no weight involved
    public class LocationCourierPricingRequestDto
    {
        public int BusRegId { get; set; }

        public string ShopperTown { get; set; }
        public string ShopperCity { get; set; }
        public string ShopperState { get; set; }
        public string ShopperCountry { get; set; }
    }

    // Combined response: location-matched courier pricing + optional transporter match
    public class LocationCourierPricingResponseDto
    {
        public List<BestcourierinfoDto> CourierOptions { get; set; } = new();

        public BestcourierinfoDto? TransporterOption { get; set; }

        public bool HasCourierOptions => CourierOptions.Count > 0;

        public bool HasTransporterOption => TransporterOption != null;
    }
}