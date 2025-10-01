using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDElite.Fleet.API.Models
{
    public class InsuranceProduct
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("providerId")]
        public string ProviderId { get; set; } = string.Empty;

        [BsonElement("productCode")]
        public string ProductCode { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("coverageOptions")]
        public List<CoverageOption> CoverageOptions { get; set; } = new();

        [BsonElement("baseRate")]
        public decimal BaseRate { get; set; }

        [BsonElement("brokerMarkupPercentage")]
        public decimal BrokerMarkupPercentage { get; set; }

        [BsonElement("vehicleTypes")]
        public List<VehicleType> SupportedVehicleTypes { get; set; } = new();

        [BsonElement("industryTypes")]
        public List<string> SupportedIndustryTypes { get; set; } = new();

        [BsonElement("minimumFleetSize")]
        public int MinimumFleetSize { get; set; } = 1;

        [BsonElement("maximumFleetSize")]
        public int MaximumFleetSize { get; set; } = 1000;

        [BsonElement("underwritingRules")]
        public List<UnderwritingRule> UnderwritingRules { get; set; } = new();

        [BsonElement("ratingFactors")]
        public List<RatingFactor> RatingFactors { get; set; } = new();

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        [BsonElement("effectiveDate")]
        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;

        [BsonElement("expirationDate")]
        public DateTime? ExpirationDate { get; set; }

        [BsonElement("availableStates")]
        public List<string> AvailableStates { get; set; } = new();

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CoverageOption
    {
        [BsonElement("type")]
        public CoverageType Type { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("limitOptions")]
        public List<decimal> LimitOptions { get; set; } = new();

        [BsonElement("deductibleOptions")]
        public List<decimal> DeductibleOptions { get; set; } = new();

        [BsonElement("isRequired")]
        public bool IsRequired { get; set; }

        [BsonElement("baseRate")]
        public decimal BaseRate { get; set; }
    }

    public class UnderwritingRule
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("criteria")]
        public string Criteria { get; set; } = string.Empty;

        [BsonElement("action")]
        public UnderwritingAction Action { get; set; }

        [BsonElement("value")]
        public decimal Value { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
    }

    public class RatingFactor
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("category")]
        public RatingCategory Category { get; set; }

        [BsonElement("multiplier")]
        public decimal Multiplier { get; set; } = 1.0m;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("applicableVehicleTypes")]
        public List<VehicleType> ApplicableVehicleTypes { get; set; } = new();
    }

    public class Quote
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("quoteNumber")]
        public string QuoteNumber { get; set; } = string.Empty;

        [BsonElement("fleetManagerId")]
        public string FleetManagerId { get; set; } = string.Empty;

        [BsonElement("brokerId")]
        public string BrokerId { get; set; } = string.Empty;

        [BsonElement("productId")]
        public string ProductId { get; set; } = string.Empty;

        [BsonElement("vehicleIds")]
        public List<string> VehicleIds { get; set; } = new();

        [BsonElement("selectedCoverages")]
        public List<Coverage> SelectedCoverages { get; set; } = new();

        [BsonElement("basePremium")]
        public decimal BasePremium { get; set; }

        [BsonElement("brokerMarkup")]
        public decimal BrokerMarkup { get; set; }

        [BsonElement("totalPremium")]
        public decimal TotalPremium { get; set; }

        [BsonElement("validUntil")]
        public DateTime ValidUntil { get; set; }

        [BsonElement("status")]
        public QuoteStatus Status { get; set; } = QuoteStatus.Pending;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum VehicleType
    {
        Car,
        Truck,
        Van,
        Motorcycle,
        Bus,
        Trailer,
        SpecialtyVehicle
    }

    public enum UnderwritingAction
    {
        Accept,
        Decline,
        RequireInspection,
        ApplyDiscount,
        ApplySurcharge,
        RequireAdditionalDocumentation
    }

    public enum RatingCategory
    {
        VehicleAge,
        DriverExperience,
        Location,
        Usage,
        Safety,
        Industry,
        ClaimsHistory,
        FleetSize
    }

    public enum QuoteStatus
    {
        Pending,
        Generated,
        Sent,
        Accepted,
        Declined,
        Expired,
        Converted
    }
}