namespace mytown.Models.DTO_s
{
    public class CourierBranchLoginDto
    {
        public int BranchId { get; set; }
        public int CourierId { get; set; }

        public string CourierServiceName { get; set; }

        public string BranchEmailId { get; set; }
        public string BranchPhoneNumber { get; set; }

        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
    }
}
