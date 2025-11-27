export interface MealPlan {
    id: number;
    dayOfWeek: number;
    mealType: string;
    recipeId: number;
    recipeName?: string;
}
