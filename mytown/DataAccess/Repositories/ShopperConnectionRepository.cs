using Microsoft.EntityFrameworkCore;
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

        public async Task<(List<ReceivedRequestDto> Items, int TotalCount)> GetReceivedRequestsAsync(
    int shopperId, int page, int pageSize)
        {
            var query =
                from c in _db.ShopperConnections.AsNoTracking()
                where c.ReceiverShopperId == shopperId && c.Status == ConnectionStatus.Pending
                join s in _db.ShopperRegisters.AsNoTracking() on c.RequesterShopperId equals s.ShopperRegId
                select new
                {
                    c.ConnectionId,
                    c.RequestedAt,
                    s.ShopperRegId,
                    s.Username,
                    s.PhotoName,
                    s.Town
                };

            var total = await query.CountAsync();

            var rows = await query
                .OrderByDescending(x => x.RequestedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mutuals = await GetMutualCountsAsync(shopperId, rows.Select(r => r.ShopperRegId).ToList());

            var items = rows.Select(r => new ReceivedRequestDto
            {
                ConnectionId = r.ConnectionId,
                ShopperId = r.ShopperRegId,
                Username = r.Username,
                PhotoName = r.PhotoName,
                Town = r.Town,
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

        public async Task<(List<NotConnectedShopperDto> Items, int TotalCount)> GetNotConnectedShoppersAsync(
    int shopperId, string? search, int page, int pageSize)
        {
            // Every shopper except me, joined with the (at most one) row between us
            var query =
                from s in _db.ShopperRegisters.AsNoTracking()
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

            // Search by name, town or city
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(x => x.Username.Contains(term) ||
                                         x.Town.Contains(term) ||
                                         x.City.Contains(term));
            }

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

        public async Task<(List<SentRequestDto> Items, int TotalCount)> GetSentRequestsAsync(
    int shopperId, string? search, int page, int pageSize)
        {
            var query =
                from c in _db.ShopperConnections.AsNoTracking()
                where c.RequesterShopperId == shopperId && c.Status == ConnectionStatus.Pending
                join s in _db.ShopperRegisters.AsNoTracking() on c.ReceiverShopperId equals s.ShopperRegId
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

            // Search by name, town or city
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(x => x.Username.Contains(term) ||
                                         x.Town.Contains(term) ||
                                         x.City.Contains(term));
            }

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

        public async Task<(List<ReceivedRequestDto> Items, int TotalCount)> GetConnectedShoppersAsync(
    int shopperId, string? search, int page, int pageSize)
        {
            // Accepted rows where I am on either side; the "other" shopper is the one we show
            var query =
                from c in _db.ShopperConnections.AsNoTracking()
                where c.Status == ConnectionStatus.Accepted &&
                      (c.RequesterShopperId == shopperId || c.ReceiverShopperId == shopperId)
                join s in _db.ShopperRegisters.AsNoTracking()
                    on (c.RequesterShopperId == shopperId ? c.ReceiverShopperId : c.RequesterShopperId)
                    equals s.ShopperRegId
                select new
                {
                    c.ConnectionId,
                    c.RequestedAt,
                    ConnectedAt = c.RespondedAt ?? c.RequestedAt,
                    s.ShopperRegId,
                    s.Username,
                    s.PhotoName,
                    s.Town,
                    s.City,
                    s.State
                };

            // Search by name, town or city
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(x => x.Username.Contains(term) ||
                                         x.Town.Contains(term) ||
                                         x.City.Contains(term));
            }

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
               // City = r.City,
              //  State = r.State,
                MutualCount = mutuals.GetValueOrDefault(r.ShopperRegId),
                RequestedAt = r.RequestedAt
            }).ToList();

            return (items, total);
        }
    }
}