using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RecipeTracker.ApiService.Service.Internal.Interface;
using RecipeTracker.ApiService.Translations;
using StackExchange.Redis;

namespace RecipeTracker.ApiService.Service.Internal;

public class TranslationService(
    TranslationDbContext context,
    IConnectionMultiplexer redis,
    ILogger<TranslationService> logger)
    : ITranslationService
{
    private readonly IDatabase _redis = redis.GetDatabase();

    public async Task<string?> GetTranslationAsync(string key, string languageCode)
    {
        var cacheKey = $"translation:{languageCode}:{key}";

        // Check Redis first
        var cachedValue = await _redis.StringGetAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedValue))
        {
            logger.LogInformation("Loaded translation for key '{Key}' from Redis cache.", key);  // Correct placeholder
            return cachedValue;
        }

        // Fallback to database
        var value = await context.Translations
            .Where(t => t.Key == key && t.LanguageCode == languageCode)
            .Select(t => t.Value)
            .FirstOrDefaultAsync();

        // Cache the result if found
        if (string.IsNullOrEmpty(value)) return value ?? string.Empty; // Ensure non-null return value
        logger.LogInformation("Caching translation for key '{Key}' in Redis.", key); // Correct placeholder
        await _redis.StringSetAsync(cacheKey, value, TimeSpan.FromHours(1)); // Expire after 1 hour

        return value; // Ensure non-null return value
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

        // Check Redis for cached translations
        var cachedTranslations = await _redis.StringGetAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedTranslations))
        {
            logger.LogInformation("Loaded all translations for language '{LanguageCode}' from Redis cache.", languageCode); // Correct placeholder

            // Ensure cached data is valid before deserialization
            return string.IsNullOrEmpty(cachedTranslations)
                ? new Dictionary<string, string>()
                : JsonSerializer.Deserialize<Dictionary<string, string>>(cachedTranslations!) ?? new Dictionary<string, string>();
        }

        // Fetch translations from the database if not cached
        logger.LogInformation("Translations not found in Redis, querying database...");
        var freshTranslations = await context.Translations
            .Where(t => t.LanguageCode == languageCode)
            .ToDictionaryAsync(t => t.Key, t => t.Value);

        // Cache the fresh translations in Redis with a 1-hour expiration
        logger.LogInformation("Caching all translations for language '{LanguageCode}' in Redis.", languageCode); // Correct placeholder
        await _redis.StringSetAsync(cacheKey, JsonSerializer.Serialize(freshTranslations), TimeSpan.FromHours(1));

        return freshTranslations;
    }

}
