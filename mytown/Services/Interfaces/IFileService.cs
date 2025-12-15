namespace mytown.Services.Interfaces
{
    public interface IFileService
    {
        Task<object> UploadProfileImageAsync(IFormFile file, HttpRequest request);
    }
}
