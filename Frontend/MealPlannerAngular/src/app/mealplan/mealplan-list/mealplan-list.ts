import { Component } from '@angular/core';
import { MealPlan } from '../../model/meal-plan';

@Component({
  selector: 'app-mealplan-list',
  standalone: true,
  imports: [],
  templateUrl: './mealplan-list.html',
  styleUrl: './mealplan-list.css'
})
export class MealplanListComponent {
  
  mealplans: MealPlan[] = [
    {
      id: 1,
      dayOfWeek: 1,
      mealType: 'Lunch',
      recipeId: 101,
      recipeName: 'Pasta Bolognese'
    },
    {
      id: 2,
      dayOfWeek: 3,
      mealType: 'Dinner',
      recipeId: 205,
      recipeName: 'Chicken Curry'
    }
  ];

}
