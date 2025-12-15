using mytown.Services.Interfaces;

namespace mytown.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
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
    }
}
