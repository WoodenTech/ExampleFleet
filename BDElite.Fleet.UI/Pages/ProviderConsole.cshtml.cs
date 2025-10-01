using Microsoft.AspNetCore.Mvc.RazorPages;
using BDElite.Fleet.UI.Models;
using System.Text.Json;

namespace BDElite.Fleet.UI.Pages
{
    public class ProviderConsoleModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProviderConsoleModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<InsuranceProductDto> Products { get; set; } = new();
        public int ActiveProductCount { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("BDEliteFleetAPI");

                // For now, we'll use a default provider ID
                var providerId = "6752a1b2e9f3c4d8a7b1c2d3"; // This should come from user session in real app

                var response = await httpClient.GetAsync($"api/InsuranceProvider/{providerId}/products");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    Products = JsonSerializer.Deserialize<List<InsuranceProductDto>>(jsonContent, options) ?? new List<InsuranceProductDto>();
                    ActiveProductCount = Products.Count(p => p.IsActive);
                }
                else
                {
                    ErrorMessage = $"Failed to load products: {response.StatusCode}";
                    Products = new List<InsuranceProductDto>();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading products: {ex.Message}";
                Products = new List<InsuranceProductDto>();
            }
        }
    }
}