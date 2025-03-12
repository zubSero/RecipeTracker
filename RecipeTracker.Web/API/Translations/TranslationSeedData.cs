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
            new() { Id = 5, LanguageCode = "en", Key = "Food.SearchPrompt", Value = "Enter a recipe name" },
            new() { Id = 6, LanguageCode = "en", Key = "Food.SearchButton", Value = "Search" },
            new() { Id = 7, LanguageCode = "en", Key = "Food.Loading", Value = "Loading..." },
            new() { Id = 8, LanguageCode = "en", Key = "Food.NoRecipesFound", Value = "No recipes found." },
            new() { Id = 9, LanguageCode = "en", Key = "Food.UnknownMeal", Value = "Unknown Meal" },
            new() { Id = 10, LanguageCode = "en", Key = "Food.Category", Value = "Category:" },
            new() { Id = 11, LanguageCode = "en", Key = "Food.Unknown", Value = "Unknown" },
            new() { Id = 12, LanguageCode = "en", Key = "Food.Area", Value = "Area:" },
            new() { Id = 13, LanguageCode = "en", Key = "Food.Instructions", Value = "Instructions:" },
            new() { Id = 14, LanguageCode = "en", Key = "Food.ShowMore", Value = "Show More" },
            new() { Id = 15, LanguageCode = "en", Key = "Food.ShowLess", Value = "Show Less" },
            new() { Id = 16, LanguageCode = "en", Key = "Food.Ingredients", Value = "Ingredients:" },
            new() { Id = 17, LanguageCode = "en", Key = "Food.WatchOnYouTube", Value = "Watch on YouTube" }
        };
        context.AddRange(products);
        context.SaveChanges();
    }
}