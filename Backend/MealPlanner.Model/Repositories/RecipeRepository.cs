namespace MealPlanner.Model.Repositories;

using System;

using MealPlanner.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;


public class RecipeRepository : BaseRepository
{
    public RecipeRepository(IConfiguration configuration) : base(configuration) { }

    
    // GET ONE RECIPE
    
    public Recipe GetRecipeById(int id)
{
    NpgsqlConnection dbConn = null;

    try
    {
        dbConn = new NpgsqlConnection(ConnectionString);
        dbConn.Open();

        var cmd = dbConn.CreateCommand();

        cmd.CommandText = @"
            SELECT r.recipe_id,
                   r.name,
                   r.cooking_time,
                   r.instructions,
                   i.ingredient_id,
                   i.name AS ingredient_name,
                   i.quantity
            FROM recipe r
            LEFT JOIN ingredient i ON r.recipe_id = i.recipe_id
            WHERE r.recipe_id = @id
        ";

        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

        var data = cmd.ExecuteReader();

        Recipe recipe = null;

        while (data.Read())
        {
            // Initialize recipe only once
            if (recipe == null)
            {
                recipe = new Recipe
                {
                    RecipeId = (int)data["recipe_id"],
                    Name = data["name"].ToString(),
                    CookingTime = (int)data["cooking_time"],
                    Instructions = data["instructions"] == DBNull.Value
                        ? null
                        : data["instructions"].ToString(),
                    Ingredients = new List<Ingredient>()
                };
            }

            // Add ingredient rows
            if (data["ingredient_id"] != DBNull.Value)
            {
                recipe.Ingredients.Add(new Ingredient
                {
                    IngredientId = (int)data["ingredient_id"],
                    RecipeId = id,
                    Name = data["ingredient_name"].ToString(),
                    Quantity = data["quantity"].ToString()
                });
            }
        }

        return recipe;
    }
    finally
    {
        dbConn?.Close();
    }
}


    
    // GET ALL RECIPES FROM THE DATABASE
    
    public List<Recipe> GetRecipes()
    {
        var recipes = new List<Recipe>();
        NpgsqlConnection dbConn = null;

        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();

            cmd.CommandText = "select * from recipe order by recipe_id";

            var data = GetData(dbConn, cmd);

            while (data.Read())
            {
                recipes.Add(new Recipe
                {
                    RecipeId = (int)data["recipe_id"],
                    Name = data["name"].ToString(),
                    CookingTime = (int)data["cooking_time"],
                    Instructions = data["instructions"] == DBNull.Value
                        ? null
                        : data["instructions"].ToString()
                });
            }

            return recipes;
        }
        finally
        {
            dbConn?.Close();
        }
    }

    
    // ADD A NEW RECIPE TO THE DATABASE
    
    public bool InsertRecipe(Recipe r)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();

        cmd.CommandText = @"
            insert into recipe (name, cooking_time, instructions)
            values (@name, @cooking_time, @instructions)
        ";

        cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, r.Name);
        cmd.Parameters.AddWithValue("@cooking_time", NpgsqlDbType.Integer, r.CookingTime);

        if (r.Instructions == null)
            cmd.Parameters.AddWithValue("@instructions", DBNull.Value);
        else
            cmd.Parameters.AddWithValue("@instructions", NpgsqlDbType.Text, r.Instructions);

        return InsertData(dbConn, cmd);
    }

    
    // UPDATE AN EXISTING RECIPE'S DETAIL
    
    public bool UpdateRecipe(Recipe r)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();

        cmd.CommandText = @"
            update recipe set
                name = @name,
                cooking_time = @cooking_time,
                instructions = @instructions
            where recipe_id = @id
        ";

        cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, r.Name);
        cmd.Parameters.AddWithValue("@cooking_time", NpgsqlDbType.Integer, r.CookingTime);

        if (r.Instructions == null)
            cmd.Parameters.AddWithValue("@instructions", DBNull.Value);
        else
            cmd.Parameters.AddWithValue("@instructions", NpgsqlDbType.Text, r.Instructions);

        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, r.RecipeId);

        return UpdateData(dbConn, cmd);
    }

    
    // DELETE A RECIPE
    
    public bool DeleteRecipe(int id)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();

        cmd.CommandText = "delete from recipe where recipe_id = @id";
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

        return DeleteData(dbConn, cmd);
    }
}
