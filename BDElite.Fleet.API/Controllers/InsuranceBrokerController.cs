using BDElite.Fleet.API.Models;
using BDElite.Fleet.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BDElite.Fleet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsuranceBrokerController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPolicyService _policyService;
        private readonly IQuoteService _quoteService;
        private readonly IProductService _productService;

        public InsuranceBrokerController(
            IUserService userService,
            IPolicyService policyService,
            IQuoteService quoteService,
            IProductService productService)
        {
            _userService = userService;
            _policyService = policyService;
            _quoteService = quoteService;
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<InsuranceBroker>>> GetAllBrokers()
        {
            var brokers = await _userService.GetInsuranceBrokersAsync();
            return Ok(brokers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InsuranceBroker>> GetBroker(string id)
        {
            var broker = await _userService.GetInsuranceBrokerByIdAsync(id);
            if (broker == null)
            {
                return NotFound();
            }
            return Ok(broker);
        }

        [HttpPost]
        public async Task<ActionResult<InsuranceBroker>> CreateBroker(InsuranceBroker broker)
        {
            var created = await _userService.CreateInsuranceBrokerAsync(broker);
            return CreatedAtAction(nameof(GetBroker), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBroker(string id, InsuranceBroker broker)
        {
            var updated = await _userService.UpdateInsuranceBrokerAsync(id, broker);
            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("{id}/policies")]
        public async Task<ActionResult<List<InsurancePolicy>>> GetBrokerPolicies(string id)
        {
            var policies = await _policyService.GetPoliciesByBrokerIdAsync(id);
            return Ok(policies);
        }

        [HttpGet("{id}/quotes")]
        public async Task<ActionResult<List<Quote>>> GetBrokerQuotes(string id)
        {
            var quotes = await _quoteService.GetQuotesByBrokerIdAsync(id);
            return Ok(quotes);
        }

        [HttpGet("{id}/available-products")]
        public async Task<ActionResult<List<InsuranceProduct>>> GetAvailableProducts(string id)
        {
            var broker = await _userService.GetInsuranceBrokerByIdAsync(id);
            if (broker == null)
            {
                return NotFound();
            }

            var products = await _productService.GetProductsByProviderIdsAsync(broker.PartneredProviderIds);
            return Ok(products);
        }

        [HttpPost("{id}/generate-quote")]
        public async Task<ActionResult<Quote>> GenerateQuote(string id, QuoteRequest request)
        {
            request.BrokerId = id;
            var quote = await _quoteService.CreateQuoteAsync(request.FleetManagerId, request);
            return CreatedAtAction("GetQuote", "Quotes", new { id = quote.Id }, quote);
        }

        [HttpPost("{id}/bind-policy")]
        public async Task<ActionResult<InsurancePolicy>> BindPolicy(string id, BindPolicyRequest request)
        {
            var policy = await _policyService.BindPolicyFromQuoteAsync(request.QuoteId, id);
            if (policy == null)
            {
                return BadRequest("Unable to bind policy from quote");
            }
            return CreatedAtAction("GetPolicy", "Policies", new { id = policy.Id }, policy);
        }

        [HttpGet("{id}/commission-report")]
        public async Task<ActionResult<CommissionReport>> GetCommissionReport(string id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var report = await _policyService.GetBrokerCommissionReportAsync(id, startDate, endDate);
            return Ok(report);
        }
    }

    public class BindPolicyRequest
    {
        public string QuoteId { get; set; } = string.Empty;
        public DateTime EffectiveDate { get; set; }
        public BillingFrequency BillingFrequency { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }

    public class CommissionReport
    {
        public string BrokerId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCommissionEarned { get; set; }
        public int PoliciesSold { get; set; }
        public decimal AverageCommissionPerPolicy { get; set; }
        public List<PolicyCommission> PolicyCommissions { get; set; } = new();
    }

    public class PolicyCommission
    {
        public string PolicyId { get; set; } = string.Empty;
        public string PolicyNumber { get; set; } = string.Empty;
        public decimal PremiumAmount { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal CommissionAmount { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}