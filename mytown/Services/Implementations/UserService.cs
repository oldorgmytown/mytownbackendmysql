using mytown.DataAccess.Interfaces;
using mytown.Services.Interfaces;

namespace mytown.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<(Dictionary<string, object>, string, string)> LoginAsync(string email, string password)
        {
            var result = await _repo.LoginAsync(email, password);
            if (result == null)
                return (null!, null!, null!);

            var token = result.GetType().GetProperty("token")?.GetValue(result)?.ToString();
            var sessionId = result.GetType().GetProperty("sessionId")?.GetValue(result)?.ToString();

            var response = new Dictionary<string, object>
            {
                ["userType"] = result.GetType().GetProperty("userType")?.GetValue(result)
            };

            void AddIfExists(string prop)
            {
                var val = result.GetType().GetProperty(prop)?.GetValue(result);
                if (val != null) response[prop] = val;
            }

            AddIfExists("user");
            AddIfExists("businessProfile");
            AddIfExists("shopper");
            AddIfExists("courier");

            return (response, token!, sessionId!);
        }
    }
}
