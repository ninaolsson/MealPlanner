import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { Recipe } from '../../model/recipe';
import { RecipeService } from '../recipe/recipe.service';   // <-- NOTE THE PATH

@Component({
  selector: 'app-recipe-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './recipe-form.html',
  styleUrl: './recipe-form.css'
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

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required]],
      cookingTime: [''],
      instructions: [''],
      ingredients: ['']
    });

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEdit = true;
      this.recipeId = Number(idParam);
      this.loadRecipe(this.recipeId);
    }
  }

  loadRecipe(id: number) {
    this.recipeService.getRecipe(id).subscribe({
      next: (r) => {
        this.form.patchValue({
          name: r.name,
          cookingTime: r.cookingTime,
          instructions: r.instructions,
          ingredients: r.ingredients || ''
        });
      },
      error: err => {
        console.error(err);
        this.error = 'Failed to load recipe';
      }
    });
  }

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving = false;
    this.error = null;

    const payload: Recipe = {
      id: this.recipeId,
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
      error: err => {
        console.error(err);
        this.error = 'Failed to save recipe';
        this.saving = false;
      }
    });
  }

  cancel() {
    this.router.navigate(['/recipes']);
  }
}
