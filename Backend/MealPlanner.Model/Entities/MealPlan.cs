namespace MealPlanner.Model.Entities;

public class MealPlan
{
    public int MealId { get; set; }
    public int RecipeId { get; set; }
    public string DayOfWeek { get; set; }
    public string MealType { get; set; }
    public string RecipeName { get; set; }
}