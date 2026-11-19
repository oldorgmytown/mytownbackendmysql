namespace mytown.Models.DTO_s
{
    public class FeedPostDto
    {

        public int PostId { get; set; }
        public string Caption { get; set; } = "";
        public DateTime CreatedAt { get; set; }          // the frontend shows "45m ago" from this

    
        public int ShopperId { get; set; }
        public string Username { get; set; } = "";
        public string PhotoName { get; set; } = "";
        public string Town { get; set; } = "";
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public bool IsVerified { get; set; }
        public bool IsMine { get; set; }                 // show Edit and Delete only when true

        // Tagged business (click it to open the business profile)
        public int? BusRegId { get; set; }
        public string? BusinessName { get; set; }

        public List<string> ImageUrls { get; set; } = new();
        public int LikeCount { get; set; }
        public bool LikedByMe { get; set; }
    }
}
