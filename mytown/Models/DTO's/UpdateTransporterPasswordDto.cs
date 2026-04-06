namespace mytown.Models.DTO_s
{
    public class UpdateTransporterPasswordDto
    {
        public int TransporterRegId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}