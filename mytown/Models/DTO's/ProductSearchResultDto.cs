namespace mytown.Models.DTO_s
{
    public class ProductSearchResultDto
    {
        public int ProductId { get; set; }

        public string? ProductName { get; set; }
        

        public decimal? Price { get; set; }

        public decimal? Discount { get; set; }

        public decimal? DiscountPrice { get; set; }

        public string? Image { get; set; }
    }
}