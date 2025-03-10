using RecipeTracker.Web.API.Models.Responses;

namespace RecipeTracker.Web.API.Models.Interfaces;

public interface IApiResponseDeserializer
{
    Task<ApiResponse?> DeserializeResponseAsync(HttpResponseMessage response);
}