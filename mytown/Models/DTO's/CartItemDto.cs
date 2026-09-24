public class CartItemDto
{
    public int CartId { get; set; }
    public int ShopperRegId { get; set; }
    public long product_id { get; set; }
    public int prod_qty { get; set; }
    public string orderstatus { get; set; }
    public string product_name { get; set; }
    public string? product_subject { get; set; }
    public string? product_description { get; set; }
    public string? product_image { get; set; }
    public decimal product_cost { get; set; }
    public bool IsProductAvailable { get; set; }

    public string? StoreName { get; set; }
    public string? StoreLocation { get; set; }
    public int StoreId { get; set; }
    public string? StoreLogo { get; set; }

    // SKU / Variant Info
    public long Sku_Id { get; set; }
    public decimal? Weight { get; set; }
    public string? MeasurementUnit { get; set; }
    public string? Brand { get; set; }
    public decimal? Discount { get; set; }
    public decimal? DiscountPrice { get; set; }
}