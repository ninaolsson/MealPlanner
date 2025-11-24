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
        protected RecipeRepository Repository { get; }
        public RecipeController(RecipeRepository repository)
        {
            Repository = repository;
        }

        [HttpGet("{id}")]
        public ActionResult<Recipe> GetRecipe([FromRoute] int id)
        {
            Recipe recipe = Repository.GetRecipeById(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return Ok(recipe);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Recipe>> GetRecipes()
        {
            return Ok(Repository.GetRecipes());
        }

        [HttpPost]
        public ActionResult Post([FromBody] Recipe recipe)
        {
            if (recipe == null)
            {
                return BadRequest("Recipe info not correct");
            }
            bool status = Repository.InsertRecipe(recipe);
            if (status)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut]

        public ActionResult UpdateRecipe([FromBody] Recipe recipe)
        {
            if (recipe == null)
            {
                return BadRequest("Recipe info not correct");
            }
            Recipe existinRecipe = Repository.GetRecipeById(recipe.RecipeId);
            if (existinRecipe == null)
            {
                return NotFound($"Recipe with id {recipe.RecipeId} not found");
            }
            bool status = Repository.UpdateRecipe(recipe);
            if (status)
            {
                return Ok();
            }
            return BadRequest("Something went wrong");
        }
        [HttpDelete("{id}")]
        public ActionResult DeleteRecipe([FromRoute] int id)
        {
            Recipe existingRecipe = Repository.GetRecipeById(id);
            if (existingRecipe == null)
            {
                return NotFound($"Recipe with id {id} not found");
            }
            bool status = Repository.DeleteRecipe(id);
            if (status)
            {
                return NoContent();
            }
            return BadRequest($"Unable to delete recipe with id {id}");
        }
    }
}
