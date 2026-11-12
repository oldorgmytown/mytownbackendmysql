using mytown.Controllers.Helpers;

namespace mytown.Models
{
    public class ChatMessage
    {
        public int ChatMessageId { get; set; }

        public int SenderUserId { get; set; }
        public UserType SenderType { get; set; }

        public int ReceiverUserId { get; set; }
        public UserType ReceiverType { get; set; }

        public string Message { get; set; } = string.Empty;

        public DateTime SentTime { get; set; }
    }
}