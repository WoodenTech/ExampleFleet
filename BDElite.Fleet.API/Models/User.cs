using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDElite.Fleet.API.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [BsonElement("lastName")]
        public string LastName { get; set; } = string.Empty;

        [BsonElement("userType")]
        public UserType UserType { get; set; }

        [BsonElement("companyName")]
        public string CompanyName { get; set; } = string.Empty;

        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [BsonElement("address")]
        public Address Address { get; set; } = new();

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class FleetManager : User
    {
        [BsonElement("fleetSize")]
        public int FleetSize { get; set; }

        [BsonElement("industryType")]
        public string IndustryType { get; set; } = string.Empty;

        [BsonElement("licenseNumber")]
        public string LicenseNumber { get; set; } = string.Empty;
    }

    public class InsuranceBroker : User
    {
        [BsonElement("brokerLicenseNumber")]
        public string BrokerLicenseNumber { get; set; } = string.Empty;

        [BsonElement("commissionRate")]
        public decimal CommissionRate { get; set; }

        [BsonElement("specializations")]
        public List<string> Specializations { get; set; } = new();

        [BsonElement("partneredProviderIds")]
        public List<string> PartneredProviderIds { get; set; } = new();
    }

    public class InsuranceProvider : User
    {
        [BsonElement("providerLicenseNumber")]
        public string ProviderLicenseNumber { get; set; } = string.Empty;

        [BsonElement("ratingAM")]
        public string RatingAM { get; set; } = string.Empty;

        [BsonElement("financialStrengthRating")]
        public string FinancialStrengthRating { get; set; } = string.Empty;

        [BsonElement("supportedCoverageTypes")]
        public List<CoverageType> SupportedCoverageTypes { get; set; } = new();

        [BsonElement("operatingStates")]
        public List<string> OperatingStates { get; set; } = new();
    }

    public class Address
    {
        [BsonElement("street")]
        public string Street { get; set; } = string.Empty;

        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("state")]
        public string State { get; set; } = string.Empty;

        [BsonElement("zipCode")]
        public string ZipCode { get; set; } = string.Empty;

        [BsonElement("country")]
        public string Country { get; set; } = "USA";
    }

    public enum UserType
    {
        FleetManager,
        InsuranceBroker,
        InsuranceProvider
    }

    public enum CoverageType
    {
        Liability,
        Collision,
        Comprehensive,
        UninsuredMotorist,
        MedicalPayments,
        PersonalInjuryProtection,
        CargoInsurance,
        GeneralLiability,
        WorkersCompensation
    }
}