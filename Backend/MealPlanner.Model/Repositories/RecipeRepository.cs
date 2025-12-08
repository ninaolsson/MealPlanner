namespace MealPlanner.Model.Repositories;

using System;

using MealPlanner.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;


public class RecipeRepository : BaseRepository
    {
        public RecipeRepository(IConfiguration configuration) : base(configuration) { }

        // ---------------------------------------------------------
        // GET ONE RECIPE BY ID (WITHOUT INGREDIENTS)
        // Ingredients are loaded in the controller via IngredientRepository
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

                        // Ingredients will be added later in controller
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
        // Ingredients will be loaded outside this repository
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
                        Ingredients = new List<Ingredient>() // init empty list
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
        // INSERT RECIPE
        // Modified: Returns TRUE and sets RecipeId on the object
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
            insert into recipe (name, cooking_time, instructions)
            values (@name, @cooking_time, @instructions)
            returning recipe_id
        ";

        cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, r.Name);
        cmd.Parameters.AddWithValue("@cooking_time", NpgsqlDbType.Integer, r.CookingTime);
        cmd.Parameters.AddWithValue("@instructions",
            r.Instructions == null ? DBNull.Value : r.Instructions);

        var newId = (int)cmd.ExecuteScalar();
        r.RecipeId = newId;

        // Insert ingredients
        if (r.Ingredients != null)
        {
            foreach (var ing in r.Ingredients)
            {
                var cmdIng = dbConn.CreateCommand();
                cmdIng.Transaction = transaction;

                cmdIng.CommandText = @"
                    insert into ingredient (recipe_id, name, quantity)
                    values (@recipe_id, @name, @quantity)
                ";

                cmdIng.Parameters.AddWithValue("@recipe_id", NpgsqlDbType.Integer, newId);
                cmdIng.Parameters.AddWithValue("@name", NpgsqlDbType.Text, ing.Name);
                cmdIng.Parameters.AddWithValue("@quantity",
                    ing.Quantity == null ? DBNull.Value : ing.Quantity);

                cmdIng.ExecuteNonQuery();
            }
        }

        transaction.Commit();
        return true;
    }
    catch
    {
        transaction.Rollback();
        return false;
    }
    finally
    {
        dbConn.Close();
    }
}

        // ---------------------------------------------------------
        // UPDATE RECIPE
        // ---------------------------------------------------------
        public bool UpdateRecipe(Recipe r)
{
    var dbConn = new NpgsqlConnection(ConnectionString);
    dbConn.Open();
    var transaction = dbConn.BeginTransaction();

    try
    {
        // Update recipe
        var cmd = dbConn.CreateCommand();
        cmd.Transaction = transaction;

        cmd.CommandText = @"
            update recipe set
                name = @name,
                cooking_time = @cooking_time,
                instructions = @instructions
            where recipe_id = @id
        ";

        cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, r.Name);
        cmd.Parameters.AddWithValue("@cooking_time", NpgsqlDbType.Integer, r.CookingTime);
        cmd.Parameters.AddWithValue("@instructions",
            r.Instructions == null ? DBNull.Value : r.Instructions);
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, r.RecipeId);

        cmd.ExecuteNonQuery();

        // Delete old ingredients
        var cmdDelete = dbConn.CreateCommand();
        cmdDelete.Transaction = transaction;
        cmdDelete.CommandText = "delete from ingredient where recipe_id = @id";
        cmdDelete.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, r.RecipeId);
        cmdDelete.ExecuteNonQuery();

        // Insert new ingredients
        if (r.Ingredients != null)
        {
            foreach (var ing in r.Ingredients)
            {
                var cmdIng = dbConn.CreateCommand();
                cmdIng.Transaction = transaction;

                cmdIng.CommandText = @"
                    insert into ingredient (recipe_id, name, quantity)
                    values (@recipe_id, @name, @quantity)
                ";

                cmdIng.Parameters.AddWithValue("@recipe_id", NpgsqlDbType.Integer, r.RecipeId);
                cmdIng.Parameters.AddWithValue("@name", NpgsqlDbType.Text, ing.Name);
                cmdIng.Parameters.AddWithValue("@quantity",
                    ing.Quantity == null ? DBNull.Value : ing.Quantity);

                cmdIng.ExecuteNonQuery();
            }
        }

        transaction.Commit();
        return true;
    }
    catch
    {
        transaction.Rollback();
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
