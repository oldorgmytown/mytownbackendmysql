using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace mytown.Models.DTO_s
{
    public class ProductCreateDto
    {
        public int ProductId { get; set; }
        [Required(ErrorMessage = "BusRegId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "BusRegId must be greater than 0.")]
        public int BusRegId { get; set; }

        [Required(ErrorMessage = "BuscatId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "BuscatId must be greater than 0.")]
        public int BuscatId { get; set; }

        [Required(ErrorMessage = "ProdSubcatId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ProdSubcatId must be greater than 0.")]
        public int ProdSubcatId { get; set; }

        [StringLength(200, ErrorMessage = "ProductName cannot exceed 200 characters.")]
        [Required(ErrorMessage = "ProductName is required.")]
        public string ProductName { get; set; }

        [StringLength(1000, ErrorMessage = "ProductDescription cannot exceed 1000 characters.")]
        [Required(ErrorMessage = "ProductDescription is required.")]
        public string ProductDescription { get; set; }

        [StringLength(200, ErrorMessage = "SupplierName cannot exceed 200 characters.")]
        public string? SupplierName { get; set; }

        public int? ProductTypeId { get; set; }
        public int? FabricId { get; set; }
        public int? DesignId { get; set; }

        [JsonIgnore]
        public List<Sku_ProductVariantDto> Variants { get; set; } = new();
    }
}
