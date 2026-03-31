namespace mytown.Models.DTO_s
{
    public class UploadDeliveryProofDto
    {
        public int StoreOrderId { get; set; }

        public IFormFile File { get; set; }
    }
}
