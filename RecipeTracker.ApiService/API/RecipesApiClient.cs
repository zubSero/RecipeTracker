using System.Net;
using RecipeTracker.ApiService.Models.Internal;

namespace RecipeTracker.ApiService.API;

public class RecipesApiClient(HttpClient client)
{
    public async Task<List<RecipeModel>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return [];  // Return empty list for empty query

        var url = $"/api/recipes?query={Uri.EscapeDataString(query)}";
        Console.WriteLine($"API Request URL: {url}");

        try
        {
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                // If successful, parse the response
                var result = await response.Content.ReadFromJsonAsync<List<RecipeModel>>();
                return result ?? []; // Return empty list if null
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // If 404, no recipes found; return empty list
                Console.WriteLine("No recipes found for the query.");
                return [];
            }
            else
            {
                // Handle other errors (e.g., 500 Internal Server Error)
                await Console.Error.WriteLineAsync($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                throw new Exception($"Error fetching recipes: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            // Log unexpected errors
            await Console.Error.WriteLineAsync($"Unexpected error: {ex.Message}");
            throw new Exception("There was an issue contacting the server. Please try again later.");
        }
    }
}