namespace mytown.Models.DTOs
{
    public class BusinessNotificationDto
    {
        public int NotificationId { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
