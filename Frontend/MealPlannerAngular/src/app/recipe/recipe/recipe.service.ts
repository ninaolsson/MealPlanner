import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { Recipe } from '../../model/recipe';

@Injectable({
  providedIn: 'root'
})
export class RecipeService {

  private apiUrl = 'http://localhost:5009/api/recipe';

  constructor(private http: HttpClient) {}

  // Get list of recipes
  getRecipes(): Observable<Recipe[]> {
    return this.http.get<Recipe[]>(`${this.apiUrl}`);
  }

  // Get single recipe by id
  getRecipe(id: number): Observable<Recipe> {
    return this.http.get<Recipe>(`${this.apiUrl}/${id}`);
  }

  // Create a new recipe
  createRecipe(payload: Recipe): Observable<Recipe> {
    return this.http.post<Recipe>(`${this.apiUrl}`, payload);
  }

  // Update existing recipe
  updateRecipe(payload: Recipe): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${payload.recipeId}`, payload);
  }

  // Delete recipe
  deleteRecipe(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }


  checkTitleExists(title: string, excludeId?: number | null): Observable<boolean> {
    if (!title || title.trim().length === 0) {
      return of(false);
    }
  
    let params = new HttpParams().set('title', title.trim());
    if (excludeId != null) {
      params = params.set('excludeId', String(excludeId));
    }
  
    return this.http.get<{ exists: boolean }>(`${this.apiUrl}/check-title`, { params }).pipe(
      map(res => !!res?.exists),
      catchError(() => of(false))
    );
  }
}