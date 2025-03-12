namespace RecipeTracker.Web.API.Translations;

public class Translation
{
    public int Id { get; set; }
    public string LanguageCode { get; set; } = string.Empty; // e.g., "en", "fr"
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}