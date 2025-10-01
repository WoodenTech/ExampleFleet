using BDElite.Fleet.API.Models;
using BDElite.Fleet.API.Controllers;

namespace BDElite.Fleet.API.Services
{
    public interface IProductService
    {
        Task<List<InsuranceProduct>> GetAllProductsAsync();
        Task<InsuranceProduct?> GetProductByIdAsync(string id);
        Task<InsuranceProduct> CreateProductAsync(InsuranceProduct product);
        Task<bool> UpdateProductAsync(string id, InsuranceProduct product);
        Task<bool> DeleteProductAsync(string id);

        Task<List<InsuranceProduct>> GetProductsByProviderIdAsync(string providerId);
        Task<List<InsuranceProduct>> GetProductsByProviderIdsAsync(List<string> providerIds);
        Task<List<InsuranceProduct>> GetActiveProductsAsync();
        Task<List<InsuranceProduct>> SearchProductsAsync(ProductSearchCriteria criteria);

        Task<bool> UpdateProductRatesAsync(string providerId, RateUpdateRequest request);
        Task<decimal> CalculateQuotePremiumAsync(string productId, List<string> vehicleIds, List<Coverage> coverages);
    }

    public class ProductSearchCriteria
    {
        public List<string>? ProviderIds { get; set; }
        public List<VehicleType>? VehicleTypes { get; set; }
        public List<string>? States { get; set; }
        public List<CoverageType>? CoverageTypes { get; set; }
        public int? MinFleetSize { get; set; }
        public int? MaxFleetSize { get; set; }
        public decimal? MaxBaseRate { get; set; }
    }
}