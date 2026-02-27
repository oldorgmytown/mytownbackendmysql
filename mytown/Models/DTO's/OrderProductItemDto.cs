namespace mytown.Models.DTO_s
{
    public class OrderProductItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public int? SkuId { get; set; }

        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;

        public string? ProductImage { get; set; }

        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
    }


}
