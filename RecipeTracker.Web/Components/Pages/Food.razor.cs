using Microsoft.AspNetCore.Components;
using RecipeTracker.ApiService.API;
using RecipeTracker.ApiService.Models.Internal;

namespace RecipeTracker.Web.Components.Pages;

public class FoodBase : ComponentBase
{
    [Inject] protected RecipesApiClient RecipesApi { get; set; } = null!;
    [Inject] protected Dictionary<string, Dictionary<string, string>> TranslationsCache { get; set; } = null!;

    protected string _query = string.Empty;
    protected List<RecipeModel> _recipes = new();
    protected string? _errorMessage;
    protected bool _isLoading;

    protected IReadOnlyDictionary<string, string> t => TranslationsCache["en"];

    protected async Task SearchRecipes(string query)
    {
        if (_isLoading) return;

        _isLoading = true;
        StateHasChanged();

        try
        {
            var results = await RecipesApi.SearchAsync(query);
            _recipes = results;
            _errorMessage = results.Count == 0 ? t["Food.NoRecipesFoundMessage"] : null;
        }
        catch (Exception ex)
        {
            _errorMessage = t["Food.ErrorOccurred"];
            Console.Error.WriteLine(ex);
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }
}