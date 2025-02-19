using System.Text.Json;
using RecipeTracker.Web.Components.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace RecipeTracker.Web
{
    public class TheMealDbApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ILogger<TheMealDbApiClient> _logger;

        // Constructor to inject HttpClient, configuration for baseUrl, and logger
        public TheMealDbApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<TheMealDbApiClient> logger)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["MealDbApi:BaseUrl"];  // Fetch base URL from appsettings.json
            _logger = logger;
        }

        // Async method to get recipes based on a search query
        public async Task<ApiResponse?> GetRecipesAsync(string query)
        {
            var url = $"{_baseUrl}search.php?s={query}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                // Only proceed if request is successful
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Request failed with status code: {response.StatusCode}");
                    return null;
                }

                // Deserialize the JSON response directly
                return await DeserializeResponseAsync(response);
            }
            catch (Exception ex)
            {
                // Log any exception that occurs
                _logger.LogError(ex, "Error occurred while fetching recipes");
                return null;
            }
        }

        // Helper method to handle the deserialization
        private static async Task<ApiResponse?> DeserializeResponseAsync(HttpResponseMessage response)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse>(jsonResponse);
        }
    }
}
