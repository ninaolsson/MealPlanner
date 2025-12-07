/* FORM TO CREATE NEW MEAL PLAN ENTRY */

import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { MealPlan } from '../../model/meal-plan';
import { MealPlanService } from '../mealplan/mealplan.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-mealplan-create',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './mealplan-create.html',
  styleUrl: './mealplan-create.css',
})
export class MealPlanCreateComponent implements OnInit {

  // The object we POST — matches the backend model exactly
  mealPlan: MealPlan = {
    mealId: 0,
    dayOfWeek: 'Monday',
    mealType: 'Breakfast',
    recipeId: 0
  };

  // Dropdown options
  daysOfWeek: string[] = [
    'Monday',
    'Tuesday',
    'Wednesday',
    'Thursday',
    'Friday',
    'Saturday',
    'Sunday',
  ];

  mealTypes: string[] = ['Breakfast', 'Lunch', 'Dinner'];

  // Recipes loaded from backend
  recipes: any[] = [];

  constructor(
    private mealPlanService: MealPlanService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Load recipes for dropdown
    this.mealPlanService.getRecipes().subscribe({
      next: (data) => {
        this.recipes = data;
      },
      error: (err) => {
        console.error("Failed to load recipes", err);
      }
    });
  }

  onSubmit() {
    console.log("Submitting meal plan:", this.mealPlan);

    this.mealPlanService.createMealPlan(this.mealPlan).subscribe({
      next: () => {
        this.router.navigate(['/mealplans']);
      },
      error: (err) => {
        console.error('Failed to create meal plan', err);
        alert('Something went wrong while saving. Please try again.');
      }
    });
  }
}

