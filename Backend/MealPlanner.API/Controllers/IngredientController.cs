using MealPlanner.Model.Entities;
using MealPlanner.Model.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        protected IngredientRepository Repository { get; }
        public IngredientController(IngredientRepository repository)
        {
            Repository = repository;
        }

        [HttpGet("{id}")]
        public ActionResult<Ingredient> GetIngredient([FromRoute] int id)
        {
            Ingredient ingredient = Repository.GetIngredientById(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            return Ok(ingredient);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Ingredient>> GetIngredients()
        {
            return Ok(Repository.GetAllIngredients());
        }

        [HttpPost]
        public ActionResult Post([FromBody] Ingredient ingredient)
        {
            if (ingredient == null)
            {
                return BadRequest("Ingredient info not correct");
            }
            bool status = Repository.InsertIngredient(ingredient);
            if (status)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut]
        public ActionResult UpdateIngredient([FromBody] Ingredient ingredient)
        {
            if (ingredient == null)
            {
                return BadRequest("Ingredient info not correct");
            }

            if (ingredient.IngredientId == null)
            {
                return BadRequest("IngredientId cannot be null for update.");
            }

            Ingredient existingIngredient = Repository.GetIngredientById(ingredient.IngredientId.Value);
            if (existingIngredient == null)
            {
                return NotFound($"Ingredient with id {ingredient.IngredientId} not found");
            }

            bool status = Repository.UpdateIngredient(ingredient);
            if (status)
            {
                return Ok();
            }

            return BadRequest("Something went wrong");
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteIngredient([FromRoute] int id)
        {
            Ingredient existingIngredient = Repository.GetIngredientById(id);
            if (existingIngredient == null)
            {
                return NotFound($"Ingredient with id {id} not found");
            }
            bool status = Repository.DeleteIngredient(id);
            if (status)
            {
                return NoContent();
            }
            return BadRequest($"Unable to delete ingredient with id {id}");
        }
    }
}

