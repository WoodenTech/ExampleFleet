using BDElite.Fleet.API.Models;
using BDElite.Fleet.API.Controllers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BDElite.Fleet.API.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly IMongoCollection<InsurancePolicy> _policiesCollection;
        private readonly IMongoCollection<Quote> _quotesCollection;

        public PolicyService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

            _policiesCollection = mongoDatabase.GetCollection<InsurancePolicy>("insurancePolicies");
            _quotesCollection = mongoDatabase.GetCollection<Quote>("quotes");
        }

        public async Task<List<InsurancePolicy>> GetAllPoliciesAsync()
        {
            return await _policiesCollection.Find(_ => true).ToListAsync();
        }

        public async Task<InsurancePolicy?> GetPolicyByIdAsync(string id)
        {
            return await _policiesCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<InsurancePolicy> CreatePolicyAsync(InsurancePolicy policy)
        {
            policy.CreatedAt = DateTime.UtcNow;
            policy.UpdatedAt = DateTime.UtcNow;
            await _policiesCollection.InsertOneAsync(policy);
            return policy;
        }

        public async Task<bool> UpdatePolicyAsync(string id, InsurancePolicy policy)
        {
            policy.UpdatedAt = DateTime.UtcNow;
            var result = await _policiesCollection.ReplaceOneAsync(p => p.Id == id, policy);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeletePolicyAsync(string id)
        {
            var result = await _policiesCollection.DeleteOneAsync(p => p.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<List<InsurancePolicy>> GetPoliciesByFleetManagerIdAsync(string fleetManagerId)
        {
            return await _policiesCollection.Find(p => p.FleetManagerId == fleetManagerId).ToListAsync();
        }

        public async Task<List<InsurancePolicy>> GetPoliciesByBrokerIdAsync(string brokerId)
        {
            return await _policiesCollection.Find(p => p.BrokerId == brokerId).ToListAsync();
        }

        public async Task<List<InsurancePolicy>> GetPoliciesByProviderIdAsync(string providerId)
        {
            return await _policiesCollection.Find(p => p.ProviderId == providerId).ToListAsync();
        }

        public async Task<List<InsurancePolicy>> GetPoliciesByStatusAsync(PolicyStatus status)
        {
            return await _policiesCollection.Find(p => p.Status == status).ToListAsync();
        }

        public async Task<InsurancePolicy?> BindPolicyFromQuoteAsync(string quoteId, string brokerId)
        {
            var quote = await _quotesCollection.Find(q => q.Id == quoteId && q.BrokerId == brokerId).FirstOrDefaultAsync();
            if (quote == null || quote.Status != QuoteStatus.Accepted)
            {
                return null;
            }

            var policy = new InsurancePolicy
            {
                PolicyNumber = GeneratePolicyNumber(),
                FleetManagerId = quote.FleetManagerId,
                BrokerId = quote.BrokerId,
                ProductId = quote.ProductId,
                VehicleIds = quote.VehicleIds,
                CoverageDetails = quote.SelectedCoverages,
                PremiumAmount = quote.TotalPremium,
                BrokerCommission = quote.BrokerMarkup,
                EffectiveDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddYears(1),
                Status = PolicyStatus.Active
            };

            await CreatePolicyAsync(policy);

            quote.Status = QuoteStatus.Converted;
            await _quotesCollection.ReplaceOneAsync(q => q.Id == quoteId, quote);

            return policy;
        }

        public async Task<bool> CancelPolicyAsync(string policyId, string reason)
        {
            var update = Builders<InsurancePolicy>.Update
                .Set(p => p.Status, PolicyStatus.Cancelled)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);

            var result = await _policiesCollection.UpdateOneAsync(p => p.Id == policyId, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> RenewPolicyAsync(string policyId, DateTime newExpirationDate)
        {
            var update = Builders<InsurancePolicy>.Update
                .Set(p => p.ExpirationDate, newExpirationDate)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);

            var result = await _policiesCollection.UpdateOneAsync(p => p.Id == policyId, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<CommissionReport> GetBrokerCommissionReportAsync(string brokerId, DateTime startDate, DateTime endDate)
        {
            var policies = await _policiesCollection
                .Find(p => p.BrokerId == brokerId && p.EffectiveDate >= startDate && p.EffectiveDate <= endDate)
                .ToListAsync();

            var totalCommission = policies.Sum(p => p.BrokerCommission);
            var averageCommission = policies.Count > 0 ? totalCommission / policies.Count : 0;

            var policyCommissions = policies.Select(p => new PolicyCommission
            {
                PolicyId = p.Id,
                PolicyNumber = p.PolicyNumber,
                PremiumAmount = p.PremiumAmount,
                CommissionAmount = p.BrokerCommission,
                EffectiveDate = p.EffectiveDate
            }).ToList();

            return new CommissionReport
            {
                BrokerId = brokerId,
                StartDate = startDate,
                EndDate = endDate,
                TotalCommissionEarned = totalCommission,
                PoliciesSold = policies.Count,
                AverageCommissionPerPolicy = averageCommission,
                PolicyCommissions = policyCommissions
            };
        }

        public async Task<BusinessReport> GetProviderBusinessReportAsync(string providerId, DateTime startDate, DateTime endDate)
        {
            var policies = await _policiesCollection
                .Find(p => p.ProviderId == providerId && p.EffectiveDate >= startDate && p.EffectiveDate <= endDate)
                .ToListAsync();

            var totalPremium = policies.Sum(p => p.PremiumAmount);
            var totalClaims = policies.SelectMany(p => p.Claims).Sum(c => c.ClaimAmount);
            var lossRatio = totalPremium > 0 ? totalClaims / totalPremium : 0;

            return new BusinessReport
            {
                ProviderId = providerId,
                StartDate = startDate,
                EndDate = endDate,
                TotalPremiumWritten = totalPremium,
                TotalClaimsPaid = totalClaims,
                LossRatio = lossRatio,
                ActivePolicies = policies.Count(p => p.Status == PolicyStatus.Active),
                NewPolicies = policies.Count,
                ProductPerformance = new List<ProductPerformance>(),
                BrokerPerformance = new List<BrokerPerformance>()
            };
        }

        public async Task<RiskAssessment> GetRiskAssessmentAsync(string providerId, string fleetManagerId)
        {
            // This would typically involve complex risk calculation logic
            // For now, returning a basic assessment
            return new RiskAssessment
            {
                FleetManagerId = fleetManagerId,
                OverallRiskLevel = RiskLevel.Medium,
                RecommendedPremiumMultiplier = 1.0m,
                RiskFactors = new List<RiskFactor>
                {
                    new RiskFactor
                    {
                        Name = "Fleet Size",
                        Level = RiskLevel.Low,
                        Description = "Fleet size is within acceptable range",
                        Impact = 0.1m
                    }
                },
                Recommendations = new List<string>
                {
                    "Regular vehicle maintenance",
                    "Driver training programs"
                }
            };
        }

        public async Task<Claim> CreateClaimAsync(Claim claim)
        {
            claim.CreatedAt = DateTime.UtcNow;

            // Find the policy and add the claim
            var policy = await _policiesCollection.Find(p => p.Id == claim.ClaimNumber).FirstOrDefaultAsync();
            if (policy != null)
            {
                policy.Claims.Add(claim);
                await _policiesCollection.ReplaceOneAsync(p => p.Id == policy.Id, policy);
            }

            return claim;
        }

        public async Task<bool> UpdateClaimAsync(string claimId, Claim claim)
        {
            // This would require finding the policy containing the claim and updating it
            // Implementation would depend on how claims are stored
            return await Task.FromResult(true);
        }

        public async Task<List<Claim>> GetClaimsByPolicyIdAsync(string policyId)
        {
            var policy = await _policiesCollection.Find(p => p.Id == policyId).FirstOrDefaultAsync();
            return policy?.Claims ?? new List<Claim>();
        }

        private string GeneratePolicyNumber()
        {
            return $"POL-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }
    }
}