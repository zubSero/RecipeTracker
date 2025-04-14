using RecipeTracker.ApiService.Models.Internal;

namespace RecipeTracker.ApiService.API;

public class RecipesApiClient(HttpClient client)
{
    public async Task<List<RecipeModel>> SearchAsync(string query)
    {
        var url = $"/api/recipes?query={Uri.EscapeDataString(query)}";
        var result = await client.GetFromJsonAsync<List<RecipeModel>>(url);
        return result ?? [];
    }
}