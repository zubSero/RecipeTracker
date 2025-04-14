namespace RecipeTracker.ApiService.Translations;

public class TranslationCacheHolder
{
    public Dictionary<string, Dictionary<string, string>> Cache { get; private set; } = new();

    public void SetCache(Dictionary<string, Dictionary<string, string>> cache)
    {
        Cache = cache;
    }
}