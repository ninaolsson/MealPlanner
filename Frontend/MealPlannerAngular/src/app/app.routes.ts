import { Routes } from '@angular/router';
import { MealplanListComponent } from './mealplan/mealplan-list/mealplan-list';
import { MealplanCreateComponent } from './mealplan/mealplan-create/mealplan-create';
import { MealplanEditComponent } from './mealplan/mealplan-edit/mealplan-edit';
import { MealplanWeekViewComponent } from './mealplan/mealplan-week-view/mealplan-week-view';



export const routes: Routes = [{ path: '', redirectTo: '/mealplans', pathMatch: 'full' },

    { path: 'mealplans', component: MealplanListComponent },
    { path: 'mealplans/create', component: MealplanCreateComponent },
    { path: 'mealplans/edit/:id', component: MealplanEditComponent },
    { path: 'mealplans/week', component: MealplanWeekViewComponent }
];
