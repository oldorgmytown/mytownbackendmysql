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

        // Filenames returned by POST /api/products-new/upload-image,
        // uploaded separately before this request is sent.
        public List<string>? Images { get; set; }

        public List<CreateProductVariantAttributeRequest> Attributes { get; set; }
            = new List<CreateProductVariantAttributeRequest>();
    }
}