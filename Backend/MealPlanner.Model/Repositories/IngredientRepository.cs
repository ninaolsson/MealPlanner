namespace MealPlanner.Model.Repositories;

using MealPlanner.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

public class IngredientRepository : BaseRepository
    {
        public IngredientRepository(IConfiguration configuration) : base(configuration) { }

        //Get ingredient by id
        public Ingredient GetIngredientById(int id)
        {
            NpgsqlConnection dbConn = null;

            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();

                cmd.CommandText = "SELECT * FROM ingredient WHERE ingredient_id = @id";
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

                var data = GetData(dbConn, cmd);

                if (data.Read())
                {
                    return new Ingredient
                    {
                        IngredientId = (int)data["ingredient_id"],
                        RecipeId = (int)data["recipe_id"],
                        Name = data["name"].ToString(),
                        Quantity = data["quantity"] == DBNull.Value
                            ? null
                            : data["quantity"].ToString()
                    };
                }

                return null;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        //Get ingredients by recipe id
        public List<Ingredient> GetIngredientsByRecipeId(int recipeId)
        {
            NpgsqlConnection dbConn = null;
            var list = new List<Ingredient>();

            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();

                cmd.CommandText = "SELECT * FROM ingredient WHERE recipe_id = @recipe_id ORDER BY ingredient_id";
                cmd.Parameters.AddWithValue("@recipe_id", NpgsqlDbType.Integer, recipeId);

                var data = GetData(dbConn, cmd);

                while (data.Read())
                {
                    list.Add(new Ingredient
                    {
                        IngredientId = (int)data["ingredient_id"],
                        RecipeId = (int)data["recipe_id"],
                        Name = data["name"].ToString(),
                        Quantity = data["quantity"] == DBNull.Value
                            ? null
                            : data["quantity"].ToString()
                    });
                }

                return list;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        //Get all ingredients
        public List<Ingredient> GetAllIngredients()
        {
            NpgsqlConnection dbConn = null;
            var list = new List<Ingredient>();

            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();

                cmd.CommandText = "SELECT * FROM ingredient ORDER BY ingredient_id";

                var data = GetData(dbConn, cmd);

                while (data.Read())
                {
                    list.Add(new Ingredient
                    {
                        IngredientId = (int)data["ingredient_id"],
                        RecipeId = (int)data["recipe_id"],
                        Name = data["name"].ToString(),
                        Quantity = data["quantity"] == DBNull.Value
                            ? null
                            : data["quantity"].ToString()
                    });
                }

                return list;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        //Insert ingredient
        public bool InsertIngredient(Ingredient i)
        {
                var dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();

                cmd.CommandText = @"
                    INSERT INTO ingredient (recipe_id, name, quantity)
                    VALUES (@recipe_id, @name, @quantity);
                ";

                cmd.Parameters.AddWithValue("@recipe_id", NpgsqlDbType.Integer, i.RecipeId);
                cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, i.Name);

            if (i.Quantity == null)
                cmd.Parameters.AddWithValue("@quantity", DBNull.Value);
            else
                    cmd.Parameters.AddWithValue("@quantity", NpgsqlDbType.Text, i.Quantity);

            return InsertData(dbConn, cmd);
        }

        // Update Ingredient
        public bool UpdateIngredient(Ingredient i)
        {
                var dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();

                cmd.CommandText = @"
                UPDATE ingredient SET
                    recipe_id = @recipe_id,
                    name = @name,
                    quantity = @quantity
                WHERE ingredient_id = @id;
                ";

                cmd.Parameters.AddWithValue("@recipe_id", NpgsqlDbType.Integer, i.RecipeId);
                cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, i.Name);
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, i.IngredientId);

            if (i.Quantity == null)
                cmd.Parameters.AddWithValue("@quantity", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@quantity", NpgsqlDbType.Text, i.Quantity);

            return UpdateData(dbConn, cmd);
        }

        //Delete one ingredient by id
        public bool DeleteIngredient(int id)
        {
                var dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();

                cmd.CommandText = "DELETE FROM ingredient WHERE ingredient_id = @id";
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

            return DeleteData(dbConn, cmd);
        }

        // Delete all ingredients by recipe id
        public bool DeleteIngredientsByRecipeId(int recipeId)
        {
                var dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();

                cmd.CommandText = "DELETE FROM ingredient WHERE recipe_id = @recipe_id";
                cmd.Parameters.AddWithValue("@recipe_id", NpgsqlDbType.Integer, recipeId);

            return DeleteData(dbConn, cmd);
        }
    }


