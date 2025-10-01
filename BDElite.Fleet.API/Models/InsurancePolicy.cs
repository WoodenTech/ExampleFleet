using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDElite.Fleet.API.Models
{
    public class InsurancePolicy
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("policyNumber")]
        public string PolicyNumber { get; set; } = string.Empty;

        [BsonElement("fleetManagerId")]
        public string FleetManagerId { get; set; } = string.Empty;

        [BsonElement("brokerId")]
        public string BrokerId { get; set; } = string.Empty;

        [BsonElement("providerId")]
        public string ProviderId { get; set; } = string.Empty;

        [BsonElement("productId")]
        public string ProductId { get; set; } = string.Empty;

        [BsonElement("vehicleIds")]
        public List<string> VehicleIds { get; set; } = new();

        [BsonElement("coverageDetails")]
        public List<Coverage> CoverageDetails { get; set; } = new();

        [BsonElement("premiumAmount")]
        public decimal PremiumAmount { get; set; }

        [BsonElement("brokerCommission")]
        public decimal BrokerCommission { get; set; }

        [BsonElement("deductible")]
        public decimal Deductible { get; set; }

        [BsonElement("effectiveDate")]
        public DateTime EffectiveDate { get; set; }

        [BsonElement("expirationDate")]
        public DateTime ExpirationDate { get; set; }

        [BsonElement("status")]
        public PolicyStatus Status { get; set; } = PolicyStatus.Pending;

        [BsonElement("billingFrequency")]
        public BillingFrequency BillingFrequency { get; set; } = BillingFrequency.Monthly;

        [BsonElement("paymentMethod")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.BankTransfer;

        [BsonElement("claims")]
        public List<Claim> Claims { get; set; } = new();

        [BsonElement("documents")]
        public List<PolicyDocument> Documents { get; set; } = new();

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Coverage
    {
        [BsonElement("type")]
        public CoverageType Type { get; set; }

        [BsonElement("limit")]
        public decimal Limit { get; set; }

        [BsonElement("deductible")]
        public decimal Deductible { get; set; }

        [BsonElement("premium")]
        public decimal Premium { get; set; }

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;
    }

    public class Claim
    {
        [BsonElement("claimNumber")]
        public string ClaimNumber { get; set; } = string.Empty;

        [BsonElement("vehicleId")]
        public string VehicleId { get; set; } = string.Empty;

        [BsonElement("dateOfLoss")]
        public DateTime DateOfLoss { get; set; }

        [BsonElement("claimAmount")]
        public decimal ClaimAmount { get; set; }

        [BsonElement("status")]
        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("adjusterId")]
        public string AdjusterId { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class PolicyDocument
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("type")]
        public DocumentType Type { get; set; }

        [BsonElement("url")]
        public string Url { get; set; } = string.Empty;

        [BsonElement("uploadedAt")]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }

    public enum PolicyStatus
    {
        Pending,
        Active,
        Expired,
        Cancelled,
        Suspended
    }

    public enum ClaimStatus
    {
        Pending,
        UnderReview,
        Approved,
        Denied,
        Settled,
        Closed
    }

    public enum BillingFrequency
    {
        Monthly,
        Quarterly,
        SemiAnnually,
        Annually
    }

    public enum PaymentMethod
    {
        BankTransfer,
        CreditCard,
        Check,
        DirectDebit
    }

    public enum DocumentType
    {
        PolicyCertificate,
        ProofOfInsurance,
        ClaimForm,
        Endorsement,
        Application,
        Other
    }
}