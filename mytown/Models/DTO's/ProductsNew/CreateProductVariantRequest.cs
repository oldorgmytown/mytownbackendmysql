using System.ComponentModel.DataAnnotations;

namespace mytown.DTOs.ProductsNew
{
    public class CreateProductVariantRequest
    {
        public decimal StockQuantity { get; set; }

        public decimal? Weight { get; set; }

        [StringLength(30)]
        public string? MeasurementUnit { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal Discount { get; set; } = 0;

        public decimal? DiscountPrice { get; set; }

        [StringLength(200)]
        public string? Brand { get; set; }

        public bool IsActive { get; set; } = true;

        public List<CreateProductVariantAttributeRequest> Attributes { get; set; }
            = new List<CreateProductVariantAttributeRequest>();
    }
}