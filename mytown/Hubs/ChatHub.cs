using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using mytown.Controllers.Helpers;
using mytown.Helpers;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;

namespace mytown.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ConnectionManager _connectionManager;
        private readonly AppDbContext _context;

        public ChatHub(ConnectionManager connectionManager,
                       AppDbContext context)
        {
            _connectionManager = connectionManager;
            _context = context;
        }

        public Task RegisterConnection(int userId, UserType userType)
        {
            _connectionManager.AddConnection(userId, userType, Context.ConnectionId);
            return Task.CompletedTask;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = _connectionManager.GetUser(Context.ConnectionId);

            if (user != null)
            {
                _connectionManager.RemoveConnection(user.UserId, user.UserType);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task<bool> SendMessage(int receiverId, UserType receiverType, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return false;

            var sender = _connectionManager.GetUser(Context.ConnectionId);

            if (sender == null)
                return false;

            if (sender.UserId == receiverId && sender.UserType == receiverType)
                return false;

            // Don't bail out here anymore — save the message regardless of
            // whether the receiver is currently online. Only affects whether
            // we push it live via SignalR right now.
            var receiverConnectionId = _connectionManager.GetConnection(receiverId, receiverType);

            string senderName;
            string? senderPhoto = null;

            if (sender.UserType == UserType.Shopper)
            {
                var shopper = await _context.ShopperRegisters
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ShopperRegId == sender.UserId);

                if (shopper == null)
                    return false;

                senderName = shopper.Username;
                senderPhoto = shopper.PhotoName;
            }
            else
            {
                senderName = "Store";
            }

            var sentTime = DateTime.UtcNow;
            var trimmedMessage = message.Trim();

            // Persist to DB so history survives disconnects/reloads and expires after 10 days
            _context.ChatMessages.Add(new mytown.Models.ChatMessage
            {
                SenderUserId = sender.UserId,
                SenderType = sender.UserType,
                ReceiverUserId = receiverId,
                ReceiverType = receiverType,
                Message = trimmedMessage,
                SentTime = sentTime
            });
            await _context.SaveChangesAsync();

            var chatMessage = new ChatMessageDto
            {
                SenderUserId = sender.UserId,
                SenderType = sender.UserType,
                SenderName = senderName,
                SenderPhoto = senderPhoto,
                Message = trimmedMessage,
                SentTime = sentTime
            };

            // Only push live if the receiver happens to be connected right now.
            // If not, the message is already saved — they'll see it via chat-history
            // next time they open the chat.
            if (!string.IsNullOrEmpty(receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId)
                    .SendAsync("ReceiveMessage", chatMessage);
            }

            return true;
        }

        public async Task Typing(int receiverId, UserType receiverType)
        {
            var sender = _connectionManager.GetUser(Context.ConnectionId);

            if (sender == null)
                return;

            var receiverConnectionId = _connectionManager.GetConnection(receiverId, receiverType);

            if (string.IsNullOrEmpty(receiverConnectionId))
                return;

            string senderName;
            string? senderPhoto = null;

            if (sender.UserType == UserType.Shopper)
            {
                var shopper = await _context.ShopperRegisters
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ShopperRegId == sender.UserId);

                if (shopper == null)
                    return;

                senderName = shopper.Username;
                senderPhoto = shopper.PhotoName;
            }
            else
            {
                senderName = "Store";
            }

            var typingDto = new TypingDto
            {
                SenderUserId = sender.UserId,
                SenderType = sender.UserType,
                SenderName = senderName,
                SenderPhoto = senderPhoto
            };

            await Clients.Client(receiverConnectionId)
                .SendAsync("Typing", typingDto);
        }

        public async Task StopTyping(int receiverId, UserType receiverType)
        {
            var sender = _connectionManager.GetUser(Context.ConnectionId);

            if (sender == null)
                return;

            var receiverConnectionId = _connectionManager.GetConnection(receiverId, receiverType);

            if (string.IsNullOrEmpty(receiverConnectionId))
                return;

            await Clients.Client(receiverConnectionId)
                .SendAsync("StopTyping", new
                {
                    SenderId = sender.UserId,
                    SenderType = sender.UserType
                });
        }
    }
}