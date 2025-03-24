using RecipeTracker.ApiService.Service.External.Responses;

namespace RecipeTracker.ApiService.Service.External.Interfaces;

public interface IApiResponseDeserializer
{
    Task<ApiResponse?> DeserializeResponseAsync(HttpResponseMessage response);
}