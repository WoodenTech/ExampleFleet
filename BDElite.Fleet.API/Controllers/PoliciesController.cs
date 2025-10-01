using BDElite.Fleet.API.Models;
using BDElite.Fleet.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BDElite.Fleet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PoliciesController : ControllerBase
    {
        private readonly IPolicyService _policyService;

        public PoliciesController(IPolicyService policyService)
        {
            _policyService = policyService;
        }

        [HttpGet]
        public async Task<ActionResult<List<InsurancePolicy>>> GetAllPolicies()
        {
            var policies = await _policyService.GetAllPoliciesAsync();
            return Ok(policies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InsurancePolicy>> GetPolicy(string id)
        {
            var policy = await _policyService.GetPolicyByIdAsync(id);
            if (policy == null)
            {
                return NotFound();
            }
            return Ok(policy);
        }

        [HttpPost]
        public async Task<ActionResult<InsurancePolicy>> CreatePolicy(InsurancePolicy policy)
        {
            var created = await _policyService.CreatePolicyAsync(policy);
            return CreatedAtAction(nameof(GetPolicy), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePolicy(string id, InsurancePolicy policy)
        {
            var updated = await _policyService.UpdatePolicyAsync(id, policy);
            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePolicy(string id)
        {
            var deleted = await _policyService.DeletePolicyAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<InsurancePolicy>>> GetPoliciesByStatus(PolicyStatus status)
        {
            var policies = await _policyService.GetPoliciesByStatusAsync(status);
            return Ok(policies);
        }
    }
}