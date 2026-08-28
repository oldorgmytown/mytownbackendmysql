namespace mytown.Models.DTO_s
{
    public class ProdVariantdetailsDto
    {
        public int ProductId { get; set; }
        public int BusRegId { get; set; }
        // public string? BusinessName { get; set; }
        public int BuscatId { get; set; }
        public int ProdSubcatId { get; set; }
        public int? ProductTypeId { get; set; }
        public int? ProductGroupId { get; set; }
        public string ProdTypename { get; set; }
        public int? FabricId { get; set; }
        public int? DesignId { get; set; }

        public string? ProductName { get; set; }
        // public string? ProductSubject { get; set; }
        public string? ProductDescription { get; set; }
        // public string? ProductImage { get; set; }
        public string? SupplierName { get; set; }

        // new to check product avaliability based on product status
        public bool IsProductAvailable { get; set; }

        public string Location { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;


        public List<Sku_ProductVariantDto> Variants { get; set; } = new();
    }
}

