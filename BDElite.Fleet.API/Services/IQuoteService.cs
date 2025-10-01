using BDElite.Fleet.API.Models;
using BDElite.Fleet.API.Controllers;

namespace BDElite.Fleet.API.Services
{
    public interface IQuoteService
    {
        Task<List<Quote>> GetAllQuotesAsync();
        Task<Quote?> GetQuoteByIdAsync(string id);
        Task<Quote> CreateQuoteAsync(string fleetManagerId, QuoteRequest request);
        Task<bool> UpdateQuoteAsync(string id, Quote quote);
        Task<bool> DeleteQuoteAsync(string id);

        Task<List<Quote>> GetQuotesByFleetManagerIdAsync(string fleetManagerId);
        Task<List<Quote>> GetQuotesByBrokerIdAsync(string brokerId);
        Task<List<Quote>> GetQuotesByStatusAsync(QuoteStatus status);

        Task<Quote> CalculateQuoteAsync(string productId, List<string> vehicleIds, List<Coverage> coverages);
        Task<bool> AcceptQuoteAsync(string quoteId);
        Task<bool> DeclineQuoteAsync(string quoteId, string reason);
        Task<bool> ExpireQuoteAsync(string quoteId);
    }
}