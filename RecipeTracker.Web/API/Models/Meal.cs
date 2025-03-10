using System.Text.Json.Serialization;

namespace RecipeTracker.Web.API.Models;

public class Meal
{
    [JsonPropertyName("idMeal")]
    public string? IdMeal { get; set; }

    [JsonPropertyName("strMeal")]
    public string? StrMeal { get; set; }

    [JsonPropertyName("strDrinkAlternate")]
    public string? StrDrinkAlternate { get; set; }

    [JsonPropertyName("strCategory")]
    public string? StrCategory { get; set; }

    [JsonPropertyName("strArea")]
    public string? StrArea { get; set; }

    [JsonPropertyName("strInstructions")]
    public string? StrInstructions { get; set; }

    [JsonPropertyName("strMealThumb")]
    public string? StrMealThumb { get; set; }

    [JsonPropertyName("strTags")]
    public string? StrTags { get; set; }

    [JsonPropertyName("strYoutube")]
    public string? StrYoutube { get; set; }

    // Add a flag to track if instructions are expanded
    public bool ShowFullInstructions { get; set; }

    // Dynamically build the list of ingredients and measures
    public List<string> Ingredients { get; set; } = [];
    public List<string> Measures { get; set; } = [];

    public Meal()
    {
        // Build the ingredients and measures list dynamically during object initialization
        for (var i = 1; i <= 20; i++)
        {
            // Dynamically get property names like strIngredient1, strMeasure1, etc.
            var ingredientProperty = typeof(Meal).GetProperty($"strIngredient{i}");
            var measureProperty = typeof(Meal).GetProperty($"strMeasure{i}");

            // Check if both ingredient and measure properties are not null and are not empty
            if (ingredientProperty == null || measureProperty == null) continue;
            var ingredient = ingredientProperty.GetValue(this) as string;
            var measure = measureProperty.GetValue(this) as string;

            // Only add to the list if there's an ingredient or measure
            if (string.IsNullOrEmpty(ingredient) || string.IsNullOrEmpty(measure)) continue;
            Ingredients.Add(ingredient);
            Measures.Add(measure);
        }
    }
}