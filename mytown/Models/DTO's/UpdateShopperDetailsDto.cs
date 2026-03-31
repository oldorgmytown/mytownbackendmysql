namespace mytown.Models.DTO_s
{
    public class UpdateShopperDetailsDto
    {
        public int ShopperRegId { get; set; }

        public string? Username { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
        public string? Town { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
    }
}
