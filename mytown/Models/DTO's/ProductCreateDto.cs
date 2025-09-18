namespace mytown.Models.DTO_s
{
    public class ProductCreateDto
    {
        public int ProductId { get; set; }
        public int BusRegId { get; set; }
        public string? BusinessName { get; set; }
        public int BuscatId { get; set; }
        public int ProdSubcatId { get; set; }
        public int? ProductTypeId { get; set; }
        public int? FabricId { get; set; }
        public int? DesignId { get; set; }

        public string? ProductName { get; set; }
       // public string? ProductSubject { get; set; }
        public string? ProductDescription { get; set; }
       // public string? ProductImage { get; set; }
        public string? SupplierName { get; set; }

       

        // All variants for this product
        public List<Sku_ProductVariantDto> Variants { get; set; } = new();
    }
}
