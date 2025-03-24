using RecipeTracker.ApiService.Service.External;
using RecipeTracker.ApiService.Service.External.Interfaces;
using RecipeTracker.ApiService.Service.External.Responses;
using RecipeTracker.ApiService.Service.Internal;
using RecipeTracker.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire's service defaults (this sets up Aspire-specific middleware and configurations)
builder.AddServiceDefaults();

// Register controllers for the API endpoints.
builder.Services.AddControllers();

// Load the configuration (e.g., base URL for the external Meal API) from appsettings.json.
var apiBaseUrl = builder.Configuration["MealDbApi:BaseUrl"];
if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException("MealDbApi:BaseUrl is not configured.");
}

// Register TheMealDbApiClient with a typed HttpClient that uses the BaseAddress from configuration.
builder.Services.AddHttpClient<TheMealDbApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Register the API serializer or response deserializer.
builder.Services.AddSingleton<IApiResponseDeserializer, ApiResponseDeserializer>();

// Register your internal service that converts external responses into your domain model.
builder.Services.AddScoped<IRecipeService, RecipeService>();

// Optionally, add ProblemDetails and OpenAPI/Swagger generation for easier debugging/documentation.
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Use an error handler endpoint in production (this example uses "/error"). Adjust as needed.
app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// Map controllers so that API endpoints will be available at URLs such as /api/recipes.
app.MapControllers();


// Run the application as a standalone host.
app.Run();
