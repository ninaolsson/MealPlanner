/*VIEW ALL MEAL PLAN ENTRIES IN A TABLE*/


/* VIEW ALL MEAL PLAN ENTRIES IN A TABLE */

import { Component, OnInit } from '@angular/core';
import { MealPlan } from '../../model/meal-plan';
import { MealPlanService } from '../mealplan/mealplan.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-mealplan-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './mealplan-list.html',
  styleUrl: './mealplan-list.css'
})
export class MealPlanListComponent implements OnInit {

  mealPlans: MealPlan[] = [];

  constructor(private mealPlanService: MealPlanService) {}

  ngOnInit() {
    this.mealPlanService.getMealPlans().subscribe({
      next: (data) => {
        this.mealPlans = this.sortMealPlans(data);
      },
      error: (err) => {
        console.error('Failed to fetch meal plans', err);
      }
    });
  }

  // ---------------------------------------------------
  // SORT BY DAY OF WEEK AND MEAL TYPE
  // ---------------------------------------------------
  sortMealPlans(list: MealPlan[]): MealPlan[] {
    const dayOrder = [
      "Monday", "Tuesday", "Wednesday", "Thursday",
      "Friday", "Saturday", "Sunday"
    ];

    const mealOrder = ["Breakfast", "Lunch", "Dinner"];

    return list.sort((a, b) => {
      const dayCompare =
        dayOrder.indexOf(a.dayOfWeek) - dayOrder.indexOf(b.dayOfWeek);

      if (dayCompare !== 0) return dayCompare;

      return mealOrder.indexOf(a.mealType) - mealOrder.indexOf(b.mealType);
    });
  }

  deleteMealPlan(id: number) {
    if (confirm('Are you sure you want to delete this entry?')) {
      this.mealPlanService.deleteMealPlan(id).subscribe({
        next: () => {
          this.mealPlans = this.mealPlans.filter(p => p.mealId !== id);
        },
        error: (err) => {
          console.error('Delete failed', err);
        }
      });
    }
  }
}


