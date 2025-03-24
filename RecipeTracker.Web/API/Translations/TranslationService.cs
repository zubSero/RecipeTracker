using Microsoft.EntityFrameworkCore;
using RecipeTracker.Web.API.Translations.Interface;
using StackExchange.Redis;
using System.Text.Json;

namespace RecipeTracker.Web.API.Translations;

public class TranslationService(
    TranslationDbContext context,
    IConnectionMultiplexer redis,
    ILogger<TranslationService> logger)
    : ITranslationService
{
    public async Task<string?> GetTranslationAsync(string key, string languageCode)
    {
        var cacheKey = $"translation:{languageCode}:{key}";
        var database = redis.GetDatabase();

        // Check Redis first
        var cachedValue = await database.StringGetAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedValue))
        {
            logger.LogInformation("Loaded translation for key '{key}' from Redis cache.", key);
            return cachedValue;
        }

        // Fallback to database
        var value = await context.Translations
            .Where(t => t.Key == key && t.LanguageCode == languageCode)
            .Select(t => t.Value)
            .FirstOrDefaultAsync();

        // Cache the result if found
        if (!string.IsNullOrEmpty(value))
        {
            logger.LogInformation("Caching translation for key '{key}' in Redis.", key);
            await database.StringSetAsync(cacheKey, value, TimeSpan.FromHours(1)); // Expire after 1 hour
        }

        return value;
    }

    public async Task<Dictionary<string, string>> SearchTranslationsAsync(string languageCode, string searchTerm)
    {
        // Redis caching might not be practical for search as results vary based on `searchTerm`.
        return await context.Translations
            .Where(t => t.LanguageCode == languageCode && t.Key.Contains(searchTerm))
            .ToDictionaryAsync(t => t.Key, t => t.Value);
    }

    public async Task<Dictionary<string, string>> GetAllTranslationsAsync(string languageCode)
    {
        var cacheKey = $"translations:{languageCode}";
        var database = redis.GetDatabase();

        // Clear Redis cache on startup (optional for first-time call)
        if (!database.KeyExists(cacheKey))
        {
            logger.LogInformation("Clearing Redis cache for language '{languageCode}' on startup.", languageCode);

            // Pull fresh data from the database
            var translations = await context.Translations
                .Where(t => t.LanguageCode == languageCode)
                .ToDictionaryAsync(t => t.Key, t => t.Value);

            // Cache the fresh data in Redis
            logger.LogInformation("Caching all translations for language '{languageCode}' in Redis.", languageCode);
            await database.StringSetAsync(cacheKey, JsonSerializer.Serialize(translations), TimeSpan.FromHours(1));

            return translations;
        }

        // Check Redis cache for translations
        var cachedTranslations = await database.StringGetAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedTranslations))
        {
            logger.LogInformation("Loaded all translations for language '{languageCode}' from Redis cache.", languageCode);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(cachedTranslations);
        }

        // Fallback to database if Redis cache is empty
        logger.LogInformation("Translations not found in Redis, querying database...");
        var freshTranslations = await context.Translations
            .Where(t => t.LanguageCode == languageCode)
            .ToDictionaryAsync(t => t.Key, t => t.Value);

        // Cache the fresh translations in Redis
        logger.LogInformation("Caching all translations for language '{languageCode}' in Redis.", languageCode);
        await database.StringSetAsync(cacheKey, JsonSerializer.Serialize(freshTranslations), TimeSpan.FromHours(1));

        return freshTranslations;
    }

}
