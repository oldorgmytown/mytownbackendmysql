namespace mytown.Models.DTO_s
{
    public class CreateShopperExperienceDto
    {
        public int ShopperRegId { get; set; }

        public int BusRegId { get; set; }

        public string PostType { get; set; }

        public decimal? Rating { get; set; }

        public string Title { get; set; }

        public string ExperienceText { get; set; }

        public List<string> PhotoUrls { get; set; } = new List<string>();

        public bool IsAnonymous { get; set; }
    }
}
