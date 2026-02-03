using Azure.Storage.Blobs;
using mytown.Services.Interfaces;

namespace mytown.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public FileService(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        public async Task<object> UploadProfileImageAsync(IFormFile file, HttpRequest request)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded");

            var folder = Path.Combine(_env.WebRootPath, "UploadedFiles");
            Directory.CreateDirectory(folder);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var newFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{timestamp}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folder, newFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var url = $"{request.Scheme}://{request.Host}/UploadedFiles/{newFileName}";
            return new { FileName = newFileName, Url = url };
        }

        public async Task<string> UploadProfileImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded");

            var imageType = "profile"; // ✅ decided internally

            var containerName = _configuration["AzureBlobStorage:ContainerName"];
            var connectionString = _configuration["AzureBlobStorage:ConnectionString"];

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(
                Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            var fileExtension = Path.GetExtension(file.FileName);

            var newFileName = $"{imageType}_{fileNameWithoutExtension}_{timestamp}{fileExtension}";

            var blobClient = containerClient.GetBlobClient(newFileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return newFileName; // ✅ frontend unchanged
        }

    }
}
