namespace mytown.Models.DTO_s
{
    public class LatestPostDto
    {
        public int ShopperId { get; set; }
        public string Username { get; set; } = "";
        public int PostId { get; set; }
        public string Caption { get; set; } = "";
        public int? BusRegId { get; set; }
        public string? BusinessName { get; set; }
    }
}
