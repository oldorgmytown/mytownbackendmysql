namespace mytown.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadProfileImageAsync(IFormFile file);
    }
}
