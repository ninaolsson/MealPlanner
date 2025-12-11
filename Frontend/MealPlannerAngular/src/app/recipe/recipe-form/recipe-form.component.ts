import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';


import { Recipe } from '../../model/recipe';
import { RecipeService } from '../recipe/recipe.service';

interface IngredientEdit {
  ingredientId?: number | null;
  name: string;
  quantity?: number | string | null;
}

interface RecipeEdit {
  recipeId?: number | null;
  name: string;
  cookingTime?: number | string | null;
  instructions?: string;
  ingredients: IngredientEdit[];
}


@Component({
  selector: 'app-recipe-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './recipe-form.html',
  styleUrls: ['./recipe-form.css']
})
export class RecipeFormComponent implements OnInit {

  recipe: RecipeEdit = {
    recipeId: undefined,
    name: '',
    cookingTime: '',
    instructions: '',
    ingredients: []
  };

  isEdit = false;
  recipeId?: number;
  saving = false;
  error: string | null = null;


  nameTaken = false;

  constructor(
    private recipeService: RecipeService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEdit = true;
      this.recipeId = Number(idParam);
      this.loadRecipe(this.recipeId);
    } else {
      // start with one blank ingredient row for usability
      if (!this.recipe.ingredients || this.recipe.ingredients.length === 0) {
        this.addIngredient();
      }
    }
  }

  loadRecipe(id: number) {
    this.recipeService.getRecipe(id).subscribe({
      next: (r) => {
        this.recipe = {
          recipeId: r.recipeId,
          name: r.name ?? '',
          cookingTime: r.cookingTime ?? '',
          instructions: r.instructions ?? '',
          ingredients: (r.ingredients || []).map((ing: any) => ({
            ingredientId: ing.ingredientId ?? null,
            name: ing.name ?? '',
            quantity: ing.quantity ?? null
          }))
        };
        if (this.recipe.ingredients.length === 0) this.addIngredient();
      },
      error: () => {
        this.error = 'Failed to load recipe.';
      }
    });
  }

  addIngredient() {
    this.recipe.ingredients.push({ ingredientId: null, name: '', quantity: null });
  }

  removeIngredient(index: number) {
    this.recipe.ingredients.splice(index, 1);
    if (this.recipe.ingredients.length === 0) this.addIngredient();
  }

  checkNameUnique() {
    this.nameTaken = false;
    const title = (this.recipe.name || '').trim();
    if (!title || title.length < 3) return; //no name under 3 characters
  
    this.recipeService.checkTitleExists(title, this.recipeId).subscribe({
      next: (exists: boolean) => {
        //another recipe already uses that name
        this.nameTaken = !!exists;
      },
      error: (err: any) => {
        console.error('checkTitleExists failed', err);
        //don't block user on network error
        this.nameTaken = false;
      }
    });
  }

  private parseNumber(value: number | string | undefined | null): number | undefined {
    if (value === null || value === undefined || value === '') return undefined;
    const n = Number(value);
    return Number.isNaN(n) ? undefined : n;
  }

  save(form: NgForm) {
    if (!form.valid || this.nameTaken) {
      form.control.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.error = null;
  
    const payload = {
      recipeId: this.recipeId ?? undefined,
      name: this.recipe.name,
      cookingTime: this.parseNumber(this.recipe.cookingTime),
      instructions: this.recipe.instructions,
      ingredients: (this.recipe.ingredients || []).map(ing => ({
        ingredientId: ing.ingredientId ?? undefined,
        name: ing.name,
        quantity: this.parseNumber(ing.quantity)
      }))
    };

    let obs: Observable<any>;
    if (this.isEdit) {
      obs = this.recipeService.updateRecipe(payload as any);
    } else {
      obs = this.recipeService.createRecipe(payload as any);
    }
  
    obs.subscribe({
      next: () => {
        this.saving = false;
        this.router.navigate(['/recipes']);
      },
      error: (err: any) => {
        console.error(err);
        this.error = 'Failed to save recipe';
        this.saving = false;
      }
    });
  }
  cancel(): void {
    this.router.navigate(['/recipes']);
  }
}