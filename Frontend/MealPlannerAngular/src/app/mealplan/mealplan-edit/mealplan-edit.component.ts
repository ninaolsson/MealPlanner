/* EDIT EXISTING MEAL PLAN ENTRY */

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { MealPlan } from '../../model/meal-plan';
import { MealPlanService } from '../mealplan/mealplan.service';

@Component({
  selector: 'app-mealplan-edit',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './mealplan-edit.html',
  styleUrl: './mealplan-edit.css',
})
export class MealPlanEditComponent implements OnInit {
  
  id!: number;
  mealPlan: MealPlan | null = null;

  daysOfWeek: string[] = [
    'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'
  ];

  mealTypes: string[] = ['Breakfast', 'Lunch', 'Dinner'];
  recipes: any[] = [];

  constructor(
    private route: ActivatedRoute,
    private mealPlanService: MealPlanService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Read ID from URL
    this.id = Number(this.route.snapshot.paramMap.get('id'));

    // Load meal plan entry from backend
    this.mealPlanService.getMealPlanById(this.id).subscribe({
      next: (data) => {
        this.mealPlan = data;   // Pre-fill form
      },
      error: (err) => console.error("Failed to load meal plan", err),
    });

    // Load recipe list for dropdown
    this.mealPlanService.getRecipes().subscribe({
      next: (data) => {
        this.recipes = data;
      },
      error: (err) => console.error("Failed to load recipes", err),
    });
  }

  onSubmit() {
    if (!this.mealPlan) return;

    this.mealPlanService.updateMealPlan(this.mealPlan).subscribe({
      next: () => {
        alert("Meal plan updated!");
        this.router.navigate(['/mealplans']);
      },
      error: (err) => {
        console.error("Update failed", err);
        alert("Something went wrong during update.");
      }
    });
  }
}

