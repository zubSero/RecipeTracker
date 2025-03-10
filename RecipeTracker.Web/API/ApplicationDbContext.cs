using Microsoft.EntityFrameworkCore;
using RecipeTracker.Web.API.Models;

namespace RecipeTracker.Web.API
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Meal> Meals { get; set; }

        // Add more DbSets as needed for other models
    }
}