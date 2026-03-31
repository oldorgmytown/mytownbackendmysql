namespace mytown.Models.DTO_s
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public int BusRegId { get; set; }

        public string StoreName { get; set; }
        public int BuscatId { get; set; }
        public int ProductType { get; set; }
        public int ProdSubcatId { get; set; }

        public int? ProductTypeId { get; set; }  
        public int? FabricId { get; set; }       
        public int? DesignId { get; set; }

        public string? ProductName { get; set; }
        public string? ProductSubject { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductImage { get; set; }
        public string? SupplierName { get; set; }

        public decimal ProductAmount { get; set; }
        public decimal ProductLength { get; set; }
        public decimal ProductWidth { get; set; }
        public decimal ProductWeight { get; set; }
        public decimal Quantity { get; set; }
        public decimal ProductHeight { get; set; }

        public int PurchasedCount { get; set; }

        public decimal? Discount { get; set; }
        public decimal? DiscountPrice { get; set; }

        public string? Color { get; set; }
        public string? Size { get; set; }

        // 🔹 Extra field from BusinessRegister
        public string? BusinessName { get; set; }

        // Multiple images
        public List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
    }
}
