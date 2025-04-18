namespace RecipeTracker.ApiService.Service.Internal.Interface
{
    public interface ITranslationService
    {
        // Retrieve a specific translation by key and language code
        Task<string?> GetTranslationAsync(string key, string languageCode);

        // Search for translations dynamically using a search term
        Task<Dictionary<string, string>> SearchTranslationsAsync(string languageCode, string searchTerm);

        // Retrieve all translations for a specific language code
        Task<Dictionary<string, string>> GetAllTranslationsAsync(string languageCode);
    }
}