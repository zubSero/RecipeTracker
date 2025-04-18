namespace RecipeTracker.ApiService.Translations
{
    public class TranslationCacheHolder
    {
        // Property to expose the cache, ensuring fallback behavior
        public SafeTranslationCache Cache { get; private set; } = new(new Dictionary<string, Dictionary<string, string>>());

        // Method to set the cache with raw translations
        public void SetCache(Dictionary<string, Dictionary<string, string>?> rawCache)
        {
            // Ensure null-safe and wrap for fallback
            var converted = rawCache.ToDictionary(
                pair => pair.Key,
                pair => pair.Value ?? new Dictionary<string, string>(),
                StringComparer.OrdinalIgnoreCase
            );

            Cache = new SafeTranslationCache(converted);
        }
    }

    // A wrapper class for translations with fallback logic
    public class SafeTranslationCache(IDictionary<string, Dictionary<string, string>> original)
        : Dictionary<string, Dictionary<string, string>>(original, StringComparer.OrdinalIgnoreCase)
    {
        // Provide fallback behavior for missing locales
        public new Dictionary<string, string> this[string locale] => !TryGetValue(locale, out var inner) ? new FallbackTranslationDictionary() : new FallbackTranslationDictionary(inner);

        // Inner class for key-based fallback
        private class FallbackTranslationDictionary : Dictionary<string, string>
        {
            public FallbackTranslationDictionary() : base(StringComparer.OrdinalIgnoreCase) { }

            public FallbackTranslationDictionary(IDictionary<string, string> source)
                : base(source, StringComparer.OrdinalIgnoreCase) { }
        }
    }
}
