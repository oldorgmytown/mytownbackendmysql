namespace mytown.Models.DTO_s
{
    public class AdminServiceEnquiryDto
    {
        public int ServiceProfileId { get; set; }
        public int BusRegId { get; set; }                  // "#490"
        public int BusServId { get; set; }

        public string? BusinessName { get; set; }          // "Sai Electronics"
        public string? BusinessLocation { get; set; }      // "Jayanagar, Banglore"
        public string? ServiceCategoryName { get; set; }   // "Electronics"
        public string? Status { get; set; }                // service profile status

        public int TotalEnquiries { get; set; }           // "2"
        public int NewEnquiries { get; set; }             // "New (1)"
        public int RespondedEnquiries { get; set; }       // "Responded (1)"
        public int ClosedEnquiries { get; set; }
    }
}
