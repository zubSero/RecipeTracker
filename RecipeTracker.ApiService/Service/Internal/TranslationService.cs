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

    private class FallbackTranslationDictionary(IDictionary<string, string> dictionary)
        : Dictionary<string, string>(dictionary, StringComparer.OrdinalIgnoreCase)
    {
        public new string this[string key]
        {
            get => base.TryGetValue(key, out var value) ? value : $"[{key}]";
            set => base[key] = value;
        }
    }

    public async Task<string?> GetTranslationAsync(string key, string languageCode)
    {
        var cacheKey = $"translation:{languageCode}:{key}";

        var cachedValue = await _redis.StringGetAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedValue))
        {
            logger.LogInformation("Loaded translation for key '{Key}' from Redis cache.", key);
            return cachedValue;
        }

        var value = await context.Translations
            .Where(t => t.Key == key && t.LanguageCode == languageCode)
            .Select(t => t.Value)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(value)) return $"[{key}]";

        logger.LogInformation("Caching translation for key '{Key}' in Redis.", key);
        await _redis.StringSetAsync(cacheKey, value, TimeSpan.FromHours(1));

        return value;
    }

    public async Task<Dictionary<string, string>> SearchTranslationsAsync(string languageCode, string searchTerm)
    {
        return await context.Translations
            .Where(t => t.LanguageCode == languageCode && t.Key.Contains(searchTerm))
            .ToDictionaryAsync(t => t.Key, t => t.Value);
    }

    public async Task<Dictionary<string, string>> GetAllTranslationsAsync(string languageCode)
    {
        var cacheKey = $"translations:{languageCode}";

        var cachedTranslations = await _redis.StringGetAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedTranslations))
        {
            logger.LogInformation("Loaded all translations for language '{LanguageCode}' from Redis cache.", languageCode);
            return new FallbackTranslationDictionary(
                JsonSerializer.Deserialize<Dictionary<string, string>>(cachedTranslations!) ?? []
            );
        }

        logger.LogInformation("Translations not found in Redis, querying database...");
        var freshTranslations = await context.Translations
            .Where(t => t.LanguageCode == languageCode)
            .ToDictionaryAsync(t => t.Key, t => t.Value);

        logger.LogInformation("Caching all translations for language '{LanguageCode}' in Redis.", languageCode);
        await _redis.StringSetAsync(cacheKey, JsonSerializer.Serialize(freshTranslations), TimeSpan.FromHours(1));

        return new FallbackTranslationDictionary(freshTranslations);
    }
}
