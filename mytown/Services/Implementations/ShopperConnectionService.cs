using mytown.DTOs;
using mytown.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.Services
{
    public class ShopperConnectionService : IShopperConnectionService
    {
        private readonly IShopperConnectionRepository _repo;

        public ShopperConnectionService(IShopperConnectionRepository repo)
        {
            _repo = repo;
        }

        public async Task<ShopperHeaderDto> GetHeaderAsync(int shopperId)
        {
            var header = await _repo.GetShopperHeaderAsync(shopperId)
                ?? throw new KeyNotFoundException("Shopper not found.");

            header.ConnectedShoppers = await _repo.GetConnectedCountAsync(shopperId);
            header.ConnectedBusiness = await _repo.GetBusinessConnectedCountAsync(shopperId);   
            return header;
        }

        public async Task<mytown.DTOs.PagedResultDto<ReceivedRequestDto>> GetReceivedRequestsAsync(
     int shopperId, string? search, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var (items, total) = await _repo.GetReceivedRequestsAsync(shopperId, search, page, pageSize);

            return new mytown.DTOs.PagedResultDto<ReceivedRequestDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task ApproveAsync(int shopperId, int connectionId)
        {
            var connection = await _repo.GetByIdAsync(connectionId)
                ?? throw new KeyNotFoundException("Request not found.");

            // Only the shopper who received the request can approve it
            if (connection.ReceiverShopperId != shopperId)
                throw new UnauthorizedAccessException("Only the receiver can approve this request.");

            // Only pending requests can be approved
            if (connection.Status != ConnectionStatus.Pending)
                throw new InvalidOperationException("This request has already been answered.");

            connection.Status = ConnectionStatus.Accepted;
            connection.RespondedAt = DateTime.UtcNow;
            await _repo.SaveAsync();
        }

        public async Task<ConnectionActionDto> SendRequestAsync(int shopperId, int receiverShopperId)
        {
            if (shopperId == receiverShopperId)
                throw new ArgumentException("You cannot connect with yourself.");

            if (!await _repo.ShopperExistsAsync(shopperId) ||
                !await _repo.ShopperExistsAsync(receiverShopperId))
                throw new KeyNotFoundException("Shopper not found.");

            var existing = await _repo.GetPairAsync(shopperId, receiverShopperId);

            // No relation yet: create a new pending request
            if (existing == null)
            {
                var connection = new ShopperConnection
                {
                    RequesterShopperId = shopperId,
                    ReceiverShopperId = receiverShopperId,
                    Status = ConnectionStatus.Pending,
                    RequestedAt = DateTime.UtcNow
                };

                await _repo.AddAsync(connection);
                await _repo.SaveAsync();

                return new ConnectionActionDto
                {
                    ConnectionId = connection.ConnectionId,
                    RelationStatus = "RequestSent"
                };
            }

            switch (existing.Status)
            {
                case ConnectionStatus.Accepted:
                    throw new InvalidOperationException("You are already connected with this shopper.");

                case ConnectionStatus.Pending when existing.RequesterShopperId == shopperId:
                    throw new InvalidOperationException("You have already sent a request to this shopper.");

                case ConnectionStatus.Pending:
                    throw new InvalidOperationException(
                        "This shopper has already sent you a request. Approve it instead.");

                default:
                    // Rejected earlier: reuse the same row and send a fresh request
                    existing.RequesterShopperId = shopperId;
                    existing.ReceiverShopperId = receiverShopperId;
                    existing.Status = ConnectionStatus.Pending;
                    existing.RequestedAt = DateTime.UtcNow;
                    existing.RespondedAt = null;
                    await _repo.SaveAsync();

                    return new ConnectionActionDto
                    {
                        ConnectionId = existing.ConnectionId,
                        RelationStatus = "RequestSent"
                    };
            }
        }
        public async Task<mytown.DTOs.PagedResultDto<NotConnectedShopperDto>> GetNotConnectedShoppersAsync(
    int shopperId, string? search, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var (items, total) = await _repo.GetNotConnectedShoppersAsync(shopperId, search, page, pageSize);

            return new mytown.DTOs.PagedResultDto<NotConnectedShopperDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<mytown.DTOs.PagedResultDto<SentRequestDto>> GetSentRequestsAsync(
    int shopperId, string? search, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var (items, total) = await _repo.GetSentRequestsAsync(shopperId, search, page, pageSize);

            return new mytown.DTOs.PagedResultDto<SentRequestDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        // Withdraw: only the shopper who sent the request, and only while it is Pending
        public async Task WithdrawAsync(int shopperId, int connectionId)
        {
            var connection = await _repo.GetByIdAsync(connectionId)
                ?? throw new KeyNotFoundException("Request not found.");

            if (connection.RequesterShopperId != shopperId)
                throw new UnauthorizedAccessException("Only the sender can withdraw this request.");

            if (connection.Status != ConnectionStatus.Pending)
                throw new InvalidOperationException("This request has already been answered.");

            _repo.Remove(connection);
            await _repo.SaveAsync();
        }

        // Disconnect: either shopper can do it, and only while the connection is Accepted
        public async Task DisconnectAsync(int shopperId, int connectionId)
        {
            var connection = await _repo.GetByIdAsync(connectionId)
                ?? throw new KeyNotFoundException("Connection not found.");

            var isInvolved = connection.RequesterShopperId == shopperId ||
                             connection.ReceiverShopperId == shopperId;
            if (!isInvolved)
                throw new UnauthorizedAccessException("You cannot change this connection.");

            if (connection.Status != ConnectionStatus.Accepted)
                throw new InvalidOperationException("These shoppers are not connected.");

            _repo.Remove(connection);
            await _repo.SaveAsync();
        }

        public async Task<mytown.DTOs.PagedResultDto<ReceivedRequestDto>> GetConnectedShoppersAsync(
    int shopperId, string? search, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var (items, total) = await _repo.GetConnectedShoppersAsync(shopperId, search, page, pageSize);

            return new mytown.DTOs.PagedResultDto<ReceivedRequestDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<mytown.DTOs.PagedResultDto<ShopperBusinessConnectionDto>> GetBusinessConnectionsAsync(
    int shopperId, string? search, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var (items, total) = await _repo.GetBusinessConnectionsAsync(shopperId, search, page, pageSize);

            return new mytown.DTOs.PagedResultDto<ShopperBusinessConnectionDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<mytown.DTOs.PagedResultDto<AllConnectionDto>> GetAllConnectionsAsync(
    int shopperId, string? search, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var result = await _repo.GetAllConnectionsAsync(shopperId, search, page, pageSize);
            var items = result.Items;
            var total = result.TotalCount;

            return new mytown.DTOs.PagedResultDto<AllConnectionDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public Task<List<BusinessNameDto>> GetBusinessNamesAsync()
        {
            return _repo.GetBusinessNamesAsync();
        }


        public async Task<int> CreatePostAsync(CreatePostDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Caption))
                throw new ArgumentException("Caption is required.");

            if (dto.ImageUrls.Count > 2)
                throw new ArgumentException("You can add at most 2 photos.");

            var post = new ShopperPost
            {
                ShopperRegId = dto.ShopperId,
                Caption = dto.Caption,
                BusRegId = dto.BusRegId,
                CreatedAt = DateTime.UtcNow
            };

            byte order = 1;
            foreach (var url in dto.ImageUrls)
                post.Images.Add(new ShopperPostImage { ImageUrl = url, SortOrder = order++ });

            await _repo.AddAsync(post);
            await _repo.SaveAsync();     // saves the post and its images together

            return post.PostId;
        }
    }
}