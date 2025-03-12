using Microsoft.EntityFrameworkCore;
using RecipeTracker.Web.API.Translations;

namespace RecipeTracker.Web.API;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Translation> Translations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Translation>()
            .Property(t => t.Id)
            .ValueGeneratedNever(); // Ensure EF Core does not treat it as auto-generated

        modelBuilder.Entity<Translation>().HasData(
                modelBuilder.Entity<Translation>().HasData(TranslationSeedData.GetSeedData())
        );
    }
}