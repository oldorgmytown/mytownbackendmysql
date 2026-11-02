namespace mytown.Models.DTO_s
{
    public class AddProductReviewDto
    {
        public int ShopperRegId { get; set; }
        public long ProductId { get; set; }
        public string PostType { get; set; } = "Review";
        public decimal? Rating { get; set; }          // 1 to 5
        public string Title { get; set; }
        public string Review { get; set; }
        public bool IsAnonymous { get; set; }
        public List<string>? PhotoPaths { get; set; } // already-uploaded photo paths/URLs
    }
}
