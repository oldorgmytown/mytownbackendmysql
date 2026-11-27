using MimeKit.Tnef;

namespace mytown.Models.DTO_s
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Town { get; set; }

        public string City { get; set; }
    }
}
