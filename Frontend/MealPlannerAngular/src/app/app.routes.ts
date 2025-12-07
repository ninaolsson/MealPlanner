import { Routes } from '@angular/router';
import { MealPlanListComponent } from './mealplan/mealplan-list/mealplan-list.component';
import { MealPlanCreateComponent } from './mealplan/mealplan-create/mealplan-create.component';
import { MealPlanEditComponent } from './mealplan/mealplan-edit/mealplan-edit.component';
import { MealPlanWeekViewComponent } from './mealplan/mealplan-week-view/mealplan-week-view.component';


export const routes: Routes = [
  { path: '', redirectTo: '/mealplans', pathMatch: 'full' },
  { path: 'mealplans', component: MealPlanListComponent },
  { path: 'mealplans/create', component: MealPlanCreateComponent },
  { path: 'mealplans/edit/:id', component: MealPlanEditComponent },
  { path: 'mealplans/week', component: MealPlanWeekViewComponent }
];

