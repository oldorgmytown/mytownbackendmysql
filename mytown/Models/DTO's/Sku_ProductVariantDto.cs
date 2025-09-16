namespace mytown.Models.DTO_s
{
    public class Sku_ProductVariantDto
    {
        public int ProductId { get; set; }          // Required to link to the parent product
        public string? Color { get; set; }
        public string? Size { get; set; }

        public decimal Sku_Cost { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal Quantity { get; set; }

        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }

        public decimal? Discount { get; set; }

      //  public List<ProductImageDto>? Images { get; set; }
    }
}
