using RecipeTracker.Web.API.Models.Interfaces;
using RecipeTracker.Web.API.Models.Responses;
using RecipeTracker.Web.API.Translations.Interface;

namespace RecipeTracker.Web.API
{
    public class TheMealDbApiClient(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<TheMealDbApiClient> logger,
        IApiResponseDeserializer responseDeserializer,
        ITranslationService translationService
        )
    {
        private readonly string _baseUrl = configuration["MealDbApi:BaseUrl"]
                                           ?? throw new ArgumentNullException(nameof(configuration),
                                               translationService.GetTranslationAsync("MealDbApi.BaseUrl.Missing", "en").Result);

        // Primary constructor to inject dependencies
        // Use the parameter name directly in the exception constructor


        public async Task<ApiResponse?> GetRecipesAsync(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                logger.LogWarning(await translationService.GetTranslationAsync("Search.Query.Empty", "en"));
                return null;
            }

            var url = $"{_baseUrl}search.php?s={query}";

            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode) return await responseDeserializer.DeserializeResponseAsync(response);

            var s = await translationService.GetTranslationAsync("Request.Failed", "en") ?? string.Empty;

            logger.LogError(string.Format(
                s,
                response.StatusCode, url));

            return null;
        }

    }
}