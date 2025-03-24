namespace RecipeTracker.ApiService.Models.Internal
{
    public class RecipeModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Youtube { get; set; } = string.Empty;

        // A list of ingredient/measure pairs.
        public List<IngredientMeasure> Ingredients { get; set; } = [];

        //helper prop to show/hide instructions
        public bool ShowFullInstructions { get; set; }
    }

    public class IngredientMeasure
    {
        public string Ingredient { get; set; } = string.Empty;
        public string Measure { get; set; } = string.Empty;
    }
}