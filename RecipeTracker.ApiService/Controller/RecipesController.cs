using Microsoft.AspNetCore.Mvc;
using RecipeTracker.ApiService.Service.Internal;

namespace RecipeTracker.ApiService.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController(IRecipeService recipeService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetRecipes([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query parameter is required.");
            }

            var recipes = await recipeService.GetRecipesAsync(query);

            if (!recipes.Any())
            {
                return NotFound("No recipes found for the given query.");
            }

            return Ok(recipes);
        }
    }
}