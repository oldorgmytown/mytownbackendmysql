using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.Controllers.Helpers;
using mytown.DataAccess.Interfaces;
using mytown.Helpers;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using MyTown.Models;

namespace mytown.DataAccess.Repositories
{
    public class ConnectionsRepository : IConnectionsRepository
    {
        private readonly AppDbContext _context;
        private readonly ConnectionManager _connectionManager;

        public ConnectionsRepository(AppDbContext context, ConnectionManager connectionManager)
        {
            _context = context;
            _connectionManager = connectionManager;
        }

        public async Task<ShopperExperience> CreateExperienceAsync(
      ShopperExperience experience)
        {
            _context.ShopperExperiences.Add(experience);

            await _context.SaveChangesAsync();

            return experience;
        }

        public async Task CreateExperiencePhotosAsync(
            List<ShopperExperiencePhoto> photos)
        {
            if (photos == null || photos.Count == 0)
                return;

            _context.ShopperExperiencePhotos.AddRange(photos);

            await _context.SaveChangesAsync();
        }
        public async Task<List<ShopperExperienceDto>> GetExperiencesByBusinessAsync(
    int busRegId,
    int shopperRegId)
        {
            var experiences = await (
                from e in _context.ShopperExperiences

                join s in _context.ShopperRegisters
                    on e.ShopperRegId equals s.ShopperRegId

                join b in _context.BusinessRegisters
                    on e.BusRegId equals b.BusRegId

                where e.BusRegId == busRegId
                      && e.Status == "Approved"

                orderby e.CreatedDate descending

                select new ShopperExperienceDto
                {
                    ShopperExperienceId = e.ShopperExperienceId,

                    ShopperRegId = e.ShopperRegId,

                    ShopperName = e.IsAnonymous
                        ? "Anonymous"
                        : s.Username,

                    BusRegId = e.BusRegId,

                    BusinessName = b.BusinessName,

                    PostType = e.PostType,

                    Rating = e.Rating,

                    Title = e.Title,

                    Experience = e.Experience,

                    IsAnonymous = e.IsAnonymous,

                    Status = e.Status,

                    CreatedDate = e.CreatedDate,

                    PhotoUrls = new List<string>(),

                    LikeCount = 0,

                    IsLiked = false,

                    CommentCount = 0
                }
            ).ToListAsync();

            if (!experiences.Any())
                return experiences;


            var experienceIds = experiences
                .Select(x => x.ShopperExperienceId)
                .ToList();


            // ------------------------------------------
            // PHOTOS
            // ------------------------------------------

            var photos = await _context.ShopperExperiencePhotos
                .Where(p =>
                    experienceIds.Contains(p.ShopperExperienceId))
                .Select(p => new
                {
                    p.ShopperExperienceId,
                    p.PhotoUrl
                })
                .ToListAsync();


            var photoLookup = photos
                .ToLookup(
                    p => p.ShopperExperienceId,
                    p => p.PhotoUrl);


            // ------------------------------------------
            // LIKES
            // ------------------------------------------

            var likes = await _context.ShopperExperienceLikes
                .Where(l =>
                    experienceIds.Contains(l.ShopperExperienceId))
                .Select(l => new
                {
                    l.ShopperExperienceId,
                    l.ShopperRegId
                })
                .ToListAsync();


            var likeLookup = likes
                .GroupBy(x => x.ShopperExperienceId)
                .ToDictionary(
                    x => x.Key,
                    x => x.ToList());


            // ------------------------------------------
            // COMMENTS COUNT
            // ------------------------------------------

            var commentCounts = await _context.ShopperExperienceComments
                .Where(c =>
                    experienceIds.Contains(c.ShopperExperienceId))
                .GroupBy(c => c.ShopperExperienceId)
                .Select(g => new
                {
                    ShopperExperienceId = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(
                    x => x.ShopperExperienceId,
                    x => x.Count);


            // ------------------------------------------
            // ATTACH DATA
            // ------------------------------------------

            foreach (var experience in experiences)
            {
                // Photos
                experience.PhotoUrls =
                    photoLookup[
                        experience.ShopperExperienceId
                    ].ToList();


                // Likes
                if (likeLookup.TryGetValue(
                    experience.ShopperExperienceId,
                    out var experienceLikes))
                {
                    experience.LikeCount =
                        experienceLikes.Count;

                    experience.IsLiked =
                        experienceLikes.Any(
                            x => x.ShopperRegId == shopperRegId);
                }


                // Comment count
                if (commentCounts.TryGetValue(
                    experience.ShopperExperienceId,
                    out var commentCount))
                {
                    experience.CommentCount =
                        commentCount;
                }
            }


            return experiences;
        }


        // Online visitors
        public async Task CaptureBusinessProfileViewAsync(CaptureBusinessProfileViewDto request)
        {
            var existingViewer = await _context.BusinessProfileViewers
                .FirstOrDefaultAsync(x =>
                    x.BusRegId == request.BusRegId &&
                    x.ShopperRegId == request.ShopperRegId);

            if (existingViewer == null)
            {
                var viewer = new BusinessProfileViewer
                {
                    BusRegId = request.BusRegId,
                    ShopperRegId = request.ShopperRegId,
                    LastSeen = DateTime.UtcNow
                };

                _context.BusinessProfileViewers.Add(viewer);
            }
            else
            {
                existingViewer.LastSeen = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<CurrentBusinessProfileViewerDto>>
    GetCurrentBusinessProfileViewersAsync(
    int busRegId,
    int currentShopperRegId)
        {
            var activeTime = DateTime.UtcNow.AddMinutes(-2);

            return await (
                from viewer in _context.BusinessProfileViewers
                join shopper in _context.ShopperRegisters
                    on viewer.ShopperRegId equals shopper.ShopperRegId

                where viewer.BusRegId == busRegId
                      && viewer.LastSeen >= activeTime
                      && shopper.Status == "Active"
                     && shopper.ShopperRegId != currentShopperRegId

                select new CurrentBusinessProfileViewerDto
                {
                    ShopperRegId = shopper.ShopperRegId,
                    Username = shopper.Username,
                    PhotoName = shopper.PhotoName,
                    IsOnline = true
                })
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> ConnectBusinessAsync(BusinessConnection connection)
        {
            var existing = await _context.BusinessConnections
                .FirstOrDefaultAsync(x =>
                    x.BusRegId == connection.BusRegId &&
                    x.ShopperRegId == connection.ShopperRegId);

            if (existing != null)
                return false;

            _context.BusinessConnections.Add(connection);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsBusinessConnectedAsync(
    int busRegId,
    int shopperRegId)
        {
            return await _context.BusinessConnections
                .AnyAsync(x =>
                    x.BusRegId == busRegId &&
                    x.ShopperRegId == shopperRegId &&
                    x.Status);
        }
        public async Task<List<ConnectedShopperDto>> GetConnectedShoppersAsync(int busRegId)
        {
            return await _context.BusinessConnections
                .Where(x => x.BusRegId == busRegId && x.Status)
                .Select(x => new ConnectedShopperDto
                {
                    ShopperRegId = x.ShopperRegId,
                    ShopperName = x.ShopperRegister.Username,
                    ShopperPhoto = x.ShopperRegister.PhotoName,
                    IsOnline = _connectionManager.GetConnection(
                        x.ShopperRegId,
                        UserType.Shopper) != null
                })
                .ToListAsync();
        }

        // like
        public async Task<ShopperExperienceLike> AddExperienceLikeAsync(
    ShopperExperienceLike like)
        {
            _context.ShopperExperienceLikes.Add(like);

            await _context.SaveChangesAsync();

            return like;
        }

        public async Task<bool> RemoveExperienceLikeAsync(
    int shopperExperienceId,
    int shopperRegId)
        {
            var like = await _context.ShopperExperienceLikes
                .FirstOrDefaultAsync(x =>
                    x.ShopperExperienceId == shopperExperienceId &&
                    x.ShopperRegId == shopperRegId);

            if (like == null)
                return false;

            _context.ShopperExperienceLikes.Remove(like);

            await _context.SaveChangesAsync();

            return true;
        }

        //check if liked by shopper

        public async Task<bool> IsExperienceLikedAsync(
    int shopperExperienceId,
    int shopperRegId)
        {
            return await _context.ShopperExperienceLikes
                .AnyAsync(x =>
                    x.ShopperExperienceId == shopperExperienceId &&
                    x.ShopperRegId == shopperRegId);
        }

        //like count

        public async Task<int> GetExperienceLikeCountAsync(
    int shopperExperienceId)
        {
            return await _context.ShopperExperienceLikes
                .CountAsync(x =>
                    x.ShopperExperienceId == shopperExperienceId);
        }

        //addcomment
        public async Task<ShopperExperienceComment> AddExperienceCommentAsync(
    ShopperExperienceComment comment)
        {
            _context.ShopperExperienceComments.Add(comment);

            await _context.SaveChangesAsync();

            return comment;
        }

        //get comment
        public async Task<List<ShopperExperienceCommentDto>> GetExperienceCommentsAsync(
    int shopperExperienceId)
        {
            return await (
                from c in _context.ShopperExperienceComments
                join s in _context.ShopperRegisters
                    on c.ShopperRegId equals s.ShopperRegId
                where c.ShopperExperienceId == shopperExperienceId
                orderby c.CreatedDate descending
                select new ShopperExperienceCommentDto
                {
                    ShopperExperienceCommentId =
                        c.ShopperExperienceCommentId,

                    ShopperExperienceId =
                        c.ShopperExperienceId,

                    ShopperRegId =
                        c.ShopperRegId,

                    ShopperName =
                        c.IsAnonymous
                            ? "Anonymous"
                            : s.Username,

                    CommentText =
                        c.CommentText,

                    IsAnonymous =
                        c.IsAnonymous,

                    CreatedDate =
                        c.CreatedDate
                }
            ).ToListAsync();
        }
    }
}
