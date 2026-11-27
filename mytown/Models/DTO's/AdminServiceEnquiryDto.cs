namespace mytown.Models.DTO_s
{
    public class AdminServiceEnquiryDto
    {
        public int BusRegId { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessLocation { get; set; }
        public int TotalEnquiries { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
    }
}
