using Microsoft.EntityFrameworkCore;
using mytown.Controllers.Helpers;

//using mytown.Data;
using mytown.DTOs;
using mytown.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;

namespace mytown.Repositories
{
    public class ShopperConnectionRepository : IShopperConnectionRepository
    {
        private readonly AppDbContext _db;

        public ShopperConnectionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ShopperHeaderDto?> GetShopperHeaderAsync(int shopperId)
        {
            // Select only the columns we need. Password is never read.
            return await _db.ShopperRegisters
                .AsNoTracking()
                .Where(s => s.ShopperRegId == shopperId)
                .Select(s => new ShopperHeaderDto
                {
                    ShopperId = s.ShopperRegId,
                    Username = s.Username,
                    PhotoName = s.PhotoName,
                    Town = s.Town,
                    City = s.City,
                    State = s.State,
                    Country = s.Country
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetConnectedCountAsync(int shopperId)
        {
            return await _db.ShopperConnections
                .CountAsync(c => c.Status == ConnectionStatus.Accepted &&
                                (c.RequesterShopperId == shopperId ||
                                 c.ReceiverShopperId == shopperId));
        }

        public async Task<int> GetBusinessConnectedCountAsync(int shopperId)
        {
            // Active business connections only (status = true)
            return await _db.BusinessConnections
                .CountAsync(c => c.ShopperRegId == shopperId && c.Status);
        }

        public async Task<int> GetPostsCountAsync(int shopperId)
        {
            return await _db.ShopperPosts.CountAsync(p => p.ShopperRegId == shopperId);
        }

        public async Task<(List<ReceivedRequestDto> Items, int TotalCount)> GetReceivedRequestsAsync(
      int shopperId, string? search, int page, int pageSize)
        {
            var query =
                from c in _db.ShopperConnections.AsNoTracking()
                where c.ReceiverShopperId == shopperId && c.Status == ConnectionStatus.Pending
                join s in _db.ShopperRegisters.AsNoTracking().ApplyShopperSearch(search)
                    on c.RequesterShopperId equals s.ShopperRegId
                select new
                {
                    c.ConnectionId,
                    c.RequestedAt,
                    s.ShopperRegId,
                    s.Username,
                    s.PhotoName,
                    s.Town,
                    s.City,
                    s.State
                };

            var total = await query.CountAsync();

            var rows = await query
                .OrderByDescending(x => x.RequestedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Mutual count only for the shoppers on this page
            var mutuals = await GetMutualCountsAsync(shopperId, rows.Select(r => r.ShopperRegId).ToList());

            var items = rows.Select(r => new ReceivedRequestDto
            {
                ConnectionId = r.ConnectionId,
                ShopperId = r.ShopperRegId,
                Username = r.Username,
                PhotoName = r.PhotoName,
                Town = r.Town,
                City = r.City,
                State = r.State,
                MutualCount = mutuals.GetValueOrDefault(r.ShopperRegId),
                RequestedAt = r.RequestedAt
            }).ToList();

            return (items, total);
        }

        // Mutual = shoppers connected (accepted) to both me and the other shopper
        private async Task<Dictionary<int, int>> GetMutualCountsAsync(int shopperId, List<int> otherIds)
        {
            var result = new Dictionary<int, int>();
            if (otherIds.Count == 0) return result;

            var myFriends = await _db.ShopperConnections.AsNoTracking()
                .Where(c => c.Status == ConnectionStatus.Accepted &&
                           (c.RequesterShopperId == shopperId || c.ReceiverShopperId == shopperId))
                .Select(c => c.RequesterShopperId == shopperId ? c.ReceiverShopperId : c.RequesterShopperId)
                .ToListAsync();

            if (myFriends.Count == 0) return result;

            var links = await _db.ShopperConnections.AsNoTracking()
                .Where(c => c.Status == ConnectionStatus.Accepted &&
                           ((otherIds.Contains(c.RequesterShopperId) && myFriends.Contains(c.ReceiverShopperId)) ||
                            (otherIds.Contains(c.ReceiverShopperId) && myFriends.Contains(c.RequesterShopperId))))
                .Select(c => new { c.RequesterShopperId, c.ReceiverShopperId })
                .ToListAsync();

            var friendSet = myFriends.ToHashSet();
            foreach (var id in otherIds)
            {
                result[id] = links.Count(l =>
                    (l.RequesterShopperId == id && friendSet.Contains(l.ReceiverShopperId)) ||
                    (l.ReceiverShopperId == id && friendSet.Contains(l.RequesterShopperId)));
            }

            return result;
        }

        public async Task<ShopperConnection?> GetByIdAsync(int connectionId)
        {
            return await _db.ShopperConnections.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);
        }

        public async Task<bool> ShopperExistsAsync(int shopperId)
        {
            return await _db.ShopperRegisters.AnyAsync(s => s.ShopperRegId == shopperId);
        }

        // Finds the row between two shoppers, whichever of them sent it
        public async Task<ShopperConnection?> GetPairAsync(int shopperA, int shopperB)
        {
            return await _db.ShopperConnections.FirstOrDefaultAsync(c =>
                (c.RequesterShopperId == shopperA && c.ReceiverShopperId == shopperB) ||
                (c.RequesterShopperId == shopperB && c.ReceiverShopperId == shopperA));
        }

        public async Task AddAsync(ShopperConnection connection)
        {
            await _db.ShopperConnections.AddAsync(connection);
        }

        public async Task SaveAsync()
        {
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("Duplicate entry") == true)
            {
                throw new InvalidOperationException("A request already exists between you and this shopper.");
            }
        }

        public async Task<(List<SentRequestDto> Items, int TotalCount)> GetSentRequestsAsync(
      int shopperId, string? search, int page, int pageSize)
        {
            var query =
                from c in _db.ShopperConnections.AsNoTracking()
                where c.RequesterShopperId == shopperId && c.Status == ConnectionStatus.Pending
                join s in _db.ShopperRegisters.AsNoTracking().ApplyShopperSearch(search)
                    on c.ReceiverShopperId equals s.ShopperRegId
                select new
                {
                    c.ConnectionId,
                    c.RequestedAt,
                    s.ShopperRegId,
                    s.Username,
                    s.PhotoName,
                    s.Town,
                    s.City,
                    s.State
                };

            var total = await query.CountAsync();

            var rows = await query
                .OrderByDescending(x => x.RequestedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Mutual count only for the shoppers on this page
            var mutuals = await GetMutualCountsAsync(shopperId, rows.Select(r => r.ShopperRegId).ToList());

            var items = rows.Select(r => new SentRequestDto
            {
                ConnectionId = r.ConnectionId,
                ShopperId = r.ShopperRegId,
                Username = r.Username,
                PhotoName = r.PhotoName,
                Town = r.Town,
                City = r.City,
                State = r.State,
                MutualCount = mutuals.GetValueOrDefault(r.ShopperRegId),
                RequestedAt = r.RequestedAt
            }).ToList();

            return (items, total);
        }

        public void Remove(ShopperConnection connection)
        {
            _db.ShopperConnections.Remove(connection);
        }

        public async Task<(List<NotConnectedShopperDto> Items, int TotalCount)> GetNotConnectedShoppersAsync(
    int shopperId, string? search, int page, int pageSize)
        {
            // Every shopper except me (filtered by search), joined with the (at most one) row between us
            var query =
                from s in _db.ShopperRegisters.AsNoTracking().ApplyShopperSearch(search)
                where s.ShopperRegId != shopperId
                from c in _db.ShopperConnections.AsNoTracking()
                    .Where(c => (c.RequesterShopperId == shopperId && c.ReceiverShopperId == s.ShopperRegId) ||
                                (c.ReceiverShopperId == shopperId && c.RequesterShopperId == s.ShopperRegId))
                    .DefaultIfEmpty()
                    // Only: no relation at all, or a previously rejected request
                where c == null || c.Status == ConnectionStatus.Rejected
                select new
                {
                    s.ShopperRegId,
                    s.Username,
                    s.PhotoName,
                    s.Town,
                    s.City,
                    s.State
                };

            var total = await query.CountAsync();

            var rows = await query
                .OrderBy(x => x.Username)
                .ThenBy(x => x.ShopperRegId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Mutual count only for the shoppers on this page
            var mutuals = await GetMutualCountsAsync(shopperId, rows.Select(r => r.ShopperRegId).ToList());

            var items = rows.Select(r => new NotConnectedShopperDto
            {
                ShopperId = r.ShopperRegId,
                Username = r.Username,
                PhotoName = r.PhotoName,
                Town = r.Town,
                City = r.City,
                State = r.State,
                MutualCount = mutuals.GetValueOrDefault(r.ShopperRegId)
            }).ToList();

            return (items, total);
        }

        public async Task<(List<ReceivedRequestDto> Items, int TotalCount)> GetConnectedShoppersAsync(
       int shopperId, string? search, int page, int pageSize)
        {
            // Accepted rows where I am on either side; OtherId is the shopper we show
            var mine = _db.ShopperConnections.AsNoTracking()
                .Where(c => c.Status == ConnectionStatus.Accepted &&
                           (c.RequesterShopperId == shopperId || c.ReceiverShopperId == shopperId))
                .Select(c => new
                {
                    c.ConnectionId,
                    c.RequestedAt,
                    ConnectedAt = c.RespondedAt ?? c.RequestedAt,
                    OtherId = c.RequesterShopperId == shopperId ? c.ReceiverShopperId : c.RequesterShopperId
                });

            var query =
                from m in mine
                join s in _db.ShopperRegisters.AsNoTracking().ApplyShopperSearch(search)
                    on m.OtherId equals s.ShopperRegId
                select new
                {
                    m.ConnectionId,
                    m.RequestedAt,
                    m.ConnectedAt,
                    s.ShopperRegId,
                    s.Username,
                    s.PhotoName,
                    s.Town,
                    s.City,
                    s.State
                };

            var total = await query.CountAsync();

            // Newest connection first
            var rows = await query
                .OrderByDescending(x => x.ConnectedAt)
                .ThenBy(x => x.ShopperRegId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Mutual count only for the shoppers on this page
            var mutuals = await GetMutualCountsAsync(shopperId, rows.Select(r => r.ShopperRegId).ToList());

            var items = rows.Select(r => new ReceivedRequestDto
            {
                ConnectionId = r.ConnectionId,
                ShopperId = r.ShopperRegId,
                Username = r.Username,
                PhotoName = r.PhotoName,
                Town = r.Town,
                City = r.City,
                State = r.State,
                MutualCount = mutuals.GetValueOrDefault(r.ShopperRegId),
                RequestedAt = r.RequestedAt
            }).ToList();

            return (items, total);
        }

        public async Task<(List<ShopperBusinessConnectionDto> Items, int TotalCount)> GetBusinessConnectionsAsync(
    int shopperId, string? search, int page, int pageSize)
        {
            // Active connections of this shopper, joined with the business details
            var query =
                from c in _db.BusinessConnections.AsNoTracking()
                where c.ShopperRegId == shopperId && c.Status
                join b in _db.BusinessRegisters.AsNoTracking().ApplyBusinessSearch(search)
                    on c.BusRegId equals b.BusRegId
                select new
                {
                    c.BusConnectionId,
                    c.ConnectedOn,
                    b.BusRegId,
                    b.BusinessName,
                  //  b.PhotoName,
                    b.Town,
                    b.BusinessCity,
                    b.BusinessState
                };

            var total = await query.CountAsync();

            // Newest connection first
            var rows = await query
                .OrderByDescending(x => x.ConnectedOn)
                .ThenBy(x => x.BusRegId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = rows.Select(r => new ShopperBusinessConnectionDto
            {
                BusConnectionId = r.BusConnectionId,
                BusRegId = r.BusRegId,
                BusinessName = r.BusinessName,
               // PhotoName = r.PhotoName,
                Town = r.Town,
                City = r.BusinessCity,
                State = r.BusinessState,
                ConnectedOn = r.ConnectedOn
            }).ToList();

            return (items, total);
        }

        public async Task<(List<AllConnectionDto> Items, int TotalCount)> GetAllConnectionsAsync(
      int shopperId, string? search, int page, int pageSize)
        {
            // 1. Approved shopper connections: OtherId is the shopper we show
            var mine = _db.ShopperConnections.AsNoTracking()
                .Where(c => c.Status == ConnectionStatus.Accepted &&
                           (c.RequesterShopperId == shopperId || c.ReceiverShopperId == shopperId))
                .Select(c => new
                {
                    c.ConnectionId,
                    ConnectedOn = c.RespondedAt ?? c.RequestedAt,
                    OtherId = c.RequesterShopperId == shopperId ? c.ReceiverShopperId : c.RequesterShopperId
                });

            var shopperQuery =
                from m in mine
                join s in _db.ShopperRegisters.AsNoTracking().ApplyShopperSearch(search)
                    on m.OtherId equals s.ShopperRegId
                select new
                {
                    m.ConnectionId,
                    m.ConnectedOn,
                    s.ShopperRegId,
                    s.Username,
                    s.PhotoName,
                    s.Town,
                    s.City,
                    s.State
                };

            // 2. Active business connections of this shopper
            var businessQuery =
                from c in _db.BusinessConnections.AsNoTracking()
                where c.ShopperRegId == shopperId && c.Status
                join b in _db.BusinessRegisters.AsNoTracking().ApplyBusinessSearch(search)
                    on c.BusRegId equals b.BusRegId
                select new
                {
                    c.BusConnectionId,
                    c.ConnectedOn,
                    b.BusRegId,
                    b.BusinessName,
                  //  b.PhotoName,
                    b.Town,
                    b.BusinessCity,
                    b.BusinessState
                };

            // 3. Counts, run separately
            var shopperTotal = await shopperQuery.CountAsync();
            var businessTotal = await businessQuery.CountAsync();
            var total = shopperTotal + businessTotal;

            // 4. Each side only needs the rows that could land on pages 1..page
            var take = page * pageSize;

            var shopperRows = await shopperQuery
                .OrderByDescending(x => x.ConnectedOn)
                .ThenBy(x => x.ShopperRegId)
                .Take(take)
                .ToListAsync();

            var businessRows = await businessQuery
                .OrderByDescending(x => x.ConnectedOn)
                .ThenBy(x => x.BusRegId)
                .Take(take)
                .ToListAsync();

            // 5. Convert to the DTO and merge in memory
            var combined = shopperRows.Select(r => new AllConnectionDto
            {
                Type = "Shopper",
                ConnectionId = r.ConnectionId,
                Id = r.ShopperRegId,
                Name = r.Username,
                PhotoName = r.PhotoName ?? "",
                Town = r.Town ?? "",
                City = r.City ?? "",
                State = r.State ?? "",
                MutualCount = 0,
                ConnectedOn = r.ConnectedOn
            })
            .Concat(businessRows.Select(r => new AllConnectionDto
            {
                Type = "Business",
                ConnectionId = r.BusConnectionId,
                Id = r.BusRegId,
                Name = r.BusinessName ?? "",
              //  PhotoName = r.PhotoName ?? "",
                Town = r.Town ?? "",
                City = r.BusinessCity ?? "",
                State = r.BusinessState ?? "",
                MutualCount = 0,
                ConnectedOn = r.ConnectedOn
            }));

            var items = combined
                .OrderByDescending(x => x.ConnectedOn)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Type)
                .ThenBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 6. Mutual count only for the shoppers on this page
            var shopperIds = items.Where(i => i.Type == "Shopper").Select(i => i.Id).ToList();
            var mutuals = await GetMutualCountsAsync(shopperId, shopperIds);

            foreach (var item in items.Where(i => i.Type == "Shopper"))
                item.MutualCount = mutuals.GetValueOrDefault(item.Id);

            return (items, total);
        }

        //get businessnames

        public async Task<List<BusinessNameDto>> GetBusinessNamesAsync()
        {
            return await _db.BusinessRegisters
                .AsNoTracking()
                .OrderBy(b => b.BusinessName)
                .Select(b => new BusinessNameDto
                {
                    BusRegId = b.BusRegId,
                    BusinessName = b.BusinessName
                })
                .ToListAsync();
        }

        public async Task AddAsync(ShopperPost post) =>
            await _db.ShopperPosts.AddAsync(post);

        // public Task SaveAsync() => _db.SaveChangesAsync();

        public async Task<ShopperPost?> GetByPostIdAsync(int postId) =>
    await _db.ShopperPosts.FirstOrDefaultAsync(p => p.PostId == postId);

        public void Remove(ShopperPost post) =>
            _db.ShopperPosts.Remove(post);
        public Task<bool> PostExistsAsync(int postId) =>
    _db.ShopperPosts.AnyAsync(p => p.PostId == postId);

        public Task<ShopperPostLike?> GetLikeAsync(int postId, int shopperId) =>
            _db.ShopperPostLikes.FirstOrDefaultAsync(l => l.PostId == postId && l.LikedByShopperId == shopperId);

        public async Task AddLikeAsync(ShopperPostLike like) =>
            await _db.ShopperPostLikes.AddAsync(like);

        public void RemoveLike(ShopperPostLike like) =>
            _db.ShopperPostLikes.Remove(like);

        public Task<int> CountLikesAsync(int postId) =>
            _db.ShopperPostLikes.CountAsync(l => l.PostId == postId);

        public async Task<(List<FeedPostDto> Items, int TotalCount)> GetFeedAsync(
    int shopperId, string filter, string? search, int page, int pageSize)
        {
            // My accepted connections
            var friendIds = await _db.ShopperConnections.AsNoTracking()
                .Where(c => c.Status == ConnectionStatus.Accepted &&
                           (c.RequesterShopperId == shopperId || c.ReceiverShopperId == shopperId))
                .Select(c => c.RequesterShopperId == shopperId ? c.ReceiverShopperId : c.RequesterShopperId)
                .ToListAsync();

            // Which posts can I see for this tab
            var posts = _db.ShopperPosts.AsNoTracking();
            posts = filter switch
            {
                "mine" => posts.Where(p => p.ShopperRegId == shopperId),
                "connections" => posts.Where(p => friendIds.Contains(p.ShopperRegId)),
                _ => posts.Where(p => p.ShopperRegId == shopperId || friendIds.Contains(p.ShopperRegId))
            };

            var query =
                from p in posts
                join s in _db.ShopperRegisters.AsNoTracking().ApplyShopperSearch(search)
                    on p.ShopperRegId equals s.ShopperRegId
                select new
                {
                    p.PostId,
                    p.Caption,
                    p.BusRegId,
                    p.CreatedAt,
                    s.ShopperRegId,
                    s.Username,
                    s.PhotoName,
                    s.Town,
                    s.City,
                    s.State,
                    s.IsEmailVerified
                };

            var total = await query.CountAsync();

            var rows = await query
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.PostId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Images, likes and business names for the posts on this page only
            var postIds = rows.Select(r => r.PostId).ToList();
            var busIds = rows.Where(r => r.BusRegId.HasValue).Select(r => r.BusRegId!.Value).Distinct().ToList();

            var images = await _db.ShopperPostImages.AsNoTracking()
                .Where(i => postIds.Contains(i.PostId))
                .OrderBy(i => i.SortOrder)
                .Select(i => new { i.PostId, i.ImageUrl })
                .ToListAsync();

            var likeCounts = await _db.ShopperPostLikes.AsNoTracking()
                .Where(l => postIds.Contains(l.PostId))
                .GroupBy(l => l.PostId)
                .Select(g => new { PostId = g.Key, Count = g.Count() })
                .ToListAsync();

            var myLikes = await _db.ShopperPostLikes.AsNoTracking()
                .Where(l => l.LikedByShopperId == shopperId && postIds.Contains(l.PostId))
                .Select(l => l.PostId)
                .ToListAsync();

            var businessNames = await _db.BusinessRegisters.AsNoTracking()
                .Where(b => busIds.Contains(b.BusRegId))
                .Select(b => new { b.BusRegId, b.BusinessName })
                .ToListAsync();

            var items = rows.Select(r => new FeedPostDto
            {
                PostId = r.PostId,
                Caption = r.Caption,
                CreatedAt = r.CreatedAt,
                ShopperId = r.ShopperRegId,
                Username = r.Username,
                PhotoName = r.PhotoName ?? "",
                Town = r.Town ?? "",
                City = r.City ?? "",
                State = r.State ?? "",
                IsVerified = r.IsEmailVerified,
                IsMine = r.ShopperRegId == shopperId,
                BusRegId = r.BusRegId,
                BusinessName = businessNames.FirstOrDefault(b => b.BusRegId == r.BusRegId)?.BusinessName,
                ImageUrls = images.Where(i => i.PostId == r.PostId).Select(i => i.ImageUrl).ToList(),
                LikeCount = likeCounts.FirstOrDefault(l => l.PostId == r.PostId)?.Count ?? 0,
                LikedByMe = myLikes.Contains(r.PostId)
            }).ToList();

            return (items, total);
        }

    }
}