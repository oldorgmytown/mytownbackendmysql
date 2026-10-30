namespace mytown.Models.DTO_s
{
    public class StoreCourierRequestDto
    {
        public int? ShopperId { get; set; }

        public bool UseAlternateAddress { get; set; }
        public List<int> StoreIds { get; set; } = new();

        // Only required for Guest
        public int? GuestCustomerId { get; set; }       
        
        public List<GuestStoreWeightDto>? StoreWeights { get; set; }
    }

    public class GuestStoreWeightDto
    {
        public int StoreId { get; set; }

        public decimal TotalWeightKg { get; set; }
    }
}
