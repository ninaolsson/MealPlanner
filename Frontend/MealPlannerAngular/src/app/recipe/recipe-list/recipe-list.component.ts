import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { Recipe } from '../../model/recipe'; 
import { RecipeService } from '../recipe/recipe.service'; 
import { Router } from '@angular/router';

@Component({
  selector: 'app-recipe-list',
  templateUrl: './recipe-list.html',
  styleUrls: ['./recipe-list.css'],
  standalone: true,  
  imports: [CommonModule] 
})
export class RecipeListComponent implements OnInit {

  recipes$!: Observable<Recipe[]>;
  error: string | null = null;

  constructor(private recipeService: RecipeService, private router: Router) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.recipes$ = this.recipeService.getRecipes();
    this.error = null;
  }

  viewRecipe(id?: number | null): void {
    if (id == null) return;
    this.router.navigate(['/recipes', id]);
  }

  editRecipe(id?: number | null): void {
    if (id == null) return;
    this.router.navigate(['/recipes/edit', id]);
  }

  deleteRecipe(id?: number | null): void {
    if (id == null) return;

    this.recipeService.deleteRecipe(id).subscribe({
      next: () => {
        // Refresh the list after deletion
        this.load();
      },
      error: (err: any) => {
        console.error('Delete failed', err);
        this.error = 'Failed to delete recipe';
      }
    });
  }

  newRecipe(): void {
    this.router.navigate(['/recipes/create']);
  }
}


