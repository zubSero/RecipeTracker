using System.Text.Json.Serialization;

namespace RecipeTracker.Web.API.Models.Responses;

public class ApiResponse
{
    [JsonPropertyName("meals")]
    public required List<Meal>? Meals { get; set; }
}