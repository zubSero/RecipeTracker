using StackExchange.Redis;
using System.Text.Json;
using RecipeTracker.ApiService.Translations;
using RecipeTracker.Web.API.Translations.Interface;

namespace RecipeTracker.ApiService.Service.Internal;

public class TranslationCacheWarmupService(
    IServiceProvider serviceProvider,
    ILogger<TranslationCacheWarmupService> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var translationService = scope.ServiceProvider.GetRequiredService<ITranslationService>();
        var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>().GetDatabase();

        var supportedLocales = new[] { "en", "es", "fr" };
        var cache = new Dictionary<string, Dictionary<string, string>>();

        foreach (var locale in supportedLocales)
        {
            var translations = await translationService.GetAllTranslationsAsync(locale);
            cache[locale] = translations;

            await redis.StringSetAsync(
                $"translations:{locale}",
                JsonSerializer.Serialize(translations),
                TimeSpan.FromDays(7)
            );
        }

        // Register shared memory cache as singleton (via singleton instance injection)
        serviceProvider.GetRequiredService<TranslationCacheHolder>().SetCache(cache);

        logger.LogInformation("Translation cache warm-up completed.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}