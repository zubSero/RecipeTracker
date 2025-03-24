using RecipeTracker.ApiService.Service.External.Interfaces;
using RecipeTracker.ApiService.Service.External.Responses;

namespace RecipeTracker.ApiService.Service.External
{
    public class TheMealDbApiClient(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<TheMealDbApiClient> logger,
        IApiResponseDeserializer responseDeserializer)
    {
        private readonly string _baseUrl = configuration["MealDbApi:BaseUrl"]
                                           ?? throw new ArgumentNullException(nameof(configuration),
                                               "MealDbApi BaseUrl is missing in configuration.");

        public async Task<ApiResponse?> GetRecipesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                logger.LogWarning("Search query is empty.");
                return null;
            }

            var url = $"{_baseUrl}search.php?s={query}";

            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await responseDeserializer.DeserializeResponseAsync(response);
            }

            logger.LogError("Request failed with status code {StatusCode} for URL: {Url}",
                response.StatusCode, url);

            return null;
        }
    }
}