using System.Text.Json;

namespace RecipeTracker.Web.API.Models
{
    public class MealDetails
    {
        // Dictionaries to store ingredients and their corresponding measures
        public Dictionary<int, string> Ingredients { get; set; } = new();
        public Dictionary<int, string> Measures { get; set; } = new();

        // Populate ingredients and measures dynamically from JSON
        public void InitializeIngredientsAndMeasures(JsonElement json)
        {
            const int maxIngredients = 20; // Maximum number of ingredients supported by the API

            for (var i = 1; i <= maxIngredients; i++)
            {
                var ingredientField = $"strIngredient{i}";
                var measureField = $"strMeasure{i}";

                var ingredient = json.TryGetProperty(ingredientField, out var ingElement) ? ingElement.GetString() : null;
                var measure = json.TryGetProperty(measureField, out var meaElement) ? meaElement.GetString() : null;

                if (!string.IsNullOrWhiteSpace(ingredient))
                {
                    Ingredients[i] = ingredient;
                }

                if (!string.IsNullOrWhiteSpace(measure))
                {
                    Measures[i] = measure;
                }
            }
        }
    }
}