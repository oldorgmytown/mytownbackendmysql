namespace mytown.Models.DTO_s
{
    public class StoreShippingSelection
    {
        public int StoreId { get; set; }
        public int BranchId { get; set; }           // 0 when P2P
        public string ShippingType { get; set; }    // "standard" / "express" / "p2p"

        // ✅ NEW — only filled when ShippingType == "p2p"
        public int? TransporterRegId { get; set; }
        public int? TransporterPlanId { get; set; }
    }
}