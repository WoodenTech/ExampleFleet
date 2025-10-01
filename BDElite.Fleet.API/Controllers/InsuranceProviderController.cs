using BDElite.Fleet.API.Models;
using BDElite.Fleet.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BDElite.Fleet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsuranceProviderController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IPolicyService _policyService;

        public InsuranceProviderController(
            IUserService userService,
            IProductService productService,
            IPolicyService policyService)
        {
            _userService = userService;
            _productService = productService;
            _policyService = policyService;
        }

        [HttpGet]
        public async Task<ActionResult<List<InsuranceProvider>>> GetAllProviders()
        {
            var providers = await _userService.GetInsuranceProvidersAsync();
            return Ok(providers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InsuranceProvider>> GetProvider(string id)
        {
            var provider = await _userService.GetInsuranceProviderByIdAsync(id);
            if (provider == null)
            {
                return NotFound();
            }
            return Ok(provider);
        }

        [HttpPost]
        public async Task<ActionResult<InsuranceProvider>> CreateProvider(InsuranceProvider provider)
        {
            var created = await _userService.CreateInsuranceProviderAsync(provider);
            return CreatedAtAction(nameof(GetProvider), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProvider(string id, InsuranceProvider provider)
        {
            var updated = await _userService.UpdateInsuranceProviderAsync(id, provider);
            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("{id}/products")]
        public async Task<ActionResult<List<InsuranceProduct>>> GetProviderProducts(string id)
        {
            var products = await _productService.GetProductsByProviderIdAsync(id);
            return Ok(products);
        }

        [HttpPost("{id}/products")]
        public async Task<ActionResult<InsuranceProduct>> CreateProduct(string id, InsuranceProduct product)
        {
            product.ProviderId = id;
            var created = await _productService.CreateProductAsync(product);
            return CreatedAtAction("GetProduct", "Products", new { id = created.Id }, created);
        }

        [HttpGet("{id}/policies")]
        public async Task<ActionResult<List<InsurancePolicy>>> GetProviderPolicies(string id)
        {
            var policies = await _policyService.GetPoliciesByProviderIdAsync(id);
            return Ok(policies);
        }

        [HttpGet("{id}/business-report")]
        public async Task<ActionResult<BusinessReport>> GetBusinessReport(string id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var report = await _policyService.GetProviderBusinessReportAsync(id, startDate, endDate);
            return Ok(report);
        }

        [HttpPost("{id}/update-rates")]
        public async Task<IActionResult> UpdateProductRates(string id, RateUpdateRequest request)
        {
            var updated = await _productService.UpdateProductRatesAsync(id, request);
            if (!updated)
            {
                return BadRequest("Failed to update rates");
            }
            return NoContent();
        }

        [HttpGet("{id}/risk-assessment")]
        public async Task<ActionResult<RiskAssessment>> GetRiskAssessment(string id, [FromQuery] string fleetManagerId)
        {
            var assessment = await _policyService.GetRiskAssessmentAsync(id, fleetManagerId);
            return Ok(assessment);
        }
    }

    public class BusinessReport
    {
        public string ProviderId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPremiumWritten { get; set; }
        public decimal TotalClaimsPaid { get; set; }
        public decimal LossRatio { get; set; }
        public int ActivePolicies { get; set; }
        public int NewPolicies { get; set; }
        public int RenewedPolicies { get; set; }
        public int CancelledPolicies { get; set; }
        public List<ProductPerformance> ProductPerformance { get; set; } = new();
        public List<BrokerPerformance> BrokerPerformance { get; set; } = new();
    }

    public class ProductPerformance
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal PremiumWritten { get; set; }
        public decimal ClaimsPaid { get; set; }
        public decimal LossRatio { get; set; }
        public int PoliciesSold { get; set; }
    }

    public class BrokerPerformance
    {
        public string BrokerId { get; set; } = string.Empty;
        public string BrokerName { get; set; } = string.Empty;
        public decimal PremiumWritten { get; set; }
        public decimal CommissionPaid { get; set; }
        public int PoliciesSold { get; set; }
    }

    public class RateUpdateRequest
    {
        public string ProductId { get; set; } = string.Empty;
        public decimal NewBaseRate { get; set; }
        public List<RatingFactorUpdate> RatingFactorUpdates { get; set; } = new();
        public DateTime EffectiveDate { get; set; }
    }

    public class RatingFactorUpdate
    {
        public string FactorName { get; set; } = string.Empty;
        public decimal NewMultiplier { get; set; }
    }

    public class RiskAssessment
    {
        public string FleetManagerId { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public RiskLevel OverallRiskLevel { get; set; }
        public decimal RecommendedPremiumMultiplier { get; set; }
        public List<RiskFactor> RiskFactors { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public DateTime AssessmentDate { get; set; } = DateTime.UtcNow;
    }

    public class RiskFactor
    {
        public string Name { get; set; } = string.Empty;
        public RiskLevel Level { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Impact { get; set; }
    }

    public enum RiskLevel
    {
        Low,
        Medium,
        High,
        Critical
    }
}