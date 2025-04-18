using RecipeTracker.ServiceDefaults;
using RecipeTracker.Web.Components;
using Microsoft.AspNetCore.Components;
using RecipeTracker.ApiService.API;
using RecipeTracker.ApiService.Service.Internal;
using RecipeTracker.ApiService.Service.Internal.Interface;
using RecipeTracker.ApiService.Translations;

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
builder.Services.AddScoped<TranslationCacheWarmupService>();  // Change to Scoped service

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

// Ensure the database is created and seeded first
await app.CreateDbIfNotExists();

// Now, warm-up the Redis cache only after DB seeding
using (var scope = app.Services.CreateScope())
{
    var warmupService = scope.ServiceProvider.GetRequiredService<TranslationCacheWarmupService>();
    await warmupService.StartAsync(CancellationToken.None);  // Starting the service after DB seeding
}

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

app.Run();
