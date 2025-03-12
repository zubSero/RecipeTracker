using Microsoft.EntityFrameworkCore;
using RecipeTracker.Web.API.Translations.Interface;

namespace RecipeTracker.Web.API.Translations;

public class TranslationService(TranslationDbContext context) : ITranslationService
{
    // Retrieve a specific translation by key and language code
    public async Task<string?> GetTranslationAsync(string key, string languageCode)
    {
        return await context.Translations
            .Where(t => t.Key == key && t.LanguageCode == languageCode)
            .Select(t => t.Value)
            .FirstOrDefaultAsync();
    }

    // Retrieve translations dynamically based on a search term
    public async Task<Dictionary<string, string>> SearchTranslationsAsync(string languageCode, string searchTerm)
    {
        return await context.Translations
            .Where(t => t.LanguageCode == languageCode && t.Key.Contains(searchTerm))
            .ToDictionaryAsync(t => t.Key, t => t.Value);
    }

    // Optional: Retrieve all translations for a specific language code
    public async Task<Dictionary<string, string>> GetAllTranslationsAsync(string languageCode)
    {
        return await context.Translations
            .Where(t => t.LanguageCode == languageCode)
            .ToDictionaryAsync(t => t.Key, t => t.Value);
    }
}