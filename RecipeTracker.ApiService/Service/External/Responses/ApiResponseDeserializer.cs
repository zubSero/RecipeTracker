using System.Text.Json;
using RecipeTracker.ApiService.Service.External.Interfaces;

namespace RecipeTracker.ApiService.Service.External.Responses;

public class ApiResponseDeserializer: IApiResponseDeserializer
{
    public async Task<ApiResponse?> DeserializeResponseAsync(HttpResponseMessage response)
    {
        var jsonResponse = await response.Content.ReadAsStringAsync();
        try
        {
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(jsonResponse);
            return apiResponse ?? throw new JsonException("Failed to deserialize response.");
        }
        catch (JsonException ex)
        {
            // Log or handle the deserialization error (for now, we rethrow)
            throw new JsonException("Failed to deserialize the JSON response from the API.", ex);
        }
    }
}