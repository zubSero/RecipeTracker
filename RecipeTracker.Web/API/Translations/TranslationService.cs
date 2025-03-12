using Microsoft.EntityFrameworkCore;
using RecipeTracker.Web.API.Translations.Interface;

namespace RecipeTracker.Web.API.Translations;

public class TranslationService(ApplicationDbContext context) : ITranslationService
{
    public async Task<string?> GetTranslationAsync(string key, string languageCode)
    {
        return await context.Translations
            .Where(t => t.Key == key && t.LanguageCode == languageCode)
            .Select(t => t.Value)
            .FirstOrDefaultAsync();
    }

    public async Task AddOrUpdateTranslationAsync(Translation translation)
    {
        var existingTranslation = await context.Translations
            .FirstOrDefaultAsync(t => t.Key == translation.Key && t.LanguageCode == translation.LanguageCode);

        if (existingTranslation != null)
        {
            existingTranslation.Value = translation.Value;
            context.Translations.Update(existingTranslation);
        }
        else
        {
            context.Translations.Add(translation);
        }

        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Translation>> GetAllTranslationsAsync(string languageCode)
    {
        return await context.Translations
            .Where(t => t.LanguageCode == languageCode)
            .ToListAsync();
    }

    public async Task<Dictionary<string, string>> GetTranslationsAsync(string languageCode, IEnumerable<string> keys)
    {
        var translations = await context.Translations
            .Where(t => t.LanguageCode == languageCode && keys.Contains(t.Key))
            .ToDictionaryAsync(t => t.Key, t => t.Value);

        return keys.ToDictionary(key => key, key => translations.GetValueOrDefault(key, key));
    }
}