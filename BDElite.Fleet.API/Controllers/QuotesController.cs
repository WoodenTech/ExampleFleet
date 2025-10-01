using BDElite.Fleet.API.Models;
using BDElite.Fleet.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BDElite.Fleet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotesController : ControllerBase
    {
        private readonly IQuoteService _quoteService;

        public QuotesController(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Quote>>> GetAllQuotes()
        {
            var quotes = await _quoteService.GetAllQuotesAsync();
            return Ok(quotes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Quote>> GetQuote(string id)
        {
            var quote = await _quoteService.GetQuoteByIdAsync(id);
            if (quote == null)
            {
                return NotFound();
            }
            return Ok(quote);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuote(string id, Quote quote)
        {
            var updated = await _quoteService.UpdateQuoteAsync(id, quote);
            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuote(string id)
        {
            var deleted = await _quoteService.DeleteQuoteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptQuote(string id)
        {
            var accepted = await _quoteService.AcceptQuoteAsync(id);
            if (!accepted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("{id}/decline")]
        public async Task<IActionResult> DeclineQuote(string id, [FromBody] string reason)
        {
            var declined = await _quoteService.DeclineQuoteAsync(id, reason);
            if (!declined)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<Quote>>> GetQuotesByStatus(QuoteStatus status)
        {
            var quotes = await _quoteService.GetQuotesByStatusAsync(status);
            return Ok(quotes);
        }
    }
}