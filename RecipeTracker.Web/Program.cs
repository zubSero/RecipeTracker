using Microsoft.EntityFrameworkCore;
using RecipeTracker.ServiceDefaults;
using RecipeTracker.Web.API;
using RecipeTracker.Web.API.Models.Interfaces;
using RecipeTracker.Web.API.Models.Responses;
using RecipeTracker.Web.API.Translations;
using RecipeTracker.Web.API.Translations.Interface;
using RecipeTracker.Web.Components;
using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

// Register necessary services for Razor Pages and Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // For Blazor components

builder.Services.AddRazorPages();  // This adds Razor Pages support

// Load the configuration from appsettings.json
var apiBaseUrl = builder.Configuration["MealDbApi:BaseUrl"];  // Reads the base URL from the configuration file

builder.Services.AddSingleton<IApiResponseDeserializer, ApiResponseDeserializer>();
builder.Services.AddScoped<ITranslationService, TranslationService>();

// Register TheMealDbApiClient as a scoped service
builder.Services.AddHttpClient<TheMealDbApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl); // Use the URL from the configuration
});

// Register TranslationDbContext
builder.AddNpgsqlDbContext<TranslationDbContext>("postgresdb");
// Preload translations into Redis and shared memory cache at startup
// Initialize the shared memory cache
var translationsCache = new Dictionary<string, Dictionary<string, string>>();

// Use a temporary service provider to preload translations
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var translationService = scope.ServiceProvider.GetRequiredService<ITranslationService>();
    var supportedLocales = new[] { "en", "es", "fr" }; // Add your supported locales

    foreach (var locale in supportedLocales)
    {
        var translations = await translationService.GetAllTranslationsAsync(locale);
        translationsCache[locale] = translations;

        var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>().GetDatabase();
        await redis.StringSetAsync(
            $"translations:{locale}",
            JsonSerializer.Serialize(translations),
            TimeSpan.FromDays(7) // Expiration for Redis cache
        );
    }
}

// Register the preloaded translations cache as a singleton service
builder.Services.AddSingleton(translationsCache);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseOutputCache();
app.MapStaticAssets();

// Map Razor Components (this is for server-side Blazor)
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Redirect root to /food (default route)
app.MapGet("/", () => Results.Redirect("/food"));

// Add default endpoints
app.MapDefaultEndpoints();
app.CreateDbIfNotExists();

app.Run();
