export interface Ingredient {
  ingredientId: number| null;
  name: string;
  quantity: string;
}

export interface Recipe {
  recipeId: null;
  name: string;
  cookingTime: number| null;
  instructions?: string;
  ingredients: Ingredient[];
}
