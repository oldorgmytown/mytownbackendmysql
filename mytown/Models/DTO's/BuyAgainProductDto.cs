namespace mytown.Models.DTO_s
{
    public class BuyAgainProductDto
    {
        public int ProductId { get; set; }
        public int SkuId { get; set; }

        public string ProductName { get; set; }
        public string VariantImage { get; set; }

        public int StoreId { get; set; }
        public string StoreName { get; set; }

        public DateTime LastOrderedOn { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

}
