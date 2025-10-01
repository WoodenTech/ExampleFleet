using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BDElite.Fleet.UI.Models;
using System.Text;
using System.Text.Json;

namespace BDElite.Fleet.UI.Pages
{
    public class GenerateQuoteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GenerateQuoteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public string CompanyName { get; set; } = string.Empty;

        [BindProperty]
        public string ContactName { get; set; } = string.Empty;

        [BindProperty]
        public string ContactEmail { get; set; } = string.Empty;

        [BindProperty]
        public string ContactPhone { get; set; } = string.Empty;

        [BindProperty]
        public string SelectedProductId { get; set; } = string.Empty;

        [BindProperty]
        public List<VehicleQuoteDto> Vehicles { get; set; } = new List<VehicleQuoteDto>();

        [BindProperty]
        public List<string> SelectedCoverages { get; set; } = new List<string>();

        public List<InsuranceProductDto> AvailableProducts { get; set; } = new List<InsuranceProductDto>();
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            await LoadAvailableProducts();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadAvailableProducts();
                return Page();
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("BDEliteFleetAPI");

                // For now, we'll use default IDs - in a real app these would come from user session
                var brokerId = "6752a1b2e9f3c4d8a7b1c2d4";
                var fleetManagerId = "6752a1b2e9f3c4d8a7b1c2d5"; // Default fleet manager

                // Create quote request that matches the API structure
                var quoteRequest = new ApiQuoteRequest
                {
                    FleetManagerId = fleetManagerId,
                    ProductId = SelectedProductId,
                    VehicleIds = new List<string>(), // We'll create vehicles on the fly
                    RequestedCoverages = SelectedCoverages.Select(c => new CoverageDto
                    {
                        Type = MapToCoverageType(c),
                        Limit = 100000, // Default values
                        Deductible = 500
                    }).ToList(),
                    BrokerId = brokerId
                };

                var json = JsonSerializer.Serialize(quoteRequest, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"api/InsuranceBroker/{brokerId}/generate-quote", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var quote = JsonSerializer.Deserialize<QuoteResponseDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    TempData["SuccessMessage"] = $"Quote generated successfully! Quote Number: {quote?.QuoteNumber}";
                    TempData["QuoteData"] = responseContent;
                    return RedirectToPage("/QuoteDetails", new { quoteId = quote?.Id });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Failed to generate quote: {response.StatusCode} - {errorContent}");
                    await LoadAvailableProducts();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while generating the quote: {ex.Message}");
                await LoadAvailableProducts();
                return Page();
            }
        }

        private async Task LoadAvailableProducts()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("BDEliteFleetAPI");

                // Get all active products from all providers
                var response = await httpClient.GetAsync("api/Products/active");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    AvailableProducts = JsonSerializer.Deserialize<List<InsuranceProductDto>>(jsonContent, options) ?? new List<InsuranceProductDto>();
                }
                else
                {
                    ErrorMessage = $"Failed to load products: {response.StatusCode}";
                    AvailableProducts = new List<InsuranceProductDto>();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading products: {ex.Message}";
                AvailableProducts = new List<InsuranceProductDto>();
            }
        }

        private CoverageTypeDtoApi MapToCoverageType(string coverageType)
        {
            return coverageType switch
            {
                "ThirdParty" => CoverageTypeDtoApi.Liability,
                "Comprehensive" => CoverageTypeDtoApi.Comprehensive,
                "Collision" => CoverageTypeDtoApi.Collision,
                "Cargo" => CoverageTypeDtoApi.CargoInsurance,
                _ => CoverageTypeDtoApi.Liability
            };
        }
    }

    public class VehicleQuoteDto
    {
        public string VehicleType { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public string VIN { get; set; } = string.Empty;
    }

    public class CreateQuoteRequest
    {
        public string CompanyName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public List<VehicleQuoteDto> Vehicles { get; set; } = new List<VehicleQuoteDto>();
        public List<string> SelectedCoverages { get; set; } = new List<string>();
    }

    public class ApiQuoteRequest
    {
        public string FleetManagerId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public List<string> VehicleIds { get; set; } = new();
        public List<CoverageDto> RequestedCoverages { get; set; } = new();
        public string BrokerId { get; set; } = string.Empty;
    }

    public class CoverageDto
    {
        public CoverageTypeDtoApi Type { get; set; }
        public decimal Limit { get; set; }
        public decimal Deductible { get; set; }
    }

    public enum CoverageTypeDtoApi
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

    public class QuoteResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string QuoteNumber { get; set; } = string.Empty;
        public string FleetManagerId { get; set; } = string.Empty;
        public string BrokerId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public List<string> VehicleIds { get; set; } = new();
        public decimal BasePremium { get; set; }
        public decimal BrokerMarkup { get; set; }
        public decimal TotalPremium { get; set; }
        public DateTime ValidUntil { get; set; }
        public QuoteStatusDto Status { get; set; } = QuoteStatusDto.Pending;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public enum QuoteStatusDto
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