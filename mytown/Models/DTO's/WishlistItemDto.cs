namespace mytown.Models.DTO_s
{
    public class WishlistItemDto
    {
        public int CartId { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public string VariantImageUrl { get; set; }

        public decimal Price { get; set; }

        public int StoreId { get; set; }
        public string StoreName { get; set; }
    }


}
