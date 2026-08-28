using Azure.Storage.Blobs;
using mytown.DataAccess;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using MyTown.Models;

namespace mytown.Services.Implementations
{
    public class ConnectionsService : IConnectionsService
    {
        private readonly IConnectionsRepository _Connectrepo;
        private readonly IConfiguration _configuration;

        public ConnectionsService(IConnectionsRepository repo, IConfiguration configuration)
        {
            _Connectrepo = repo;
            _configuration = configuration;

        }

        public async Task<ShopperExperienceDto> CreateExperienceAsync(
      CreateShopperExperienceDto dto)
        {
            var experience = new ShopperExperience
            {
                ShopperRegId = dto.ShopperRegId,
                BusRegId = dto.BusRegId,
                PostType = dto.PostType,
                Rating = dto.Rating,
                Title = dto.Title,
                Experience = dto.ExperienceText,
                IsAnonymous = dto.IsAnonymous,
                Status = "Approved",
                CreatedDate = DateTime.UtcNow
            };

            // Create main experience
            var result = await _Connectrepo.CreateExperienceAsync(experience);

            // Save photos
            if (dto.PhotoUrls != null && dto.PhotoUrls.Any())
            {
                var photos = dto.PhotoUrls
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(photoUrl => new ShopperExperiencePhoto
                    {
                        ShopperExperienceId = result.ShopperExperienceId,
                        PhotoUrl = photoUrl,
                        CreatedDate = DateTime.UtcNow
                    })
                    .ToList();

                await _Connectrepo.CreateExperiencePhotosAsync(photos);
            }

            return new ShopperExperienceDto
            {
                ShopperExperienceId = result.ShopperExperienceId,
                ShopperRegId = result.ShopperRegId,
                BusRegId = result.BusRegId,
                PostType = result.PostType,
                Rating = result.Rating,
                Title = result.Title,
                Experience = result.Experience,
                IsAnonymous = result.IsAnonymous,
                Status = result.Status,
                CreatedDate = result.CreatedDate,

                PhotoUrls = dto.PhotoUrls
            };
        }
        public async Task<string> UploadToBlobAsync(IFormFile file, string imageType)
        {
            var containerName = _configuration["AzureBlobStorage:ContainerName"];
            var connectionString = _configuration["AzureBlobStorage:ConnectionString"];
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            var fileExtension = Path.GetExtension(file.FileName);
            var newFileName = $"{imageType}_{fileNameWithoutExtension}_{timestamp}{fileExtension}";
            var blobClient = containerClient.GetBlobClient(newFileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);
            return newFileName;
        }

        public async Task DeleteFromBlobAsync(string fileName)
        {
            var containerName = _configuration["AzureBlobStorage:ContainerName"];
            var connectionString = _configuration["AzureBlobStorage:ConnectionString"];
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.GetBlobClient(fileName).DeleteIfExistsAsync();
        }


        public async Task<List<ShopperExperienceDto>> GetExperiencesByBusinessAsync(int busRegId, int shopperRegId)
        {
            return await _Connectrepo.GetExperiencesByBusinessAsync(busRegId, shopperRegId);
        }

        public async Task CaptureBusinessProfileViewAsync(CaptureBusinessProfileViewDto request)
        {
            await _Connectrepo.CaptureBusinessProfileViewAsync(request);
        }

        public async Task<List<CurrentBusinessProfileViewerDto>> GetCurrentBusinessProfileViewersAsync(int busRegId, int shopperRegId)
        {
            return await _Connectrepo.GetCurrentBusinessProfileViewersAsync(busRegId, shopperRegId);
        }

        public async Task<bool> ConnectBusinessAsync(BusinessConnection connection)
        {
            return await _Connectrepo.ConnectBusinessAsync(connection);
        }

        public async Task<bool> IsBusinessConnectedAsync(int busRegId, int shopperRegId)
        {
            return await _Connectrepo.IsBusinessConnectedAsync(busRegId, shopperRegId);
        }

        public async Task<List<ConnectedShopperDto>> GetConnectedShoppersAsync(int busRegId)
        {
            return await _Connectrepo.GetConnectedShoppersAsync(busRegId);
        }

        // for likes and comments
        public async Task<bool> ToggleExperienceLikeAsync(
    ShopperExperienceLikeDto dto)
        {
            var alreadyLiked =
                await _Connectrepo.IsExperienceLikedAsync(
                    dto.ShopperExperienceId,
                    dto.ShopperRegId);

            if (alreadyLiked)
            {
                await _Connectrepo.RemoveExperienceLikeAsync(
                    dto.ShopperExperienceId,
                    dto.ShopperRegId);

                return false;
            }

            var like = new ShopperExperienceLike
            {
                ShopperExperienceId = dto.ShopperExperienceId,
                ShopperRegId = dto.ShopperRegId,
                CreatedDate = DateTime.UtcNow
            };

            await _Connectrepo.AddExperienceLikeAsync(like);

            return true;
        }

        //comments
        public async Task<ShopperExperienceCommentDto>
    AddExperienceCommentAsync(
        CreateShopperExperienceCommentDto dto)
        {
            var comment = new ShopperExperienceComment
            {
                ShopperExperienceId = dto.ShopperExperienceId,
                ShopperRegId = dto.ShopperRegId,
                CommentText = dto.CommentText,
                IsAnonymous = dto.IsAnonymous,
                CreatedDate = DateTime.UtcNow
            };

            var result =
                await _Connectrepo.AddExperienceCommentAsync(comment);

            return new ShopperExperienceCommentDto
            {
                ShopperExperienceCommentId =
                    result.ShopperExperienceCommentId,

                ShopperExperienceId =
                    result.ShopperExperienceId,

                ShopperRegId =
                    result.ShopperRegId,

                CommentText =
                    result.CommentText,

                IsAnonymous =
                    result.IsAnonymous,

                CreatedDate =
                    result.CreatedDate
            };
        }

        public async Task<List<ShopperExperienceCommentDto>>
    GetExperienceCommentsAsync(
        int shopperExperienceId)
        {
            return await _Connectrepo
                .GetExperienceCommentsAsync(shopperExperienceId);
        }
    }
}
