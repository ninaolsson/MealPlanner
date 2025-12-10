import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { Observable } from 'rxjs';

import { Recipe } from '../../model/recipe';
import { RecipeService } from '../recipe/recipe.service'; 

@Component({
  selector: 'app-recipe-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './recipe-list.html',
  styleUrl: './recipe-list.css'
})
export class RecipeListComponent implements OnInit {

  recipes$!: Observable<Recipe[]>;
  error: string | null = null;

  constructor(
    private recipeService: RecipeService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.recipes$ = this.recipeService.getRecipes();
    this.error = null;
  }

  viewRecipe(recipeId?: number) {
    if (!recipeId) return;
    this.router.navigate(['/recipes', recipeId]);
  }

  editRecipe(recipeId?: number) {
    if (!recipeId) return;
    this.router.navigate(['/recipes/edit', recipeId]);
  }

  newRecipe() {
    this.router.navigate(['/recipes/create']);
  }

  deleteRecipe(recipeId?: number) {
    if (!recipeId) return;
  
    if (!confirm('Are you sure you want to delete this recipe?')) return;
  
    this.recipeService.deleteRecipe(recipeId).subscribe({
      next: () => {
        // Reload fresh list from backend
        this.load();
      },
      error: (err) => {
        console.error('Delete failed', err);
        this.error = 'Failed to delete recipe.';
      }
    });
  }
}

