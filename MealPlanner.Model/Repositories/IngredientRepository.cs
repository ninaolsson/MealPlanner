namespace MealPlanner.Model.Repositories;

using MealPlanner.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

public class IngredientRepository : BaseRepository
{
    public IngredientRepository(IConfiguration configuration) : base(configuration) { }

    //GET ONE INGREDIENT
    public Ingredient GetIngredientById(int id)
    {
        NpgsqlConnection dbConn = null;

        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();

            cmd.CommandText = "select * from ingredient where ingredient_id = @id";
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

    
    //GET ALL INGREDIENTS FOR A SPECIFIC RECIPE
    
    public List<Ingredient> GetIngredientsByRecipeId(int recipeId)
    {
        NpgsqlConnection dbConn = null;
        var list = new List<Ingredient>();

        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();

            cmd.CommandText = "select * from ingredient where recipe_id = @recipe_id";
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

    
    //GET ALL INGREDIENTS IN THE DATABASE
    
    public List<Ingredient> GetAllIngredients()
    {
        NpgsqlConnection dbConn = null;
        var list = new List<Ingredient>();

        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();

            cmd.CommandText = "select * from ingredient order by ingredient_id";

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

   
    //INSERT A NEW INGREDIENT INTO A RECIPE

    public bool InsertIngredient(Ingredient i)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();

        cmd.CommandText = @"
            insert into ingredient (recipe_id, name, quantity)
            values (@recipe_id, @name, @quantity)
        ";

        cmd.Parameters.AddWithValue("@recipe_id", NpgsqlDbType.Integer, i.RecipeId);
        cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, i.Name);

        if (i.Quantity == null)
            cmd.Parameters.AddWithValue("@quantity", DBNull.Value);
        else
            cmd.Parameters.AddWithValue("@quantity", NpgsqlDbType.Text, i.Quantity);

        return InsertData(dbConn, cmd);
    }

    
    //UPDATE AN EXISTING INGREDIENT'S DETAILS
    
    public bool UpdateIngredient(Ingredient i)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();

        cmd.CommandText = @"
            update ingredient set
                recipe_id = @recipe_id,
                name = @name,
                quantity = @quantity
            where ingredient_id = @id
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

    
    // DELETE AN INGREDIENT
    
    public bool DeleteIngredient(int id)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();

        cmd.CommandText = "delete from ingredient where ingredient_id = @id";
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

        return DeleteData(dbConn, cmd);
    }
}


