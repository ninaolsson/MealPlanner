import { Routes } from '@angular/router';
import { MealPlanListComponent } from './mealplan/mealplan-list/mealplan-list.component';
import { MealPlanCreateComponent } from './mealplan/mealplan-create/mealplan-create.component';
import { MealPlanEditComponent } from './mealplan/mealplan-edit/mealplan-edit.component';
import { MealPlanWeekViewComponent } from './mealplan/mealplan-week-view/mealplan-week-view.component';
import { RecipeListComponent } from './recipe/recipe-list/recipe-list.component';
import { RecipeFormComponent } from './recipe/recipe-form/recipe-form.component';
import { RecipeDetailsComponent } from './recipe/recipe-details/recipe-details.component';


export const routes: Routes = [
  { path: '', redirectTo: '/mealplans', pathMatch: 'full' },
  { path: 'mealplans', component: MealPlanListComponent },
  { path: 'mealplans/create', component: MealPlanCreateComponent },
  { path: 'mealplans/edit/:id', component: MealPlanEditComponent },
  { path: 'mealplans/week', component: MealPlanWeekViewComponent },

  { path: 'recipes', component: RecipeListComponent },
  { path: 'recipes/create', component: RecipeFormComponent },
  { path: 'recipes/edit/:id', component: RecipeFormComponent },
  { path: 'recipes/:id', component: RecipeDetailsComponent },
];

