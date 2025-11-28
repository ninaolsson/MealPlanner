import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MealPlan } from '../../model/meal-plan';

@Injectable({
  providedIn: 'root',
})
export class MealPlanService {

  private apiUrl = 'http://localhost:5009/api/mealplan'; 

  constructor(private http: HttpClient) {}

  getMealPlans(): Observable<MealPlan[]> {
    return this.http.get<MealPlan[]>(this.apiUrl);
  }

  getMealPlanById(id: number): Observable<MealPlan> {
    return this.http.get<MealPlan>(`${this.apiUrl}/${id}`);
  }

  createMealPlan(plan: MealPlan): Observable<any> {
    return this.http.post(this.apiUrl, plan);
  }

  updateMealPlan(plan: MealPlan): Observable<any> {
    return this.http.put(this.apiUrl, plan);
  }
  
  deleteMealPlan(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}

