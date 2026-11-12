using System.ComponentModel.DataAnnotations;

namespace mytown.DTOs.ProductsNew
{
    public class CreateProductNewRequest
    {
        [Required]
        public int BusRegId { get; set; }

        public long? BusCatId { get; set; }

        public long? ProdSubcatId { get; set; }

        public long? ProductGroupId { get; set; }

        public long? ProdTypeId { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        public string? ProductDescription { get; set; }

        public string ProductStatus { get; set; } = "ACTIVE";

        public bool IsActive { get; set; } = true;

        public List<CreateProductVariantRequest> Variants { get; set; }
            = new List<CreateProductVariantRequest>();
    }
}