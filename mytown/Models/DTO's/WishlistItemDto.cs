namespace mytown.Models.DTO_s
{
    public class WishlistItemDto
    {
        public int WishlistId { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int SkuId { get; set; }

        public string VariantImageUrl { get; set; }

        public decimal Price { get; set; }

        public int StoreId { get; set; }
        public string StoreName { get; set; }

        public bool IsProductAvailable { get; set; }

    }


}
