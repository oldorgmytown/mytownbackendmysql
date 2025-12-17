namespace mytown.Services.Interfaces
{
    public interface IUserService
    {
        Task<(Dictionary<string, object> response, string token, string sessionId)> LoginAsync(string email, string password);
    }
}
