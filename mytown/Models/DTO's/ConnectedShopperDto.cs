namespace mytown.Models.DTO_s
{
    public class ConnectedShopperDto
    {
        public int ShopperRegId { get; set; }

        public string ShopperName { get; set; }

        public string? ShopperPhoto { get; set; }

        public bool IsOnline { get; set; }
    }
}
