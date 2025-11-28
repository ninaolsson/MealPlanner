/*VIEW ALL MEAL PLAN ENTRIES IN A TABLE*/


import { Component, OnInit } from '@angular/core';
import { MealPlan } from '../../model/meal-plan';
import { MealPlanService } from '../mealplan/mealplan.service';

@Component({
  selector: 'app-mealplan-list',
  standalone: true,
  imports: [],
  templateUrl: './mealplan-list.html',
  styleUrl: './mealplan-list.css'
})
export class MealPlanListComponent implements OnInit {

  mealPlans: MealPlan[] = [];

  constructor(private mealPlanService: MealPlanService) {}

  ngOnInit() { 
    this.mealPlanService.getMealPlans().subscribe({
      next: data => {
        this.mealPlans = data;
      },
      error: err => {
        console.error('Failed to fetch meal plans', err);
      }
    });
  }
}
