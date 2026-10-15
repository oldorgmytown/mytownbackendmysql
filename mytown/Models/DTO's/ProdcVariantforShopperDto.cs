namespace mytown.Models.DTO_s
{
    public class ProdcVariantforShopperDto
    {
        public int ProductId { get; set; }

       
        public int BusRegId { get; set; }
        public string? BusinessName { get; set; }

       
        public int BuscatId { get; set; }
        public string? BuscatName { get; set; }
         public string Location { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public int ProdcatId { get; set; }
        public string? ProdcatName { get; set; }


        public int? ProductTypeId { get; set; }
        public string? ProductTypeName { get; set; }

        public int? FabricId { get; set; }
        public string? FabricName { get; set; }

        public int? DesignId { get; set; }
        public string? DesignName { get; set; }

    
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public string? SupplierName { get; set; }

        // Variants
        public List<Sku_ProductVariantDto> Variants { get; set; } = new();
    }
}
