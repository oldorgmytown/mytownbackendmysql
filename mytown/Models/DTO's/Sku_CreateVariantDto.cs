using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace mytown.Models.DTO_s
{
    public class Sku_CreateVariantDto
    {
        public int SkuId_Productvariant { get; set; }
        [Required(ErrorMessage = "ProductId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be greater than 0.")]
        public int ProductId { get; set; }

        [StringLength(50, ErrorMessage = "Color cannot exceed 50 characters.")]
        public string? Color { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "SizeId must be greater than 0.")]
        public int? SizeId { get; set; }

        [Required(ErrorMessage = "SKU cost is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sku_Cost must be greater than 0.")]
        public decimal? Sku_Cost { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public decimal? Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "DiscountPrice must be non-negative.")]
        public decimal? DiscountPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount must be non-negative.")]
        public decimal? Discount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Length must be non-negative.")]
        public decimal? Length { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Width must be non-negative.")]
        public decimal? Width { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Height must be non-negative.")]
        public decimal? Height { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Weight must be non-negative.")]
        public decimal? Weight { get; set; }

        [MinLength(1, ErrorMessage = "At least one image must be uploaded.")]
        public List<IFormFile> Images { get; set; } = new();
    }
}
