using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using mytown.Helpers;
using mytown.Models;
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

        public Task RegisterShopper(int shopperRegId)
        {
            _connectionManager.AddConnection(shopperRegId, Context.ConnectionId);
            return Task.CompletedTask;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var shopperRegId = _connectionManager.GetShopperId(Context.ConnectionId);

            if (shopperRegId.HasValue)
            {
                _connectionManager.RemoveConnection(shopperRegId.Value);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task<bool> SendMessage(int receiverShopperId, string message)
        {
            var senderShopperId = _connectionManager.GetShopperId(Context.ConnectionId);

            if (!senderShopperId.HasValue)
                return false;

            if (senderShopperId.Value == receiverShopperId)
                return false;

            var receiverConnectionId = _connectionManager.GetConnection(receiverShopperId);

            if (string.IsNullOrEmpty(receiverConnectionId))
                return false;

            var sender = await _context.ShopperRegisters
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ShopperRegId == senderShopperId.Value);

            if (sender == null)
                return false;

            var chatMessage = new ChatMessageDto
            {
                SenderShopperId = sender.ShopperRegId,
                SenderName = sender.Username,
                SenderPhoto = sender.PhotoName,
                Message = message,
                SentTime = DateTime.UtcNow
            };

            await Clients.Client(receiverConnectionId)
                .SendAsync("ReceiveMessage", chatMessage);

            return true;
        }

        public async Task StopTyping(int receiverShopperId)
        {
            var senderShopperId = _connectionManager.GetShopperId(Context.ConnectionId);

            if (!senderShopperId.HasValue)
                return;

            var receiverConnectionId = _connectionManager.GetConnection(receiverShopperId);

            if (string.IsNullOrEmpty(receiverConnectionId))
                return;

            await Clients.Client(receiverConnectionId)
                .SendAsync("StopTyping", senderShopperId.Value);
        }
    }
}