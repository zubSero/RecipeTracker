using RecipeTracker.ServiceDefaults;
using RecipeTracker.Web.API;
using RecipeTracker.Web.API.Translations;
using RecipeTracker.Web.API.Translations.Interface;
using RecipeTracker.Web.Components;
using Microsoft.AspNetCore.Components;
using RecipeTracker.ApiService.API;
using RecipeTracker.ApiService.DB;
using RecipeTracker.ApiService.Service.Internal;
using RecipeTracker.ApiService.Translations;

// Note: Removed using references for API internal services since they now run on a separate host.

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// 1. Add Aspire service defaults & Redis output cache
// -----------------------------------------------------------------------------
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

// -----------------------------------------------------------------------------
// 2. Register services for Razor Pages, Blazor, and Translations
// -----------------------------------------------------------------------------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // For Blazor components
builder.Services.AddRazorPages();
builder.Services.AddScoped<ITranslationService, TranslationService>();

// Register TranslationDbContext (using "postgresdb" connection key)
builder.AddNpgsqlDbContext<TranslationDbContext>("postgresdb");

// -----------------------------------------------------------------------------
// 3. Register TranslationCacheHolder and TranslationCacheWarmupService
// -----------------------------------------------------------------------------
builder.Services.AddSingleton<TranslationCacheHolder>();
builder.Services.AddHostedService<TranslationCacheWarmupService>(); // Preload translations

// -----------------------------------------------------------------------------
// 4. Remove direct API service registrations (TheMealDbApiClient, IRecipeService, etc.)
//    Instead, register an HttpClient for the standalone API host.
// -----------------------------------------------------------------------------
var apiStandaloneHost = builder.Configuration.GetConnectionString("ApiStandaloneHost");
if (string.IsNullOrWhiteSpace(apiStandaloneHost))
{
    throw new InvalidOperationException("ApiStandaloneHost is not configured.");
}

builder.Services.AddHttpClient("ApiHost", client =>
{
    client.BaseAddress = new Uri(apiStandaloneHost);
});

// Register strongly-typed API client for Blazor-side consumption
builder.Services.AddHttpClient<RecipesApiClient>("ApiHost");

// -----------------------------------------------------------------------------
// 5. Register a default HttpClient using NavigationManager for internal links
// -----------------------------------------------------------------------------
builder.Services.AddScoped(sp =>
{
    var navigation = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(navigation.BaseUri) };
});

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

// -----------------------------------------------------------------------------
// 6. Map Razor Components (for Blazor) and default endpoints
// -----------------------------------------------------------------------------
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapRazorPages();
app.MapGet("/", () => Results.Redirect("/food"));
app.MapDefaultEndpoints();

// (Optional) Create the database if necessary.
app.CreateDbIfNotExists();

app.Run();
