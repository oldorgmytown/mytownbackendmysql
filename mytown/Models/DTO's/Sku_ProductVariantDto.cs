using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models.DTO_s
{
    public class Sku_ProductVariantDto
    {
        public int SkuId_Productvariant { get; set; }
        public int ProductId { get; set; }          
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


        public string? VariantName => $"{SkuId_Productvariant}-{Color}";

        
        public List<ProductImageDto> Images { get; set; } = new();

        //  public List<ProductImageDto>? Images { get; set; }

      

       

    }
}
