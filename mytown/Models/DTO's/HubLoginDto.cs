namespace mytown.Models.DTO_s
{
    public class HubLoginDto
    {
        public int HubId { get; set; }
        public int HubAddressId { get; set; }
        public string HubName { get; set; }
        public string HubEmail { get; set; }
        public string? AddressLine { get; set; }
        public string? Town { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Pin { get; set; }
        public string? Phone { get; set; }
    }
}
