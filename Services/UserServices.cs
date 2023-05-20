using wm_api.Data;
using MongoDB.Driver;
using wm_api.Models;

namespace wm_api.Services
{
    public interface IUserService
    {
        Task<bool> IsUsernameTaken(string username);
    }

    public class UserService : IUserService
    {
        private readonly IMongoCollection<Users> _usersCollection;

        public UserService(IMongoCollection<Users> usersCollection)
        {
            _usersCollection = usersCollection;
        }

        public async Task<bool> IsUsernameTaken(string username)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.username, username);
            var user = await _usersCollection.Find(filter).FirstOrDefaultAsync();

            return user != null;
        }
    }
}
