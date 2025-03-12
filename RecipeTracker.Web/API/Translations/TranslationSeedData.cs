using RecipeTracker.Web.API.Translations;

public static class TranslationSeedData
{
    public static IEnumerable<Translation> GetSeedData() =>
        new List<Translation>
        {
            new Translation { Id = 1, LanguageCode = "en", Key = "MealDbApi.BaseUrl.Missing", Value = "MealDbApi:BaseUrl not found in configuration." },
            new Translation { Id = 2, LanguageCode = "en", Key = "Search.Query.Empty", Value = "Search query is empty or null." },
            new Translation { Id = 3, LanguageCode = "en", Key = "Request.Failed", Value = "Request failed with status code: {0}. URL: {1}" },
            new Translation { Id = 4, LanguageCode = "en", Key = "Food.PageTitle", Value = "Food Search" },
            new Translation { Id = 5, LanguageCode = "en", Key = "Food.SearchPrompt", Value = "Enter a recipe name" },
            new Translation { Id = 6, LanguageCode = "en", Key = "Food.SearchButton", Value = "Search" },
            new Translation { Id = 7, LanguageCode = "en", Key = "Food.Loading", Value = "Loading..." },
            new Translation { Id = 8, LanguageCode = "en", Key = "Food.NoRecipesFound", Value = "No recipes found." },
            new Translation { Id = 9, LanguageCode = "en", Key = "Food.UnknownMeal", Value = "Unknown Meal" },
            new Translation { Id = 10, LanguageCode = "en", Key = "Food.Category", Value = "Category:" },
            new Translation { Id = 11, LanguageCode = "en", Key = "Food.Unknown", Value = "Unknown" },
            new Translation { Id = 12, LanguageCode = "en", Key = "Food.Area", Value = "Area:" },
            new Translation { Id = 13, LanguageCode = "en", Key = "Food.Instructions", Value = "Instructions:" },
            new Translation { Id = 14, LanguageCode = "en", Key = "Food.ShowMore", Value = "Show More" },
            new Translation { Id = 15, LanguageCode = "en", Key = "Food.ShowLess", Value = "Show Less" },
            new Translation { Id = 16, LanguageCode = "en", Key = "Food.Ingredients", Value = "Ingredients:" },
            new Translation { Id = 17, LanguageCode = "en", Key = "Food.WatchOnYoutube", Value = "Watch on YouTube" }
        };
}