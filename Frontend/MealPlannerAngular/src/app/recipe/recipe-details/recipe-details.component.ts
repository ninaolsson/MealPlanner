import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { RecipeService } from '../recipe/recipe.service';
import { Recipe } from '../../model/recipe';

@Component({
  selector: 'app-recipe-details',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './recipe-details.html',
  styleUrls: ['./recipe-details.css']
})
export class RecipeDetailsComponent implements OnInit {

  recipe?: Recipe;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private recipeService: RecipeService
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const id = Number(idParam);
      this.loadRecipe(id);
    }
  }

  loadRecipe(id: number) {
    this.recipeService.getRecipe(id).subscribe({
      next: (r) => this.recipe = r,
      error: (err) => {
        console.error(err);
        this.error = 'Failed to load recipe.';
      }
    });
  }
}

