using Microsoft.EntityFrameworkCore;
using RecipeTracker.Web.API.Translations;

namespace RecipeTracker.Web.API;

public class TranslationDbContext(DbContextOptions<TranslationDbContext> options) : DbContext(options)
{
    public DbSet<Translation> Translations { get; set; }
}

public static class Extensions
{
    public static void CreateDbIfNotExists(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<TranslationDbContext>();
        try
        {
            context.Database.EnsureCreated();
            TranslationSeedData.Initialize(context);

        }
        catch (Exception ex)
        {
            // ignored
        }
    }
}