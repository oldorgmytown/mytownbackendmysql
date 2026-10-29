namespace mytown.DTOs
{
    public class ShopperHeaderDto
    {
        public int ShopperId { get; set; }
        public string Username { get; set; } = "";
        public string PhotoName { get; set; } = "";
        public string Town { get; set; } = "";
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public string Country { get; set; } = "";
        public int ConnectedShoppers { get; set; }
        public int ConnectedBusiness{ get; set; }
    
}
}