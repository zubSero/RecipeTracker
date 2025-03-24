using System.Text.Json.Serialization;
using RecipeTracker.ApiService.Models.External;

namespace RecipeTracker.ApiService.Service.External.Responses;

public class ApiResponse
{
    [JsonPropertyName("meals")]
    public required List<Meal>? Meals { get; set; }
}