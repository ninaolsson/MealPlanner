/*EDIT EXISTING MEAL PLAN ENTRY (WITH PRE-FILLED COMPONENTS)*/

import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-mealplan-edit',
  imports: [],
  templateUrl: './mealplan-edit.html',
  styleUrl: './mealplan-edit.css',
})
export class MealPlanEditComponent {
  id: number | null = null;

  constructor(private route: ActivatedRoute) {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
  
  }
}
