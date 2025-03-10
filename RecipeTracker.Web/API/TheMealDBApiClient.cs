using RecipeTracker.Web.API.Models.Interfaces;
using RecipeTracker.Web.API.Models.Responses;

namespace RecipeTracker.Web.API
{
    public class TheMealDbApiClient(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<TheMealDbApiClient> logger,
        IApiResponseDeserializer responseDeserializer)
    {
        private readonly string _baseUrl = configuration["MealDbApi:BaseUrl"]
                                           ?? throw new ArgumentNullException(nameof(configuration), "MealDbApi:BaseUrl not found in configuration.");

        // Primary constructor to inject dependencies
        // Use the parameter name directly in the exception constructor


        public async Task<ApiResponse?> GetRecipesAsync(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                logger.LogWarning("Search query is empty or null.");
                return null;
            }

            var url = $"{_baseUrl}search.php?s={query}";

            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Request failed with status code: {response.StatusCode}. URL: {url}");
                return null;
            }

            return await responseDeserializer.DeserializeResponseAsync(response);
        }
    }
}