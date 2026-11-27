namespace mytown.Models.DTO_s
{
    public class BusinessServiceEnquiriesResponseDto
    {
        public int TotalCount { get; set; }
        public int NewCount { get; set; }
        public int PendingCount { get; set; }
        public int RespondedCount { get; set; }
        public int CompletedCount { get; set; }

        public List<BusinessServiceEnquiryListDto> Enquiries { get; set; } = new();
    }

    public class BusinessServiceEnquiryListDto
    {
        public int EnquiryId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int NumberOfUnits { get; set; }
        public DateTime PreferredDate { get; set; }
        public string PreferredTimeSlot { get; set; }
        public string ServiceLocation { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
