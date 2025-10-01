using BDElite.Fleet.API.Models;
using BDElite.Fleet.API.Controllers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BDElite.Fleet.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IMongoCollection<InsuranceProduct> _productsCollection;

        public ProductService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

            _productsCollection = mongoDatabase.GetCollection<InsuranceProduct>("insuranceProducts");
        }

        public async Task<List<InsuranceProduct>> GetAllProductsAsync()
        {
            return await _productsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<InsuranceProduct?> GetProductByIdAsync(string id)
        {
            return await _productsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<InsuranceProduct> CreateProductAsync(InsuranceProduct product)
        {
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            await _productsCollection.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> UpdateProductAsync(string id, InsuranceProduct product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            var result = await _productsCollection.ReplaceOneAsync(p => p.Id == id, product);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            var result = await _productsCollection.DeleteOneAsync(p => p.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<List<InsuranceProduct>> GetProductsByProviderIdAsync(string providerId)
        {
            return await _productsCollection.Find(p => p.ProviderId == providerId).ToListAsync();
        }

        public async Task<List<InsuranceProduct>> GetProductsByProviderIdsAsync(List<string> providerIds)
        {
            return await _productsCollection.Find(p => providerIds.Contains(p.ProviderId)).ToListAsync();
        }

        public async Task<List<InsuranceProduct>> GetActiveProductsAsync()
        {
            return await _productsCollection.Find(p => p.IsActive).ToListAsync();
        }

        public async Task<List<InsuranceProduct>> SearchProductsAsync(ProductSearchCriteria criteria)
        {
            var filterBuilder = Builders<InsuranceProduct>.Filter;
            var filters = new List<FilterDefinition<InsuranceProduct>>();

            if (criteria.ProviderIds != null && criteria.ProviderIds.Any())
            {
                filters.Add(filterBuilder.In(p => p.ProviderId, criteria.ProviderIds));
            }

            if (criteria.VehicleTypes != null && criteria.VehicleTypes.Any())
            {
                filters.Add(filterBuilder.AnyIn(p => p.SupportedVehicleTypes, criteria.VehicleTypes));
            }

            if (criteria.States != null && criteria.States.Any())
            {
                filters.Add(filterBuilder.AnyIn(p => p.AvailableStates, criteria.States));
            }

            if (criteria.MinFleetSize.HasValue)
            {
                filters.Add(filterBuilder.Lte(p => p.MinimumFleetSize, criteria.MinFleetSize.Value));
            }

            if (criteria.MaxFleetSize.HasValue)
            {
                filters.Add(filterBuilder.Gte(p => p.MaximumFleetSize, criteria.MaxFleetSize.Value));
            }

            if (criteria.MaxBaseRate.HasValue)
            {
                filters.Add(filterBuilder.Lte(p => p.BaseRate, criteria.MaxBaseRate.Value));
            }

            filters.Add(filterBuilder.Eq(p => p.IsActive, true));

            var combinedFilter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;

            return await _productsCollection.Find(combinedFilter).ToListAsync();
        }

        public async Task<bool> UpdateProductRatesAsync(string providerId, RateUpdateRequest request)
        {
            var product = await _productsCollection.Find(p => p.Id == request.ProductId && p.ProviderId == providerId).FirstOrDefaultAsync();
            if (product == null)
            {
                return false;
            }

            product.BaseRate = request.NewBaseRate;

            foreach (var ratingFactorUpdate in request.RatingFactorUpdates)
            {
                var factor = product.RatingFactors.FirstOrDefault(rf => rf.Name == ratingFactorUpdate.FactorName);
                if (factor != null)
                {
                    factor.Multiplier = ratingFactorUpdate.NewMultiplier;
                }
            }

            product.UpdatedAt = DateTime.UtcNow;

            var result = await _productsCollection.ReplaceOneAsync(p => p.Id == product.Id, product);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<decimal> CalculateQuotePremiumAsync(string productId, List<string> vehicleIds, List<Coverage> coverages)
        {
            var product = await GetProductByIdAsync(productId);
            if (product == null)
            {
                return 0;
            }

            var basePremium = product.BaseRate * vehicleIds.Count;
            var coveragePremium = coverages.Sum(c => c.Premium);

            // Apply rating factors
            var ratingMultiplier = product.RatingFactors.Any()
                ? product.RatingFactors.Average(rf => rf.Multiplier)
                : 1.0m;

            return (basePremium + coveragePremium) * ratingMultiplier;
        }
    }
}