namespace mytown.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<object?> LoginAsync(string email, string password);
    }
}
