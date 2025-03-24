    using System.Text.Json;
    using RecipeTracker.ApiService.Models.Internal;
    using RecipeTracker.ApiService.Service.External;
    // Your external model namespace

    namespace RecipeTracker.ApiService.Service.Internal
    {
        public interface IRecipeService
        {
            Task<IEnumerable<RecipeModel>> GetRecipesAsync(string query);
        }

        public class RecipeService(TheMealDbApiClient mealDbApiClient, ILogger<RecipeService> logger)
            : IRecipeService
        {
            public async Task<IEnumerable<RecipeModel>> GetRecipesAsync(string query)
            {
                // Call the external API
                var response = await mealDbApiClient.GetRecipesAsync(query);
                if (response?.Meals == null)
                {
                    logger.LogWarning("No recipes returned from the external service.");
                    return [];
                }

                // Map each Meal into our RecipeModel.
                var recipes = response.Meals.Select(meal =>
                {
                    // Ensure MealDetails has been populated.
                    // Here we serialize the meal into a JsonElement and initialize the details.
                    // (Alternatively, you could perform the initialization as a part of custom deserialization.)
                    var jsonElement = JsonSerializer.SerializeToElement(meal);
                    meal.InitializeDetailsFromJson(jsonElement);

                    var recipe = new RecipeModel
                    {
                        Id = meal.IdMeal ?? string.Empty,
                        Name = meal.StrMeal ?? string.Empty,
                        Instructions = meal.StrInstructions ?? string.Empty,
                        Thumbnail = meal.StrMealThumb ?? string.Empty,
                        Category = meal.StrCategory ?? string.Empty,
                        Area = meal.StrArea ?? string.Empty,
                        Youtube = meal.StrYoutube ?? string.Empty
                    };

                    // Use the MealDetails dictionaries to build a list of ingredients.
                    foreach (var ingredientEntry in meal.MealDetails.Ingredients)
                    {
                        // Look up the corresponding measure using the same key.
                        meal.MealDetails.Measures.TryGetValue(ingredientEntry.Key, out var measure);

                        recipe.Ingredients.Add(new IngredientMeasure
                        {
                            Ingredient = ingredientEntry.Value.Trim(),
                            Measure = measure?.Trim() ?? string.Empty
                        });
                    }

                    return recipe;
                });

                return recipes;
            }
        }
    }
