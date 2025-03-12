namespace RecipeTracker.Web.API.Translations.Interface;

public interface ITranslationService
{
    Task<string?> GetTranslationAsync(string key, string languageCode);
    Task AddOrUpdateTranslationAsync(Translation translation);
    Task<IEnumerable<Translation>> GetAllTranslationsAsync(string languageCode);
    Task<Dictionary<string, string>> GetTranslationsAsync(string languageCode, IEnumerable<string> keys);
}