using BDElite.Fleet.API.Models;

namespace BDElite.Fleet.API.Services
{
    public interface IUserService
    {
        Task<List<FleetManager>> GetFleetManagersAsync();
        Task<FleetManager?> GetFleetManagerByIdAsync(string id);
        Task<FleetManager> CreateFleetManagerAsync(FleetManager fleetManager);
        Task<bool> UpdateFleetManagerAsync(string id, FleetManager fleetManager);

        Task<List<InsuranceBroker>> GetInsuranceBrokersAsync();
        Task<InsuranceBroker?> GetInsuranceBrokerByIdAsync(string id);
        Task<InsuranceBroker> CreateInsuranceBrokerAsync(InsuranceBroker broker);
        Task<bool> UpdateInsuranceBrokerAsync(string id, InsuranceBroker broker);

        Task<List<InsuranceProvider>> GetInsuranceProvidersAsync();
        Task<InsuranceProvider?> GetInsuranceProviderByIdAsync(string id);
        Task<InsuranceProvider> CreateInsuranceProviderAsync(InsuranceProvider provider);
        Task<bool> UpdateInsuranceProviderAsync(string id, InsuranceProvider provider);

        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> DeleteUserAsync(string id);
    }
}