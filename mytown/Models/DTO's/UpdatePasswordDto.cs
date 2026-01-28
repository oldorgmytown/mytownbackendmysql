namespace mytown.Models.DTO_s
{
    public class UpdatePasswordDto
    {
        public int ShopperRegId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
