namespace mytown.Models.DTO_s
{
    public class UpdateDefaultAddressDto
    {
        public int BusRegId { get; set; }
        public string? Town { get; set; }            // Street / Area
        public string? Address1 { get; set; }        // Address
        public string? BusinessCity { get; set; }
        public string? BusinessState { get; set; }
        public string? BusinessCountry { get; set; }
        public string? PostalCode { get; set; }
    }
}
