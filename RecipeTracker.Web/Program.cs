using Microsoft.EntityFrameworkCore;
using RecipeTracker.ServiceDefaults;
using RecipeTracker.Web.API;
using RecipeTracker.Web.API.Models.Interfaces;
using RecipeTracker.Web.API.Models.Responses;
using RecipeTracker.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

// Register necessary services for Razor Pages and Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // For Blazor components

builder.Services.AddRazorPages();  // This adds Razor Pages support

// Load the configuration from appsettings.json
var apiBaseUrl = builder.Configuration["MealDbApi:BaseUrl"];  // Reads the base URL from the configuration file

builder.Services.AddSingleton<IApiResponseDeserializer, ApiResponseDeserializer>();

// Register TheMealDbApiClient as a scoped service
builder.Services.AddHttpClient<TheMealDbApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl); // Use the URL from the configuration
});

// Register ApplicationDbContext with PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

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

// Ensure Database is Migrated and Seeded on Startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Apply any pending migrations
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.EnsureCreated();
        dbContext.Database.Migrate();
    }
}


app.Run();
