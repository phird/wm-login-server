using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using wm_api.Models;

namespace wm_api.Data
{
    public class DbContext
    {
        private readonly IMongoDatabase _database;

        public DbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("MongoDB:URI").Value;
            System.Console.WriteLine("i connecting ... ");
            //System.Console.WriteLine(connectionString);
            var databaseName = configuration.GetSection("MongoDB:DatabaseName").Value;

            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.RetryWrites = false;

            //var client = new MongoClient(settings);
            //var client = new MongoClient(connectionString);
            //_database = client.GetDatabase(databaseName);
            try
            {
                var client = new MongoClient(settings);
                _database = client.GetDatabase(databaseName);
                System.Console.WriteLine("MongoDB connection established successfully.");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error connecting to MongoDB: " + ex.Message);
                throw; // re-throw the exception to handle it further up the call stack
            }
        }

        public IMongoCollection<Users> Users => _database.GetCollection<Users>("userList");

        // Add other collections for different entities if needed
    }
}
