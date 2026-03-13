namespace mytown.Models.DTO_s
{
    public class CourierBranchDto
{
    public int BranchId { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string Town { get; set; }

    public string BranchAddress { get; set; }

    public string BranchPhoneNumber { get; set; }

    public List<CourierBranchServiceDto> Services { get; set; }
}
}
