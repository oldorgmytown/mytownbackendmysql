namespace mytown.Models.DTO_s
{
    public class ShopperAlternateAddressDto
    {
        public int AltAddressId { get; set; }
        public int ShopperRegId { get; set; }
        public string AltName { get; set; }
        public string AltPhoneNumber { get; set; }
        public string AltAddress { get; set; }
        public string AltTown { get; set; }
        public string AltCity { get; set; }
        public string AltState { get; set; }
        public string AltCountry { get; set; }
        public string AltPostalCode { get; set; }
        public string DeliveryNotes { get; set; }
    }
}
