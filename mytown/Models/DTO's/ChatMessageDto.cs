using mytown.Controllers.Helpers;

namespace mytown.Models.DTO_s
{
    public class ChatMessageDto
    {
        public int SenderUserId { get; set; }

        public string SenderName { get; set; } = string.Empty;

        public string? SenderPhoto { get; set; }

        public string Message { get; set; } = string.Empty;

        public DateTime SentTime { get; set; }

        public UserType SenderType { get; set; }
    }
}
