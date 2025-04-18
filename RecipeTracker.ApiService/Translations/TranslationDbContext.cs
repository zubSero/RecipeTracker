using Microsoft.EntityFrameworkCore;
using RecipeTracker.Web.API.Translations;

namespace RecipeTracker.ApiService.Translations
{
    public class TranslationDbContext(DbContextOptions<TranslationDbContext> options) : DbContext(options)
    {
        public DbSet<TranslationModel> Translations { get; set; }
    }

    public static class Extensions
    {
        public static async Task CreateDbIfNotExists(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<TranslationDbContext>();

            try
            {
                // Apply migrations to the database (instead of EnsureCreated)
                await context.Database.MigrateAsync();
                await TranslationSeedData.InitializeAsync(context); // Seed data after migration
            }
            catch (Exception ex)
            {
                // Log exception using logger
                var logger = services.GetRequiredService<ILogger<TranslationDbContext>>();
                logger.LogError(ex, "An error occurred while migrating the database or seeding data.");
            }
        }
    }
}