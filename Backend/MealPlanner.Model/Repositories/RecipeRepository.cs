namespace MealPlanner.Model.Repositories;

using System;
using System.Collections.Generic;
using MealPlanner.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

public class RecipeRepository : BaseRepository
{
    public RecipeRepository(IConfiguration configuration) : base(configuration) { }

    // ---------------------------------------------------------
    // GET ONE RECIPE BY ID (WITHOUT INGREDIENTS)
    // ---------------------------------------------------------
    public Recipe GetRecipeById(int id)
    {
        NpgsqlConnection dbConn = null;

        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();

            cmd.CommandText = "SELECT * FROM recipe WHERE recipe_id = @id";
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

            var data = GetData(dbConn, cmd);

            if (data.Read())
            {
                return new Recipe
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

            return null;
        }
        finally
        {
            dbConn?.Close();
        }
    }

    // ---------------------------------------------------------
    // GET ALL RECIPES (WITHOUT INGREDIENTS)
    // ---------------------------------------------------------
    public List<Recipe> GetRecipes()
    {
        var recipes = new List<Recipe>();
        NpgsqlConnection dbConn = null;

        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM recipe ORDER BY recipe_id";

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
                        : data["instructions"].ToString(),
                    Ingredients = new List<Ingredient>()
                });
            }

            return recipes;
        }
        finally
        {
            dbConn?.Close();
        }
    }
// ---------------------------------------------------------
// INSERT RECIPE (WITH INGREDIENTS)
// ---------------------------------------------------------
public bool InsertRecipe(Recipe r)
{
    var dbConn = new NpgsqlConnection(ConnectionString);
    dbConn.Open();
    var transaction = dbConn.BeginTransaction();

    try
    {
        // Insert recipe
        var cmd = dbConn.CreateCommand();
        cmd.Transaction = transaction;

        cmd.CommandText = @"
            INSERT INTO recipe (name, cooking_time, instructions)
            VALUES (@name, @cooking_time, @instructions)
            RETURNING recipe_id
        ";

        cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, r.Name);
        cmd.Parameters.AddWithValue("@cooking_time", NpgsqlDbType.Integer, r.CookingTime);
        cmd.Parameters.AddWithValue("@instructions",
            r.Instructions == null ? DBNull.Value : r.Instructions);

        // Store new ID on recipe object
        int newId = (int)cmd.ExecuteScalar();
        r.RecipeId = newId;

        // Insert ingredients
        if (r.Ingredients != null)
        {
            foreach (var ing in r.Ingredients)
            {
                var cmdIng = dbConn.CreateCommand();
                cmdIng.Transaction = transaction;

                cmdIng.CommandText = @"
                    INSERT INTO ingredient (recipe_id, name, quantity)
                    VALUES (@recipe_id, @name, @quantity)
                ";

                cmdIng.Parameters.AddWithValue("@recipe_id", NpgsqlDbType.Integer, newId);
                cmdIng.Parameters.AddWithValue("@name", NpgsqlDbType.Text, ing.Name);

                // IMPORTANT: Specify varchar type explicitly
                cmdIng.Parameters.AddWithValue("@quantity", 
                    ing.Quantity == null ? DBNull.Value : (object)ing.Quantity);
                cmdIng.Parameters["@quantity"].NpgsqlDbType = NpgsqlDbType.Varchar;

                cmdIng.ExecuteNonQuery();
            }
        }

        transaction.Commit();
        return true;
    }
    catch (Exception ex)
{
    Console.WriteLine("=== INSERT ERROR ===");
    Console.WriteLine(ex.GetType().ToString());
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);
    Console.WriteLine("=== END ERROR ===");

    transaction.Rollback();
    return false;
}
    finally
    {
        dbConn.Close();
    }
}

    // ---------------------------------------------------------
    // UPDATE RECIPE (WITH INGREDIENT INSERT/UPDATE/DELETE)
    // ---------------------------------------------------------
    public bool UpdateRecipe(Recipe r)
{
    var dbConn = new NpgsqlConnection(ConnectionString);
    dbConn.Open();
    var tx = dbConn.BeginTransaction();

    try
    {
        // 1) Update main recipe
        var cmd = dbConn.CreateCommand();
        cmd.Transaction = tx;

        cmd.CommandText = @"
            UPDATE recipe SET
                name = @name,
                cooking_time = @cooking_time,
                instructions = @instructions
            WHERE recipe_id = @id
        ";

        cmd.Parameters.AddWithValue("@name", r.Name);
        cmd.Parameters.AddWithValue("@cooking_time", r.CookingTime);
        cmd.Parameters.AddWithValue("@instructions",
            (object)r.Instructions ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@id", r.RecipeId);

        cmd.ExecuteNonQuery();

        // 2) Load existing ingredient IDs
        var existingIds = new List<int>();

        var cmdLoad = dbConn.CreateCommand();
        cmdLoad.Transaction = tx;
        cmdLoad.CommandText = "SELECT ingredient_id FROM ingredient WHERE recipe_id = @rid";
        cmdLoad.Parameters.AddWithValue("@rid", r.RecipeId);

        using (var reader = cmdLoad.ExecuteReader())
        {
            while (reader.Read())
                existingIds.Add((int)reader["ingredient_id"]);
        }

        var keepIds = new List<int>();

        // 3) Insert/update incoming ingredients
        if (r.Ingredients != null)
{
    foreach (var ing in r.Ingredients)
    {
        // NEW INGREDIENT
        if (ing.IngredientId == null || ing.IngredientId == 0)
        {
            var cmdInsert = dbConn.CreateCommand();
            cmdInsert.Transaction = tx;

            cmdInsert.CommandText = @"
                insert into ingredient (recipe_id, name, quantity)
                values (@rid, @name, @qty)
                returning ingredient_id
            ";

            cmdInsert.Parameters.AddWithValue("@rid", r.RecipeId);
            cmdInsert.Parameters.AddWithValue("@name", ing.Name);
            cmdInsert.Parameters.AddWithValue("@qty", (object?)ing.Quantity ?? DBNull.Value);

            int newId = (int)cmdInsert.ExecuteScalar();
            keepIds.Add(newId);
        }
        else
        {
            // EXISTING INGREDIENT
            keepIds.Add(ing.IngredientId.Value);

            var cmdUpdate = dbConn.CreateCommand();
            cmdUpdate.Transaction = tx;

            cmdUpdate.CommandText = @"
                update ingredient set
                    name = @name,
                    quantity = @qty
                where ingredient_id = @id
            ";

            cmdUpdate.Parameters.AddWithValue("@id", ing.IngredientId.Value);
            cmdUpdate.Parameters.AddWithValue("@name", ing.Name);
            cmdUpdate.Parameters.AddWithValue("@qty", (object?)ing.Quantity ?? DBNull.Value);

            cmdUpdate.ExecuteNonQuery();
        }
    }
}

        // 4) Delete removed ingredients
        var toDelete = existingIds.Except(keepIds).ToList();

        foreach (var delId in toDelete)
        {
            var cmdDelete = dbConn.CreateCommand();
            cmdDelete.Transaction = tx;

            cmdDelete.CommandText = "DELETE FROM ingredient WHERE ingredient_id = @id";
            cmdDelete.Parameters.AddWithValue("@id", delId);

            cmdDelete.ExecuteNonQuery();
        }

        tx.Commit();
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine("UpdateRecipe ERROR: " + ex.Message);
        tx.Rollback();
        return false;
    }
    finally
    {
        dbConn.Close();
    }
}


    // ---------------------------------------------------------
    // DELETE RECIPE
    // ---------------------------------------------------------
    public bool DeleteRecipe(int id)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();

        cmd.CommandText = "DELETE FROM recipe WHERE recipe_id = @id";
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

        return DeleteData(dbConn, cmd);
    }
}

