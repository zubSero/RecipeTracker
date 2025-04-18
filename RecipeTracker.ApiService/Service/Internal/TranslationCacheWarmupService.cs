using StackExchange.Redis;
using System.Text.Json;
using RecipeTracker.ApiService.Service.Internal.Interface;
using RecipeTracker.ApiService.Translations;

namespace RecipeTracker.ApiService.Service.Internal;

public class TranslationCacheWarmupService(
    IServiceProvider serviceProvider,
    ILogger<TranslationCacheWarmupService> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var translationService = scope.ServiceProvider.GetRequiredService<ITranslationService>();
        var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>().GetDatabase();

        var supportedLocales = new[] { "en", "es", "fr" };
        var cache = new Dictionary<string, Dictionary<string, string>?>();

        foreach (var locale in supportedLocales)
        {
            var cacheKey = $"translations:{locale}";

            // Check if cache already exists
            var existingCache = await redis.StringGetAsync(cacheKey);

            if (!string.IsNullOrEmpty(existingCache))
            {
                logger.LogInformation($"Cache for {locale} already exists, skipping warm-up.");
                continue; // Skip if the cache already exists
            }

            // Fetch translations from the database
            var translations = await translationService.GetAllTranslationsAsync(locale);
            cache[locale] = translations;

            // Store in Redis with an expiration of 7 days
            await redis.StringSetAsync(cacheKey, JsonSerializer.Serialize(translations), TimeSpan.FromDays(7));
        }

        // Register the cache into the shared memory (optional, if needed elsewhere)
        serviceProvider.GetRequiredService<TranslationCacheHolder>().SetCache(cache);

        logger.LogInformation("Translation cache warm-up completed.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
