namespace RecipeTracker.Web.API.Translations;

public static class TranslationSeedData
{
    public static void Initialize(TranslationDbContext context)
    {
        if (context.Translations.Any())
            return;

        var products = new List<Translation>
        {
            new() { Id = 1, LanguageCode = "en", Key = "MealDbApi.BaseUrl.Missing", Value = "MealDbApi:BaseUrl not found in configuration." },
            new() { Id = 2, LanguageCode = "en", Key = "Search.Query.Empty", Value = "Search query is empty or null." },
            new() { Id = 3, LanguageCode = "en", Key = "Request.Failed", Value = "Request failed with status code: {0}. URL: {1}" },
            new() { Id = 4, LanguageCode = "en", Key = "Food.PageTitle", Value = "Food Search" },
            new() { Id = 5, LanguageCode = "en", Key = "SearchBar.SearchPrompt", Value = "Enter a recipe name" },
            new() { Id = 6, LanguageCode = "en", Key = "SearchBar.SearchButton", Value = "Search" },
            new() { Id = 7, LanguageCode = "en", Key = "SearchBar.SearchLabel", Value = "Search input for recipes" },
            new() { Id = 8, LanguageCode = "en", Key = "SearchBar.Loading", Value = "Loading search results..." },
            new() { Id = 9, LanguageCode = "en", Key = "SearchBar.SearchButtonLabel", Value = "Trigger recipe search" },
            new() { Id = 10, LanguageCode = "en", Key = "Food.LoadingState", Value = "Loading..." },
            new() { Id = 11, LanguageCode = "en", Key = "Food.NoRecipesFoundMessage", Value = "No recipes found." },
            new() { Id = 12, LanguageCode = "en", Key = "Food.UnknownMealName", Value = "Unknown Meal" },
            new() { Id = 13, LanguageCode = "en", Key = "Food.MealCategory", Value = "Category:" },
            new() { Id = 14, LanguageCode = "en", Key = "Food.UnknownCategory", Value = "Unknown" },
            new() { Id = 15, LanguageCode = "en", Key = "Food.MealArea", Value = "Area:" },
            new() { Id = 16, LanguageCode = "en", Key = "Food.NoInstructionsAvailable", Value = "No instructions available." },
            new() { Id = 17, LanguageCode = "en", Key = "Food.ShowMoreInstructions", Value = "Show More" },
            new() { Id = 18, LanguageCode = "en", Key = "Food.ShowLessInstructions", Value = "Show Less" },
            new() { Id = 19, LanguageCode = "en", Key = "Food.IngredientsSection", Value = "Ingredients:" },
            new() { Id = 20, LanguageCode = "en", Key = "Food.WatchOnYouTubeLink", Value = "Watch on YouTube" },
            new() { Id = 21, LanguageCode = "en", Key = "Food.ErrorOccurred", Value = "An error has occurred!" }
        };
        context.AddRange(products);
        context.SaveChanges();
    }
}
