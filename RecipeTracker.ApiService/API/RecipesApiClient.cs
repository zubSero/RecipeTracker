using System.Net;
using RecipeTracker.ApiService.Models.Internal;
using RecipeTracker.ApiService.Translations;

namespace RecipeTracker.ApiService.API;

public class RecipesApiClient
{
    private readonly HttpClient _client;
    private readonly TranslationCacheHolder _cacheHolder;

    public RecipesApiClient(HttpClient client, TranslationCacheHolder cacheHolder)
    {
        _client = client;
        _cacheHolder = cacheHolder;
    }

    private IReadOnlyDictionary<string, string> GetTranslations(string languageCode)
    {
        return _cacheHolder.Cache.TryGetValue(languageCode, out var translations) && translations != null
            ? translations
            : new Dictionary<string, string>
            {
                ["Error.FetchingRecipes"] = "Error fetching recipes. Please try again later."
            };
    }

    public async Task<List<RecipeModel>> SearchAsync(string query, string languageCode = "en")
    {
        if (string.IsNullOrWhiteSpace(query)) return new List<RecipeModel>();  // Return empty list for empty query

        var url = $"/api/recipes?query={Uri.EscapeDataString(query)}";
        Console.WriteLine($"API Request URL: {url}");

        try
        {
            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<RecipeModel>>();
                return result ?? new List<RecipeModel>(); // Return empty list if null
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("No recipes found for the query.");
                return new List<RecipeModel>(); // Return empty list if not found
            }
            else
            {
                var translations = GetTranslations(languageCode);
                var errorMessage = translations.GetValueOrDefault("Error.FetchingRecipes");
                await Console.Error.WriteLineAsync($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                throw new Exception(errorMessage);
            }
        }
        catch (Exception ex)
        {
            var translations = GetTranslations(languageCode);
            var errorMessage = translations.GetValueOrDefault("Error.FetchingRecipes");
            await Console.Error.WriteLineAsync($"Unexpected error: {ex.Message}");
            throw new Exception(errorMessage);
        }
    }
}
