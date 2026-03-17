namespace mytown.Models.DTO_s
{
    public class CourierDeliveryFilterDto
    {
        public int? Month { get; set; }
        public int? Year { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
