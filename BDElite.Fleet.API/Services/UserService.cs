using BDElite.Fleet.API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BDElite.Fleet.API.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<FleetManager> _fleetManagersCollection;
        private readonly IMongoCollection<InsuranceBroker> _brokersCollection;
        private readonly IMongoCollection<InsuranceProvider> _providersCollection;
        private readonly IMongoCollection<User> _usersCollection;

        public UserService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

            _fleetManagersCollection = mongoDatabase.GetCollection<FleetManager>("fleetManagers");
            _brokersCollection = mongoDatabase.GetCollection<InsuranceBroker>("insuranceBrokers");
            _providersCollection = mongoDatabase.GetCollection<InsuranceProvider>("insuranceProviders");
            _usersCollection = mongoDatabase.GetCollection<User>("users");
        }

        public async Task<List<FleetManager>> GetFleetManagersAsync()
        {
            return await _fleetManagersCollection.Find(_ => true).ToListAsync();
        }

        public async Task<FleetManager?> GetFleetManagerByIdAsync(string id)
        {
            return await _fleetManagersCollection.Find(fm => fm.Id == id).FirstOrDefaultAsync();
        }

        public async Task<FleetManager> CreateFleetManagerAsync(FleetManager fleetManager)
        {
            fleetManager.UserType = UserType.FleetManager;
            fleetManager.CreatedAt = DateTime.UtcNow;
            fleetManager.UpdatedAt = DateTime.UtcNow;
            await _fleetManagersCollection.InsertOneAsync(fleetManager);
            return fleetManager;
        }

        public async Task<bool> UpdateFleetManagerAsync(string id, FleetManager fleetManager)
        {
            fleetManager.UpdatedAt = DateTime.UtcNow;
            var result = await _fleetManagersCollection.ReplaceOneAsync(fm => fm.Id == id, fleetManager);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<List<InsuranceBroker>> GetInsuranceBrokersAsync()
        {
            return await _brokersCollection.Find(_ => true).ToListAsync();
        }

        public async Task<InsuranceBroker?> GetInsuranceBrokerByIdAsync(string id)
        {
            return await _brokersCollection.Find(b => b.Id == id).FirstOrDefaultAsync();
        }

        public async Task<InsuranceBroker> CreateInsuranceBrokerAsync(InsuranceBroker broker)
        {
            broker.UserType = UserType.InsuranceBroker;
            broker.CreatedAt = DateTime.UtcNow;
            broker.UpdatedAt = DateTime.UtcNow;
            await _brokersCollection.InsertOneAsync(broker);
            return broker;
        }

        public async Task<bool> UpdateInsuranceBrokerAsync(string id, InsuranceBroker broker)
        {
            broker.UpdatedAt = DateTime.UtcNow;
            var result = await _brokersCollection.ReplaceOneAsync(b => b.Id == id, broker);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<List<InsuranceProvider>> GetInsuranceProvidersAsync()
        {
            return await _providersCollection.Find(_ => true).ToListAsync();
        }

        public async Task<InsuranceProvider?> GetInsuranceProviderByIdAsync(string id)
        {
            return await _providersCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<InsuranceProvider> CreateInsuranceProviderAsync(InsuranceProvider provider)
        {
            provider.UserType = UserType.InsuranceProvider;
            provider.CreatedAt = DateTime.UtcNow;
            provider.UpdatedAt = DateTime.UtcNow;
            await _providersCollection.InsertOneAsync(provider);
            return provider;
        }

        public async Task<bool> UpdateInsuranceProviderAsync(string id, InsuranceProvider provider)
        {
            provider.UpdatedAt = DateTime.UtcNow;
            var result = await _providersCollection.ReplaceOneAsync(p => p.Id == id, provider);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var result = await _usersCollection.DeleteOneAsync(u => u.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}