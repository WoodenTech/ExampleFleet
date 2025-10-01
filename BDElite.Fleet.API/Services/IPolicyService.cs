using BDElite.Fleet.API.Models;
using BDElite.Fleet.API.Controllers;

namespace BDElite.Fleet.API.Services
{
    public interface IPolicyService
    {
        Task<List<InsurancePolicy>> GetAllPoliciesAsync();
        Task<InsurancePolicy?> GetPolicyByIdAsync(string id);
        Task<InsurancePolicy> CreatePolicyAsync(InsurancePolicy policy);
        Task<bool> UpdatePolicyAsync(string id, InsurancePolicy policy);
        Task<bool> DeletePolicyAsync(string id);

        Task<List<InsurancePolicy>> GetPoliciesByFleetManagerIdAsync(string fleetManagerId);
        Task<List<InsurancePolicy>> GetPoliciesByBrokerIdAsync(string brokerId);
        Task<List<InsurancePolicy>> GetPoliciesByProviderIdAsync(string providerId);
        Task<List<InsurancePolicy>> GetPoliciesByStatusAsync(PolicyStatus status);

        Task<InsurancePolicy?> BindPolicyFromQuoteAsync(string quoteId, string brokerId);
        Task<bool> CancelPolicyAsync(string policyId, string reason);
        Task<bool> RenewPolicyAsync(string policyId, DateTime newExpirationDate);

        Task<CommissionReport> GetBrokerCommissionReportAsync(string brokerId, DateTime startDate, DateTime endDate);
        Task<BusinessReport> GetProviderBusinessReportAsync(string providerId, DateTime startDate, DateTime endDate);
        Task<RiskAssessment> GetRiskAssessmentAsync(string providerId, string fleetManagerId);

        Task<Claim> CreateClaimAsync(Claim claim);
        Task<bool> UpdateClaimAsync(string claimId, Claim claim);
        Task<List<Claim>> GetClaimsByPolicyIdAsync(string policyId);
    }
}