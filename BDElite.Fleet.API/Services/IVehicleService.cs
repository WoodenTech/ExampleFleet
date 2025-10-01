using BDElite.Fleet.API.Models;

namespace BDElite.Fleet.API.Services
{
    public interface IVehicleService
    {
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle?> GetVehicleByIdAsync(string id);
        Task<Vehicle> CreateVehicleAsync(Vehicle vehicle);
        Task<bool> UpdateVehicleAsync(string id, Vehicle vehicle);
        Task<bool> DeleteVehicleAsync(string id);
        Task<List<Vehicle>> GetVehiclesByStatusAsync(VehicleStatus status);
    }
}