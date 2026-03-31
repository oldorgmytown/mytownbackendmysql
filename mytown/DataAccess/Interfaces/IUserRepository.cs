namespace mytown.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<object?> LoginAsyncwithRole(string email, string password, string role);
    }
}
