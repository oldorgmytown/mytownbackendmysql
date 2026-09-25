namespace mytown.Models.DTO_s
{
    public class BusinessProductDashboardDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
         public int SkuId { get; set; }
        public string CategoryName { get; set; }
        public string ProductType { get; set; }
        public string Fabric { get; set; }
        public string Design { get; set; }

        public string Supplier { get; set; }
        public string ProductDescription { get; set; }

        public decimal ProductAmount { get; set; }   // derived (min SKU price)
        public int InStock { get; set; }              // sum of SKU qty
        public decimal? Discount { get; set; }

        public int NoOfPurchased { get; set; }

        public string ProductImage { get; set; }
    }
}
