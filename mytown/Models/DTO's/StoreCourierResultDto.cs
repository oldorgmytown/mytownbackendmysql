namespace mytown.Models.DTO_s
{
    public class StoreCourierResultDto
    {
        public int StoreId { get; set; }
        public decimal TotalWeightKg { get; set; }
        public List<BestcourierinfoDto> CourierOptions { get; set; } = new();
    }
}
