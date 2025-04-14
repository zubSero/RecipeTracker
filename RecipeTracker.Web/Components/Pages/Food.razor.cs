using Microsoft.AspNetCore.Components;
using RecipeTracker.ApiService.API;
using RecipeTracker.ApiService.Models.Internal;

namespace RecipeTracker.Web.Components.Pages;

public class FoodBase : ComponentBase
{
    [Inject] protected RecipesApiClient RecipesApi { get; set; } = null!;
    [Inject] protected Dictionary<string, Dictionary<string, string>> TranslationsCache { get; set; } = null!;

    protected string Query = string.Empty;
    protected List<RecipeModel> Recipes = [];
    protected string? ErrorMessage;
    protected bool IsLoading;

    protected IReadOnlyDictionary<string, string> t => TranslationsCache["en"];

    protected async Task SearchRecipes(string query)
    {
        if (IsLoading) return;

        IsLoading = true;
        StateHasChanged();

        try
        {
            var results = await RecipesApi.SearchAsync(query);
            Recipes = results;
            ErrorMessage = results.Count == 0 ? t["Food.NoRecipesFoundMessage"] : null;
        }
        catch (Exception ex)
        {
            ErrorMessage = t["Food.ErrorOccurred"];
            Console.Error.WriteLine(ex);
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }
}