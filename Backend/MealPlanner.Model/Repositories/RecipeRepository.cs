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

    public bool ExistsByName(string name, int? excludeId = null)    
    {
    using var dbConn = new NpgsqlConnection(ConnectionString);
    dbConn.Open();
    using var cmd = dbConn.CreateCommand();

    if (excludeId.HasValue)
    {
        cmd.CommandText = @"
            SELECT COUNT(*) FROM recipe
            WHERE lower(name) = lower(@name) AND recipe_id <> @excludeId
        ";
        cmd.Parameters.AddWithValue("@excludeId", NpgsqlDbType.Integer, excludeId.Value);
        }
    else
        {
        cmd.CommandText = @"
            SELECT COUNT(*) FROM recipe
            WHERE lower(name) = lower(@name)
        ";
        }

    cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, name);

    var cntObj = cmd.ExecuteScalar();
    var cnt = cntObj == null ? 0L : Convert.ToInt64(cntObj);
    return cnt > 0;
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
    using var dbConn = new NpgsqlConnection(ConnectionString);
    dbConn.Open();
    using var tx = dbConn.BeginTransaction();

    try
    {
        // 1) Obtain advisory lock based on recipe name (xact-lock so it's released on commit/rollback)
        using (var cmdLock = dbConn.CreateCommand())
        {
            cmdLock.Transaction = tx;
            cmdLock.CommandText = @"
                SELECT pg_advisory_xact_lock(
                  ('x' || substr(md5(@name), 1, 16))::bit(64)::bigint
                )";
            cmdLock.Parameters.AddWithValue("@name", NpgsqlDbType.Text, r.Name ?? string.Empty);
            cmdLock.ExecuteNonQuery();
        }

        // 2) Double-check that name does not exist (case-insensitive)
        using (var cmdCheck = dbConn.CreateCommand())
        {
            cmdCheck.Transaction = tx;
            cmdCheck.CommandText = "SELECT COUNT(*) FROM recipe WHERE lower(name) = lower(@name)";
            cmdCheck.Parameters.AddWithValue("@name", NpgsqlDbType.Text, r.Name ?? string.Empty);
            var exists = Convert.ToInt64(cmdCheck.ExecuteScalar()) > 0;
            if (exists)
            {
                tx.Rollback();
                return false; // caller (controller) should treat this as "409 Conflict"
            }
        }

        // 3) Insert recipe (same transaction)
        using (var cmd = dbConn.CreateCommand())
        {
            cmd.Transaction = tx;
            cmd.CommandText = @"
                INSERT INTO recipe (name, cooking_time, instructions)
                VALUES (@name, @cooking_time, @instructions)
                RETURNING recipe_id
            ";
            cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, r.Name ?? string.Empty);
            cmd.Parameters.AddWithValue("@cooking_time", NpgsqlDbType.Integer, r.CookingTime);
            cmd.Parameters.AddWithValue("@instructions", (object?)r.Instructions ?? DBNull.Value);

            int newId = (int)cmd.ExecuteScalar();
            r.RecipeId = newId;
        }

        // 4) Insert ingredients (if any) — same as din eksisterende logic
        if (r.Ingredients != null)
        {
            foreach (var ing in r.Ingredients)
            {
                using var cmdIng = dbConn.CreateCommand();
                cmdIng.Transaction = tx;
                cmdIng.CommandText = @"
                    INSERT INTO ingredient (recipe_id, name, quantity)
                    VALUES (@recipe_id, @name, @quantity)
                ";
                cmdIng.Parameters.AddWithValue("@recipe_id", NpgsqlDbType.Integer, r.RecipeId);
                cmdIng.Parameters.AddWithValue("@name", NpgsqlDbType.Text, ing.Name ?? string.Empty);
                cmdIng.Parameters.AddWithValue("@quantity", (object?)ing.Quantity ?? DBNull.Value);
                cmdIng.Parameters["@quantity"].NpgsqlDbType = NpgsqlDbType.Varchar;
                cmdIng.ExecuteNonQuery();
            }
        }

        tx.Commit();
        return true;
    }
    catch (PostgresException pgEx) when (pgEx.SqlState == "23505")
    {
        // Hvis DB-side unique constraint på et tidspunkt findes, håndter det pænt
        tx.Rollback();
        Console.WriteLine("Insert unique violation: " + pgEx.Message);
        return false;
    }
    catch (Exception ex)
    {
        tx.Rollback();
        Console.WriteLine("InsertRecipe ERROR: " + ex.Message);
        return false;
    }
}

    // ---------------------------------------------------------
    // UPDATE RECIPE (WITH INGREDIENT INSERT/UPDATE/DELETE)
    // ---------------------------------------------------------
public bool UpdateRecipe(Recipe r)
{
    using var dbConn = new NpgsqlConnection(ConnectionString);
    dbConn.Open();
    using var tx = dbConn.BeginTransaction();

    try
    {
        // Acquire advisory lock on the new name to prevent race
        using (var cmdLock = dbConn.CreateCommand())
        {
            cmdLock.Transaction = tx;
            cmdLock.CommandText = @"
                SELECT pg_advisory_xact_lock(
                  ('x' || substr(md5(@name), 1, 16))::bit(64)::bigint
                )";
            cmdLock.Parameters.AddWithValue("@name", NpgsqlDbType.Text, r.Name ?? string.Empty);
            cmdLock.ExecuteNonQuery();
        }

        // Check for existing recipe with same name but different id
        using (var cmdCheck = dbConn.CreateCommand())
        {
            cmdCheck.Transaction = tx;
            cmdCheck.CommandText = @"
                SELECT COUNT(*) FROM recipe
                WHERE lower(name) = lower(@name) AND recipe_id <> @id
            ";
            cmdCheck.Parameters.AddWithValue("@name", NpgsqlDbType.Text, r.Name ?? string.Empty);
            cmdCheck.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, r.RecipeId);
            var exists = Convert.ToInt64(cmdCheck.ExecuteScalar()) > 0;
            if (exists)
            {
                tx.Rollback();
                return false; 
            }
        }

        // 1) Update main recipe
        using (var cmd = dbConn.CreateCommand())
        {
            cmd.Transaction = tx;
            cmd.CommandText = @"
                UPDATE recipe SET
                    name = @name,
                    cooking_time = @cooking_time,
                    instructions = @instructions
                WHERE recipe_id = @id
            ";
            cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, r.Name ?? string.Empty);
            cmd.Parameters.AddWithValue("@cooking_time", NpgsqlDbType.Integer, r.CookingTime);
            cmd.Parameters.AddWithValue("@instructions", (object?)r.Instructions ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, r.RecipeId);
            cmd.ExecuteNonQuery();
        }

        // 2) Load existing ingredient IDs (within same tx)
        var existingIds = new List<int>();
        using (var cmdLoad = dbConn.CreateCommand())
        {
            cmdLoad.Transaction = tx;
            cmdLoad.CommandText = "SELECT ingredient_id FROM ingredient WHERE recipe_id = @rid";
            cmdLoad.Parameters.AddWithValue("@rid", NpgsqlDbType.Integer, r.RecipeId);

            using (var reader = cmdLoad.ExecuteReader())
            {
                while (reader.Read())
                    existingIds.Add((int)reader["ingredient_id"]);
            }
        }

        var keepIds = new List<int>();

        // 3) Insert/update incoming ingredients
        if (r.Ingredients != null)
        {
            foreach (var ing in r.Ingredients)
            {
                if (ing.IngredientId == null || ing.IngredientId == 0)
                {
                    using (var cmdInsert = dbConn.CreateCommand())
                    {
                        cmdInsert.Transaction = tx;
                        cmdInsert.CommandText = @"
                            INSERT INTO ingredient (recipe_id, name, quantity)
                            VALUES (@rid, @name, @qty)
                            RETURNING ingredient_id
                        ";
                        cmdInsert.Parameters.AddWithValue("@rid", NpgsqlDbType.Integer, r.RecipeId);
                        cmdInsert.Parameters.AddWithValue("@name", NpgsqlDbType.Text, ing.Name ?? string.Empty);
                        cmdInsert.Parameters.AddWithValue("@qty", (object?)ing.Quantity ?? DBNull.Value);
                        cmdInsert.Parameters["@qty"].NpgsqlDbType = NpgsqlDbType.Varchar;

                        int newId = (int)cmdInsert.ExecuteScalar();
                        keepIds.Add(newId);
                    }
                }
                else
                {
                    keepIds.Add(ing.IngredientId.Value);

                    using (var cmdUpdate = dbConn.CreateCommand())
                    {
                        cmdUpdate.Transaction = tx;
                        cmdUpdate.CommandText = @"
                            UPDATE ingredient SET
                                name = @name,
                                quantity = @qty
                            WHERE ingredient_id = @id
                        ";
                        cmdUpdate.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, ing.IngredientId.Value);
                        cmdUpdate.Parameters.AddWithValue("@name", NpgsqlDbType.Text, ing.Name ?? string.Empty);
                        cmdUpdate.Parameters.AddWithValue("@qty", (object?)ing.Quantity ?? DBNull.Value);
                        cmdUpdate.Parameters["@qty"].NpgsqlDbType = NpgsqlDbType.Varchar;

                        cmdUpdate.ExecuteNonQuery();
                    }
                }
            }
        }

        // 4) Delete removed ingredients
        var toDelete = existingIds.Except(keepIds).ToList();
        foreach (var delId in toDelete)
        {
            using (var cmdDelete = dbConn.CreateCommand())
            {
                cmdDelete.Transaction = tx;
                cmdDelete.CommandText = "DELETE FROM ingredient WHERE ingredient_id = @id";
                cmdDelete.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, delId);
                cmdDelete.ExecuteNonQuery();
            }
        }

        tx.Commit();
        return true;
    }
    catch (PostgresException pgEx) when (pgEx.SqlState == "23505")
    {
        tx.Rollback();
        Console.WriteLine("Update unique violation: " + pgEx.Message);
        return false;
    }
    catch (Exception ex)
    {
        tx.Rollback();
        Console.WriteLine("UpdateRecipe ERROR: " + ex.Message);
        return false;
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

