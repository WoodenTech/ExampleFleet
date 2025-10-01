using BDElite.Fleet.API.Models;
using BDElite.Fleet.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BDElite.Fleet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FleetManagerController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPolicyService _policyService;
        private readonly IQuoteService _quoteService;

        public FleetManagerController(
            IUserService userService,
            IPolicyService policyService,
            IQuoteService quoteService)
        {
            _userService = userService;
            _policyService = policyService;
            _quoteService = quoteService;
        }

        [HttpGet]
        public async Task<ActionResult<List<FleetManager>>> GetAllFleetManagers()
        {
            var fleetManagers = await _userService.GetFleetManagersAsync();
            return Ok(fleetManagers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FleetManager>> GetFleetManager(string id)
        {
            var fleetManager = await _userService.GetFleetManagerByIdAsync(id);
            if (fleetManager == null)
            {
                return NotFound();
            }
            return Ok(fleetManager);
        }

        [HttpPost]
        public async Task<ActionResult<FleetManager>> CreateFleetManager(FleetManager fleetManager)
        {
            var created = await _userService.CreateFleetManagerAsync(fleetManager);
            return CreatedAtAction(nameof(GetFleetManager), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFleetManager(string id, FleetManager fleetManager)
        {
            var updated = await _userService.UpdateFleetManagerAsync(id, fleetManager);
            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("{id}/policies")]
        public async Task<ActionResult<List<InsurancePolicy>>> GetFleetManagerPolicies(string id)
        {
            var policies = await _policyService.GetPoliciesByFleetManagerIdAsync(id);
            return Ok(policies);
        }

        [HttpGet("{id}/quotes")]
        public async Task<ActionResult<List<Quote>>> GetFleetManagerQuotes(string id)
        {
            var quotes = await _quoteService.GetQuotesByFleetManagerIdAsync(id);
            return Ok(quotes);
        }

        [HttpPost("{id}/request-quote")]
        public async Task<ActionResult<Quote>> RequestQuote(string id, QuoteRequest request)
        {
            var quote = await _quoteService.CreateQuoteAsync(id, request);
            return CreatedAtAction("GetQuote", "Quotes", new { id = quote.Id }, quote);
        }
    }

    public class QuoteRequest
    {
        public string FleetManagerId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public List<string> VehicleIds { get; set; } = new();
        public List<Coverage> RequestedCoverages { get; set; } = new();
        public string BrokerId { get; set; } = string.Empty;
    }
}