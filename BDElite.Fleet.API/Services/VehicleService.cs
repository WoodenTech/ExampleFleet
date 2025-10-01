using BDElite.Fleet.API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BDElite.Fleet.API.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IMongoCollection<Vehicle> _vehiclesCollection;

        public VehicleService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _vehiclesCollection = mongoDatabase.GetCollection<Vehicle>("vehicles");
        }

        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            return await _vehiclesCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(string id)
        {
            return await _vehiclesCollection.Find(v => v.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
        {
            vehicle.CreatedAt = DateTime.UtcNow;
            vehicle.UpdatedAt = DateTime.UtcNow;
            await _vehiclesCollection.InsertOneAsync(vehicle);
            return vehicle;
        }

        public async Task<bool> UpdateVehicleAsync(string id, Vehicle vehicle)
        {
            vehicle.UpdatedAt = DateTime.UtcNow;
            var result = await _vehiclesCollection.ReplaceOneAsync(v => v.Id == id, vehicle);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteVehicleAsync(string id)
        {
            var result = await _vehiclesCollection.DeleteOneAsync(v => v.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<List<Vehicle>> GetVehiclesByStatusAsync(VehicleStatus status)
        {
            return await _vehiclesCollection.Find(v => v.Status == status).ToListAsync();
        }
    }
}