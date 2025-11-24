namespace MealPlanner.Model.Entities;

public class Recipe
{
    public int RecipeId { get; set; }
    public string Name { get; set; }
    public int CookingTime { get; set; }
    public string Instructions { get; set; }
}
