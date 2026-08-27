using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models.DTO_s
{
    public class Sku_ProductVariantDto
    {
        public long SkuId_Productvariant { get; set; }
        public long ProductId { get; set; }          
        public string? Color { get; set; }
        public int? SizeId { get; set; }
        public string? SizeName { get; set; }
        public decimal? Sku_Cost { get; set; }     
        public decimal? DiscountPrice { get; set; }
        public decimal? Quantity { get; set; }     

        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Discount { get; set; }
         public string? metric { get; set; }

        public string? VariantName => $"{SkuId_Productvariant}-{Color}";

        
        // Existing image metadata (returned on GET)
        public List<ProductImageDto> Images { get; set; } = new();

        // On UPDATE: full replacement filename list (already-uploaded via
        // /api/products-new/upload-image, same as create flow).
        public List<string>? UpdatedImageFileNames { get; set; }

        // Full replacement attribute list (same shape as create).
        public List<VariantAttributeDto> Attributes { get; set; } = new();
    }
}
