using MealPlanner.Model.Entities;
using MealPlanner.Model.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly RecipeRepository _recipeRepo;
        private readonly IngredientRepository _ingredientRepo;

        public RecipeController(RecipeRepository recipeRepo, IngredientRepository ingredientRepo)
        {
            _recipeRepo = recipeRepo;
            _ingredientRepo = ingredientRepo;
        }

        // ---------------------------------------------------------
        // GET ONE RECIPE (WITH INGREDIENTS)
        // ---------------------------------------------------------
        [HttpGet("{id}")]
        public ActionResult<Recipe> GetRecipe(int id)
        {
            var recipe = _recipeRepo.GetRecipeById(id);
            if (recipe == null)
                return NotFound($"Recipe with ID {id} not found.");

            recipe.Ingredients = _ingredientRepo.GetIngredientsByRecipeId(id);
            return Ok(recipe);
        }

        // ---------------------------------------------------------
        // GET ALL RECIPES (WITHOUT INGREDIENTS)
        // ---------------------------------------------------------
        [HttpGet]
        public ActionResult<IEnumerable<Recipe>> GetRecipes()
        {
            return Ok(_recipeRepo.GetRecipes());
        }

        // ---------------------------------------------------------
        // CREATE RECIPE (repository also inserts ingredients)
        // ---------------------------------------------------------
     [HttpPost]
        public ActionResult CreateRecipe([FromBody] Recipe recipe)
        {
            if (recipe == null) return BadRequest("Recipe data missing.");

        if (_recipeRepo.ExistsByName(recipe.Name))
        return Conflict(new { message = "A recipe with that name already exists." });

         bool ok = _recipeRepo.InsertRecipe(recipe);

         if (!ok)
        {
        if (_recipeRepo.ExistsByName(recipe.Name))
            return Conflict(new { message = "A recipe with that name already exists." });

        return BadRequest("Could not insert recipe.");
        }

    return Ok();
}

        // ---------------------------------------------------------
        // UPDATE RECIPE + INGREDIENTS
        // ---------------------------------------------------------
        [HttpPut]
        public ActionResult UpdateRecipe([FromBody] Recipe recipe)
        {
        if (recipe == null)
        return BadRequest("Recipe data missing.");

        var existing = _recipeRepo.GetRecipeById(recipe.RecipeId);
        if (existing == null)
        return NotFound($"Recipe with ID {recipe.RecipeId} does not exist.");

        existing.Ingredients = _ingredientRepo.GetIngredientsByRecipeId(recipe.RecipeId);

        existing.Name = recipe.Name;
        existing.CookingTime = recipe.CookingTime;
        existing.Instructions = recipe.Instructions;
        existing.Ingredients = recipe.Ingredients;

        bool ok = _recipeRepo.UpdateRecipe(existing);

        if (!ok)
        {
        if (_recipeRepo.ExistsByName(existing.Name, existing.RecipeId))
        return Conflict(new { message = "A recipe with that name already exists." });

        return BadRequest("Failed to update recipe.");
        }      

        return Ok();
        }

        // ---------------------------------------------------------
        // DELETE RECIPE + INGREDIENTS
        // ---------------------------------------------------------
        [HttpDelete("{id}")]
        public ActionResult DeleteRecipe(int id)
        {
            var existing = _recipeRepo.GetRecipeById(id);
            if (existing == null)
                return NotFound($"Recipe with ID {id} not found.");

            _ingredientRepo.DeleteIngredientsByRecipeId(id);

            bool ok = _recipeRepo.DeleteRecipe(id);
            if (!ok)
                return BadRequest("Failed to delete recipe.");

            return NoContent();
        }

        [HttpGet("check-title")]
        public IActionResult CheckTitle([FromQuery] string title, [FromQuery] int? excludeId)
        {
            if (string.IsNullOrWhiteSpace(title))
                return Ok(new { exists = false });

            var all = _recipeRepo.GetRecipes();

            var exists = all.Any(r =>
                string.Equals(r.Name, title, StringComparison.OrdinalIgnoreCase)
                && (!excludeId.HasValue || r.RecipeId != excludeId.Value)
            );

            return Ok(new { exists });
}
    }
}