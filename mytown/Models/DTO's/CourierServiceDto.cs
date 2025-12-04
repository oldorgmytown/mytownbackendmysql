namespace mytown.Models.DTO_s
{

    public class CourierServiceDto
    {
        public string CourierServiceName { get; set; }
        public string CourierWebsiteName { get; set; }
        public string CourierPhone { get; set; }
        public string CourierEmail { get; set; }
        public string Password { get; set; } // incoming plain text
        public string cnfPassword { get; set; }
        public bool IsLocal { get; set; }
        public bool IsState { get; set; }
        public bool IsNational { get; set; }
        public bool IsInternational { get; set; }
    }



}
