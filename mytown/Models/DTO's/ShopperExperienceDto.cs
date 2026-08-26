namespace mytown.Models.DTO_s
{
    public class ShopperExperienceDto
    {
        public int ShopperExperienceId { get; set; }

        public int ShopperRegId { get; set; }

        public string ShopperName { get; set; }

        public int BusRegId { get; set; }

        public string BusinessName { get; set; }

        public string PostType { get; set; }

        public decimal? Rating { get; set; }

        public string Title { get; set; }

        public string Experience { get; set; }

        public bool IsAnonymous { get; set; }

        public string Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public List<string> PhotoUrls { get; set; } = new();
    }
}