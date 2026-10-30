namespace mytown.Models.DTO_s
{
    public class ProductReviewDto
    {
        public int ProductReviewId { get; set; }
        public string ShopperName { get; set; }       // "Anonymous" when IsAnonymous
        public decimal? Rating { get; set; }
        public string Title { get; set; }
        public string Review { get; set; }
        public bool VerifiedPurchase { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> Photos { get; set; } = new();
    }
    public class ProductReviewSummaryDto
    {
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<ProductReviewDto> Reviews { get; set; } = new();
    }
}
