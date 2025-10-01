using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BDElite.Fleet.UI.Models;
using System.Text;
using System.Text.Json;

namespace BDElite.Fleet.UI.Pages
{
    public class AddProductModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AddProductModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [BindProperty]
        public string ProductName { get; set; } = string.Empty;

        [BindProperty]
        public string ProductCode { get; set; } = string.Empty;

        [BindProperty]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public string PricingModel { get; set; } = "PerPolicy";

        [BindProperty]
        public bool AdjustForAge { get; set; }

        [BindProperty]
        public bool AdjustForWeight { get; set; }

        [BindProperty]
        public bool AdjustForType { get; set; }

        [BindProperty]
        public decimal BasePrice { get; set; }

        [BindProperty]
        public decimal IPT { get; set; }

        [BindProperty]
        public decimal ProviderCommission { get; set; }

        [BindProperty]
        public decimal MaxCoverage { get; set; }

        [BindProperty]
        public decimal Deductible { get; set; }

        [BindProperty]
        public List<string> CoverageTypes { get; set; } = new List<string>();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Create the product DTO
                var productDto = new InsuranceProductDto
                {
                    Name = ProductName,
                    ProductCode = ProductCode,
                    Description = Description,
                    BaseRate = BasePrice,
                    BrokerMarkupPercentage = ProviderCommission,
                    SupportedVehicleTypes = new List<VehicleTypeDto> { VehicleTypeDto.Car, VehicleTypeDto.Truck, VehicleTypeDto.Van },
                    SupportedIndustryTypes = new List<string> { "Logistics", "Construction", "Food Service" },
                    MinimumFleetSize = 1,
                    MaximumFleetSize = 1000,
                    IsActive = true,
                    EffectiveDate = DateTime.UtcNow
                };

                // Map coverage types from form to coverage options
                if (CoverageTypes.Any())
                {
                    productDto.CoverageOptions = CoverageTypes.Select(ct => new CoverageOptionDto
                    {
                        Type = Enum.Parse<CoverageTypeDto>(ct),
                        Name = ct,
                        Description = $"{ct} coverage",
                        LimitOptions = new List<decimal> { MaxCoverage },
                        DeductibleOptions = new List<decimal> { Deductible },
                        IsRequired = ct == "ThirdParty",
                        BaseRate = BasePrice
                    }).ToList();
                }

                // Add rating factors based on vehicle adjustments
                if (PricingModel == "PerVehicle")
                {
                    if (AdjustForAge)
                    {
                        productDto.RatingFactors.Add(new RatingFactorDto
                        {
                            Name = "Vehicle Age",
                            Category = RatingCategoryDto.VehicleAge,
                            Multiplier = 1.1m,
                            Description = "Adjustment based on vehicle age"
                        });
                    }
                    if (AdjustForWeight)
                    {
                        productDto.RatingFactors.Add(new RatingFactorDto
                        {
                            Name = "Vehicle Weight",
                            Category = RatingCategoryDto.Usage,
                            Multiplier = 1.05m,
                            Description = "Adjustment based on vehicle weight"
                        });
                    }
                    if (AdjustForType)
                    {
                        productDto.RatingFactors.Add(new RatingFactorDto
                        {
                            Name = "Vehicle Type",
                            Category = RatingCategoryDto.Usage,
                            Multiplier = 1.15m,
                            Description = "Adjustment based on vehicle type"
                        });
                    }
                }

                // Call the API
                var httpClient = _httpClientFactory.CreateClient("BDEliteFleetAPI");

                // For now, we'll use a default provider ID
                var providerId = "6752a1b2e9f3c4d8a7b1c2d3"; // This should come from user session in real app

                var json = JsonSerializer.Serialize(productDto, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"api/InsuranceProvider/{providerId}/products", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Product '{ProductName}' created successfully!";
                    return RedirectToPage("/ProviderConsole");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Failed to create product: {response.StatusCode} - {errorContent}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while creating the product: {ex.Message}");
                return Page();
            }
        }
    }
}