namespace mytown.Models.DTO_s
{
    public class CreatePostDto
    {
        public int ShopperId { get; set; }
        public string Caption { get; set; } = "";
        public int? BusRegId { get; set; }                     // tagged business, optional
        public List<string> ImageUrls { get; set; } = new();   // max 2
    }
}
