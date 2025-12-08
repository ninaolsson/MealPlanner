export interface Ingredient {
    ingredientId: number;
    name: string;
    quantity: string;
  }
  
  export interface Recipe {
    recipeId: number;
    name: string;
    cookingTime: number;
    instructions?: string;
    ingredients: Ingredient[];
  }
