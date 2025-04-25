using Microsoft.AspNetCore.Components;
using RecipeTracker.ApiService.API;
using RecipeTracker.ApiService.Models.Internal;
using RecipeTracker.ApiService.Translations;

namespace RecipeTracker.Web.Components.Pages;

public class FoodBase : ComponentBase
{
    // Inject dependencies for API client and translation cache holder
    [Inject] protected RecipesApiClient RecipesApi { get; set; } = null!;
    [Inject] protected TranslationCacheHolder CacheHolder { get; set; } = null!;

    // Query for searching recipes
    protected string Query = string.Empty;

    // List of fetched recipes
    protected List<RecipeModel> Recipes = [];

    // Error message, if any
    protected string? ErrorMessage;

    // Loading state flag
    protected bool IsLoading;

    // Property for translations. Will gracefully fall back to displaying the key if no translations are available.
    protected IReadOnlyDictionary<string, string> t =>
        CacheHolder.Cache.TryGetValue("en", out var translations) ? translations : new Dictionary<string, string>();

    // Called after the component is first rendered. Trigger the search operation.
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Only perform the search after the first render
            await SearchRecipes(Query);
        }
    }

    // Searches for recipes and handles loading and error states.
    protected async Task SearchRecipes(string query)
    {
        if (IsLoading) return; // Prevent multiple simultaneous searches

        IsLoading = true; // Set loading flag
        ErrorMessage = null; // Clear previous error message

        try
        {
            // Fetch recipes based on the search query
            var results = await RecipesApi.SearchAsync(query);
            Recipes = results; // Store the results

            // If no results, display the "No recipes found" message or fallback to the key
            ErrorMessage = results.Count == 0 ? t.GetValueOrDefault("Food.NoRecipesFoundMessage", "Food.NoRecipesFoundMessage") : null;
        }
        catch
        {
            // On error, set the error message or fallback to the key
            ErrorMessage = t.GetValueOrDefault("Food.ErrorOccurred", "Food.ErrorOccurred");
        }
        finally
        {
            // Reset loading flag after completion
            IsLoading = false;
        }
    }
}
