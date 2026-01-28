namespace mytown.Models.DTO_s
{
    public class UpdateShopperPasswordDto
    {
        public int ShopperRegId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
