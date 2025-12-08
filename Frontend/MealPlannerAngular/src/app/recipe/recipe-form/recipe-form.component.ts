import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormArray, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { Recipe } from '../../model/recipe';
import { RecipeService } from '../recipe/recipe.service';   // <-- NOTE THE PATH

@Component({
  selector: 'app-recipe-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './recipe-form.html',
  styleUrls: ['./recipe-form.css']
})
export class RecipeFormComponent implements OnInit {

  form!: FormGroup;
  isEdit = false;
  recipeId?: number;
  saving = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private recipeService: RecipeService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  // ---- Getter for easy access to ingredients ----
  get ingredients(): FormArray {
    return this.form.get('ingredients') as FormArray;
  }

  ngOnInit(): void {
    // ---- Build the form ----
    this.form = this.fb.group({
      name: ['', Validators.required],
      cookingTime: [''],
      instructions: [''],
      ingredients: this.fb.array([])   // <-- IMPORTANT
    });

    // ---- Check if editing ----
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEdit = true;
      this.recipeId = Number(idParam);
      this.loadRecipe(this.recipeId);
    }
  }

  // ---- Populate form when editing ----
  loadRecipe(id: number) {
    this.recipeService.getRecipe(id).subscribe({
      next: (r) => {
        this.form.patchValue({
          name: r.name,
          cookingTime: r.cookingTime,
          instructions: r.instructions
        });

        // Populate ingredients
        this.ingredients.clear();
        r.ingredients.forEach(ing => {
          this.ingredients.push(
            this.fb.group({
              ingredientId: [ing.ingredientId],
              name: [ing.name, Validators.required],
              quantity: [ing.quantity, Validators.required]
            })
          );
        });
      },
      error: () => {
        this.error = 'Failed to load recipe.';
      }
    });
  }

  // ---- Add a new ingredient row ----
  addIngredient() {
    this.ingredients.push(
      this.fb.group({
        ingredientId: [null],
        name: ['', Validators.required],
        quantity: ['', Validators.required]
      })
    );
  }

  // ---- Remove ingredient row ----
  removeIngredient(index: number) {
    this.ingredients.removeAt(index);
  }

  // ---- Save handler ----
  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.error = null;

    const payload: Recipe = {
      recipeId: this.recipeId,
      ...this.form.value
    };

    const obs = this.isEdit
      ? this.recipeService.updateRecipe(payload)
      : this.recipeService.createRecipe(payload);

    obs.subscribe({
      next: () => {
        this.saving = false;
        this.router.navigate(['/recipes']);
      },
      error: () => {
        this.error = 'Failed to save recipe';
        this.saving = false;
      }
    });
  }

  cancel() {
    this.router.navigate(['/recipes']);
  }
}
