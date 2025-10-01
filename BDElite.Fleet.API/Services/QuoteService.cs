using BDElite.Fleet.API.Models;
using BDElite.Fleet.API.Controllers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BDElite.Fleet.API.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly IMongoCollection<Quote> _quotesCollection;
        private readonly IMongoCollection<InsuranceProduct> _productsCollection;

        public QuoteService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

            _quotesCollection = mongoDatabase.GetCollection<Quote>("quotes");
            _productsCollection = mongoDatabase.GetCollection<InsuranceProduct>("insuranceProducts");
        }

        public async Task<List<Quote>> GetAllQuotesAsync()
        {
            return await _quotesCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Quote?> GetQuoteByIdAsync(string id)
        {
            return await _quotesCollection.Find(q => q.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Quote> CreateQuoteAsync(string fleetManagerId, QuoteRequest request)
        {
            var product = await _productsCollection.Find(p => p.Id == request.ProductId).FirstOrDefaultAsync();
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }

            var basePremium = await CalculatePremiumAsync(product, request.VehicleIds, request.RequestedCoverages);
            var brokerMarkup = basePremium * (product.BrokerMarkupPercentage / 100);
            var totalPremium = basePremium + brokerMarkup;

            var quote = new Quote
            {
                QuoteNumber = GenerateQuoteNumber(),
                FleetManagerId = fleetManagerId,
                BrokerId = request.BrokerId,
                ProductId = request.ProductId,
                VehicleIds = request.VehicleIds,
                SelectedCoverages = request.RequestedCoverages,
                BasePremium = basePremium,
                BrokerMarkup = brokerMarkup,
                TotalPremium = totalPremium,
                ValidUntil = DateTime.UtcNow.AddDays(30),
                Status = QuoteStatus.Generated
            };

            await _quotesCollection.InsertOneAsync(quote);
            return quote;
        }

        public async Task<bool> UpdateQuoteAsync(string id, Quote quote)
        {
            quote.UpdatedAt = DateTime.UtcNow;
            var result = await _quotesCollection.ReplaceOneAsync(q => q.Id == id, quote);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteQuoteAsync(string id)
        {
            var result = await _quotesCollection.DeleteOneAsync(q => q.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<List<Quote>> GetQuotesByFleetManagerIdAsync(string fleetManagerId)
        {
            return await _quotesCollection.Find(q => q.FleetManagerId == fleetManagerId).ToListAsync();
        }

        public async Task<List<Quote>> GetQuotesByBrokerIdAsync(string brokerId)
        {
            return await _quotesCollection.Find(q => q.BrokerId == brokerId).ToListAsync();
        }

        public async Task<List<Quote>> GetQuotesByStatusAsync(QuoteStatus status)
        {
            return await _quotesCollection.Find(q => q.Status == status).ToListAsync();
        }

        public async Task<Quote> CalculateQuoteAsync(string productId, List<string> vehicleIds, List<Coverage> coverages)
        {
            var product = await _productsCollection.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }

            var basePremium = await CalculatePremiumAsync(product, vehicleIds, coverages);
            var brokerMarkup = basePremium * (product.BrokerMarkupPercentage / 100);

            return new Quote
            {
                ProductId = productId,
                VehicleIds = vehicleIds,
                SelectedCoverages = coverages,
                BasePremium = basePremium,
                BrokerMarkup = brokerMarkup,
                TotalPremium = basePremium + brokerMarkup,
                ValidUntil = DateTime.UtcNow.AddDays(30)
            };
        }

        public async Task<bool> AcceptQuoteAsync(string quoteId)
        {
            var update = Builders<Quote>.Update
                .Set(q => q.Status, QuoteStatus.Accepted)
                .Set(q => q.UpdatedAt, DateTime.UtcNow);

            var result = await _quotesCollection.UpdateOneAsync(q => q.Id == quoteId, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeclineQuoteAsync(string quoteId, string reason)
        {
            var update = Builders<Quote>.Update
                .Set(q => q.Status, QuoteStatus.Declined)
                .Set(q => q.UpdatedAt, DateTime.UtcNow);

            var result = await _quotesCollection.UpdateOneAsync(q => q.Id == quoteId, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> ExpireQuoteAsync(string quoteId)
        {
            var update = Builders<Quote>.Update
                .Set(q => q.Status, QuoteStatus.Expired)
                .Set(q => q.UpdatedAt, DateTime.UtcNow);

            var result = await _quotesCollection.UpdateOneAsync(q => q.Id == quoteId, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        private async Task<decimal> CalculatePremiumAsync(InsuranceProduct product, List<string> vehicleIds, List<Coverage> coverages)
        {
            // Basic premium calculation - would be more complex in real implementation
            var basePremium = product.BaseRate * vehicleIds.Count;
            var coveragePremium = coverages.Sum(c => c.Premium);

            // Apply rating factors
            var ratingMultiplier = product.RatingFactors.Sum(rf => rf.Multiplier) / Math.Max(product.RatingFactors.Count, 1);

            return (basePremium + coveragePremium) * ratingMultiplier;
        }

        private string GenerateQuoteNumber()
        {
            return $"QTE-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }
    }
}