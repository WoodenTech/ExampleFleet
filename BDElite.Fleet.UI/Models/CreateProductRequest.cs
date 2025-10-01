namespace BDElite.Fleet.UI.Models
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PricingModel { get; set; } = "PerPolicy";
        public bool AdjustForAge { get; set; }
        public bool AdjustForWeight { get; set; }
        public bool AdjustForType { get; set; }
        public decimal BaseRate { get; set; }
        public decimal IPT { get; set; }
        public decimal BrokerMarkupPercentage { get; set; }
        public decimal MaxCoverage { get; set; }
        public decimal Deductible { get; set; }
        public List<string> CoverageTypes { get; set; } = new List<string>();
    }

    public class InsuranceProductDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ProviderId { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<CoverageOptionDto> CoverageOptions { get; set; } = new();
        public decimal BaseRate { get; set; }
        public decimal BrokerMarkupPercentage { get; set; }
        public List<VehicleTypeDto> SupportedVehicleTypes { get; set; } = new();
        public List<string> SupportedIndustryTypes { get; set; } = new();
        public int MinimumFleetSize { get; set; } = 1;
        public int MaximumFleetSize { get; set; } = 1000;
        public List<UnderwritingRuleDto> UnderwritingRules { get; set; } = new();
        public List<RatingFactorDto> RatingFactors { get; set; } = new();
        public bool IsActive { get; set; } = true;
        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpirationDate { get; set; }
        public List<string> AvailableStates { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CoverageOptionDto
    {
        public CoverageTypeDto Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<decimal> LimitOptions { get; set; } = new();
        public List<decimal> DeductibleOptions { get; set; } = new();
        public bool IsRequired { get; set; }
        public decimal BaseRate { get; set; }
    }

    public class UnderwritingRuleDto
    {
        public string Name { get; set; } = string.Empty;
        public string Criteria { get; set; } = string.Empty;
        public UnderwritingActionDto Action { get; set; }
        public decimal Value { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class RatingFactorDto
    {
        public string Name { get; set; } = string.Empty;
        public RatingCategoryDto Category { get; set; }
        public decimal Multiplier { get; set; } = 1.0m;
        public string Description { get; set; } = string.Empty;
        public List<VehicleTypeDto> ApplicableVehicleTypes { get; set; } = new();
    }

    public enum VehicleTypeDto
    {
        Car,
        Truck,
        Van,
        Motorcycle,
        Bus,
        Trailer,
        SpecialtyVehicle
    }

    public enum CoverageTypeDto
    {
        ThirdParty,
        Comprehensive,
        Collision,
        PersonalInjury,
        PropertyDamage,
        MedicalPayments,
        UninsuredMotorist,
        UnderinsuredMotorist,
        CargoProtection,
        MotorTruckCargo,
        GeneralLiability,
        GoodsInTransit
    }

    public enum UnderwritingActionDto
    {
        Accept,
        Decline,
        RequireInspection,
        ApplyDiscount,
        ApplySurcharge,
        RequireAdditionalDocumentation
    }

    public enum RatingCategoryDto
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
}