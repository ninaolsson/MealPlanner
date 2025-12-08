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

            // Attach ingredients
            recipe.Ingredients = _ingredientRepo.GetIngredientsByRecipeId(id);

            return Ok(recipe);
        }

        // ---------------------------------------------------------
        // GET ALL RECIPES (WITHOUT ingredients for performance)
        // ---------------------------------------------------------
        [HttpGet]
        public ActionResult<IEnumerable<Recipe>> GetRecipes()
        {
            return Ok(_recipeRepo.GetRecipes());
        }

        // ---------------------------------------------------------
        // CREATE RECIPE + INGREDIENTS
        // ---------------------------------------------------------
        [HttpPost]
        public ActionResult CreateRecipe([FromBody] Recipe recipe)
        {
            if (recipe == null)
                return BadRequest("Recipe data missing.");

            bool ok = _recipeRepo.InsertRecipe(recipe);
            if (!ok)
                return BadRequest("Could not insert recipe.");

            // -----------------------------------------------------
            // Insert ingredients
            // -----------------------------------------------------
            if (recipe.Ingredients != null && recipe.Ingredients.Count > 0)
            {
                foreach (var ing in recipe.Ingredients)
                {
                    ing.RecipeId = recipe.RecipeId;
                    _ingredientRepo.InsertIngredient(ing);
                }
            }

            return Ok();
        }

        // ---------------------------------------------------------
        // UPDATE RECIPE + REPLACE INGREDIENTS
        // ---------------------------------------------------------
        [HttpPut]
        public ActionResult UpdateRecipe([FromBody] Recipe recipe)
        {
            if (recipe == null)
                return BadRequest("Recipe data missing.");

            var existing = _recipeRepo.GetRecipeById(recipe.RecipeId);
            if (existing == null)
                return NotFound($"Recipe with ID {recipe.RecipeId} does not exist.");

            bool ok = _recipeRepo.UpdateRecipe(recipe);
            if (!ok)
                return BadRequest("Failed to update recipe.");

            // -----------------------------------------------------
            // Replace Ingredients: delete all + reinsert new ones
            // -----------------------------------------------------
            _ingredientRepo.DeleteIngredientsByRecipeId(recipe.RecipeId);

            if (recipe.Ingredients != null)
            {
                foreach (var ing in recipe.Ingredients)
                {
                    ing.RecipeId = recipe.RecipeId;
                    _ingredientRepo.InsertIngredient(ing);
                }
            }

            return Ok();
        }

        // ---------------------------------------------------------
        // DELETE RECIPE + ALL INGREDIENTS
        // ---------------------------------------------------------
        [HttpDelete("{id}")]
        public ActionResult DeleteRecipe(int id)
        {
            var existing = _recipeRepo.GetRecipeById(id);
            if (existing == null)
                return NotFound($"Recipe with ID {id} not found.");

            // Delete ingredients first
            _ingredientRepo.DeleteIngredientsByRecipeId(id);

            // Delete the recipe
            bool ok = _recipeRepo.DeleteRecipe(id);
            if (!ok)
                return BadRequest("Failed to delete recipe.");

            return NoContent();
        }
    }
}