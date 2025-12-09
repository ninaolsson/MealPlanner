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
            Console.WriteLine("CreateRecipe called");

            if (recipe == null)
            {
                Console.WriteLine("CreateRecipe FAILED: recipe was null");
                return BadRequest("Recipe data missing.");
            }

            bool ok = _recipeRepo.InsertRecipe(recipe);

            if (!ok)
            {
                Console.WriteLine("CreateRecipe FAILED in repository");
                return BadRequest("Could not insert recipe.");
            }

            Console.WriteLine("Recipe successfully inserted!");
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
                return BadRequest("Failed to update recipe.");

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
    }
}