namespace mytown.Models.DTO_s
{
    public class StoreCourierRequestDto
    {
        public int ShopperId { get; set; }
        public List<int> StoreIds { get; set; } = new();
    }
}
