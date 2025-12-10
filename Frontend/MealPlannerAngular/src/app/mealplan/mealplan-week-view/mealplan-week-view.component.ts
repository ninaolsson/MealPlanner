/*UI THAT VISUALIZES THE MEAL PLAN WEEK IN A 7x3 GRID*/

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MealPlanService } from '../mealplan/mealplan.service';
import { MealPlan } from '../../model/meal-plan';

@Component({
  selector: 'app-mealplan-week-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './mealplan-week-view.html',
  styleUrl: './mealplan-week-view.css',
})
export class MealPlanWeekViewComponent implements OnInit {

  week: any = {};  // holds structured data
  loading = true;

  days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
  mealTypes = ["Breakfast", "Lunch", "Dinner"];

  constructor(private mealPlanService: MealPlanService) {}

  ngOnInit(): void {
    this.loadWeek();
  }

  loadWeek() {
    this.mealPlanService.getMealPlans().subscribe({
      next: (list: MealPlan[]) => {
        this.week = this.structureWeek(list);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  structureWeek(list: MealPlan[]) {
    const result: any = {};

    this.days.forEach(day => {
      result[day] = {};

      this.mealTypes.forEach(type => {
        result[day][type] = null; 
      });
    });

    list.forEach(mp => {
      result[mp.dayOfWeek][mp.mealType] = mp; 
    });

    return result;
  }
}
