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

        public async Task<(Dictionary<string, object> response, string token, string sessionId)>
         LoginAsync(string email, string password, string role)
        {
            // call ROLE-BASED repo
            var result = await _repo.LoginAsyncwithRole(email, password, role);

            if (result == null)
                return (null!, null!, null!);

            var token = result.GetType()
                .GetProperty("token")?
                .GetValue(result)?
                .ToString();

            var sessionId = result.GetType()
                .GetProperty("sessionId")?
                .GetValue(result)?
                .ToString();

            var response = new Dictionary<string, object>
            {
                ["userType"] = result.GetType()
                    .GetProperty("userType")?
                    .GetValue(result)!
            };

            void AddIfExists(string prop)
            {
                var val = result.GetType().GetProperty(prop)?.GetValue(result);
                if (val != null)
                    response[prop] = val;
            }

            // match EXACT repo return keys
            AddIfExists("user");
            AddIfExists("businessProfile");
            AddIfExists("shopper");
            AddIfExists("courier");
            AddIfExists("hasBranches");
            AddIfExists("transporter");
            AddIfExists("sender");

            return (response, token!, sessionId!);
        }
    }
}
