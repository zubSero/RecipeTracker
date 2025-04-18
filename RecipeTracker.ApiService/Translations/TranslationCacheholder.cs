namespace RecipeTracker.ApiService.Translations
{
    public class TranslationCacheHolder
    {
        // Property to expose the cache with thread-safety protection
        private readonly Lock _cacheLock = new();
        public SafeTranslationCache Cache { get; private set; } = new(new Dictionary<string, Dictionary<string, string>>());

        // Method to set the cache with raw translations
        public void SetCache(Dictionary<string, Dictionary<string, string>?> rawCache)
        {
            lock (_cacheLock)  // Ensures thread-safety when modifying the cache
            {
                var converted = rawCache.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value ?? new Dictionary<string, string>(),
                    StringComparer.OrdinalIgnoreCase
                );

                Cache = new SafeTranslationCache(converted);
            }
        }
    }

    // A wrapper class for translations with fallback logic
    public class SafeTranslationCache(IDictionary<string, Dictionary<string, string>> original)
        : Dictionary<string, Dictionary<string, string>>(original, StringComparer.OrdinalIgnoreCase)
    {
        // Provide fallback behavior for missing locales
        public new Dictionary<string, string> this[string locale] =>
            !TryGetValue(locale, out var inner)
                ? new FallbackTranslationDictionary() // Return fallback dictionary if locale is not found
                : new FallbackTranslationDictionary(inner);

        // Inner class for key-based fallback
        private class FallbackTranslationDictionary : Dictionary<string, string>
        {
            public FallbackTranslationDictionary() : base(StringComparer.OrdinalIgnoreCase) { }

            public FallbackTranslationDictionary(IDictionary<string, string> source)
                : base(source, StringComparer.OrdinalIgnoreCase) { }
        }
    }
}
