using System.Text.Json.Serialization;

namespace RecipeTracker.Web.Components.Models;

public class ApiResponse
{
    [JsonPropertyName("meals")]
    public required List<Meal>? Meals { get; set; }
}