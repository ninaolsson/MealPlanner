using MealPlanner.Model.Entities;
using MealPlanner.Model.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealPlanController : ControllerBase
    {
        protected MealPlanRepository Repository { get; }
        public MealPlanController(MealPlanRepository repository)
        {
            Repository = repository;
        }

        [HttpGet("{id}")]
        public ActionResult<MealPlan> GetMealplan([FromRoute] int id)
        {
            MealPlan mealplan = Repository.GetMealPlanById(id);
            if (mealplan == null)
            {
                return NotFound();
            }
            return Ok(mealplan);
        }

        [HttpGet]
        public ActionResult<IEnumerable<MealPlan>> GetMealplans()
        {
            return Ok(Repository.GetMealPlans());
        }

        [HttpPost]
        public ActionResult Post([FromBody] MealPlan mealplan)
        {
            if (mealplan == null)
            {
                return BadRequest("Mealplan info not correct");
            }
            bool status = Repository.InsertMealPlan(mealplan);
            if (status)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut]

        public ActionResult UpdateMealplan([FromBody] MealPlan mealplan)
        {
            if (mealplan == null)
            {
                return BadRequest("Mealplan info not correct");
            }
            MealPlan existinMealplan = Repository.GetMealPlanById(mealplan.MealId);
            if (existinMealplan == null)
            {
                return NotFound($"Mealplan with id {mealplan.MealId} not found");
            }
            bool status = Repository.UpdateMealPlan(mealplan);
            if (status)
            {
                return Ok();
            }
            return BadRequest("Something went wrong");
        }
        [HttpDelete("{id}")]
        public ActionResult DeleteMealplan([FromRoute] int id)
        {
            MealPlan existingMealplan = Repository.GetMealPlanById(id);
            if (existingMealplan == null)
            {
                return NotFound($"Mealplan with id {id} not found");
            }
            bool status = Repository.DeleteMealPlan(id);
            if (status)
            {
                return NoContent();
            }
            return BadRequest($"Unable to delete mealplan with id {id}");
        }
    }
}