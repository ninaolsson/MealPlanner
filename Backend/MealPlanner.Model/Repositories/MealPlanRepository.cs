namespace MealPlanner.Model.Repositories;

using MealPlanner.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

public class MealPlanRepository : BaseRepository
{
    public MealPlanRepository(IConfiguration configuration) : base(configuration) { }


    //GET ONE MEAL PLAN ENTRY

    public MealPlan GetMealPlanById(int id)
    {
        NpgsqlConnection dbConn = null;

        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();

            cmd.CommandText = "select * from mealplan where meal_id = @id";
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

            var data = GetData(dbConn, cmd);

            if (data.Read())
            {
                return new MealPlan
                {
                    MealId = (int)data["meal_id"],
                    RecipeId = (int)data["recipe_id"],
                    DayOfWeek = data["day_of_week"].ToString(),
                    MealType = data["meal_type"].ToString()
                };
            }

            return null;
        }
        finally
        {
            dbConn?.Close();
        }
    }


    // GET MEAL PLANS FOR A SPECIFIC DAY

    public List<MealPlan> GetMealPlansByDay(string dayOfWeek)
    {
        NpgsqlConnection dbConn = null;
        var list = new List<MealPlan>();

        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);

            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
            select *
            from mealplan
            where day_of_week = @day
            order by meal_id
        ";

            cmd.Parameters.AddWithValue("@day", NpgsqlDbType.Text, dayOfWeek);

            var data = GetData(dbConn, cmd);

            while (data.Read())
            {
                list.Add(new MealPlan
                {
                    MealId = (int)data["meal_id"],
                    RecipeId = (int)data["recipe_id"],
                    DayOfWeek = data["day_of_week"].ToString()!,
                    MealType = data["meal_type"].ToString()!
                });
            }

            return list;
        }
        finally
        {
            dbConn?.Close();
        }
    }


    //GET MEAL PLAN FOR ENTIRE WEEK

    // GET MEAL PLAN FOR ENTIRE WEEK
public List<MealPlan> GetMealPlans()
{
    NpgsqlConnection dbConn = null;
    var list = new List<MealPlan>();

    try
    {
        dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();

        cmd.CommandText = @"
            SELECT 
                m.meal_id,
                m.recipe_id,
                m.day_of_week,
                m.meal_type,
                r.name AS recipe_name
            FROM mealplan m
            LEFT JOIN recipe r ON r.recipe_id = m.recipe_id
            ORDER BY m.meal_id;
        ";

        var data = GetData(dbConn, cmd);

        while (data.Read())
        {
            list.Add(new MealPlan
            {
                MealId = (int)data["meal_id"],
                RecipeId = (int)data["recipe_id"],
                DayOfWeek = data["day_of_week"].ToString(),
                MealType = data["meal_type"].ToString(),
                RecipeName = data["recipe_name"] == DBNull.Value 
                    ? null 
                    : data["recipe_name"].ToString()
            });
        }

        return list;
    }
    finally
    {
        dbConn?.Close();
    }
}



    //INSERT RECIPE INTO MEAL PLAN

    public bool InsertMealPlan(MealPlan m)
{
    var dbConn = new NpgsqlConnection(ConnectionString);
    var cmd = dbConn.CreateCommand();

    cmd.CommandText = @"
        INSERT INTO mealplan (recipe_id, day_of_week, meal_type)
        VALUES (@recipeId, @dayOfWeek::day_of_week_enum, @mealType::meal_type_enum);
    ";

    cmd.Parameters.AddWithValue("@recipeId", NpgsqlDbType.Integer, m.RecipeId);
    cmd.Parameters.AddWithValue("@dayOfWeek", NpgsqlDbType.Text, m.DayOfWeek);
    cmd.Parameters.AddWithValue("@mealType", NpgsqlDbType.Text, m.MealType);

    return InsertData(dbConn, cmd);
}


    //UPDATE AN EXISTING MEAL PLAN ENTRY

    public bool UpdateMealPlan(MealPlan m)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();

        cmd.CommandText = @"
            update mealplan set
                recipe_id = @recipe_id,
                day_of_week = @day_of_week,
                meal_type = @meal_type
            where meal_id = @id
        ";

        cmd.Parameters.AddWithValue("@recipe_id", NpgsqlDbType.Integer, m.RecipeId);
        cmd.Parameters.AddWithValue("@day_of_week", NpgsqlDbType.Text, m.DayOfWeek);
        cmd.Parameters.AddWithValue("@meal_type", NpgsqlDbType.Text, m.MealType);
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, m.MealId);

        return UpdateData(dbConn, cmd);
    }


    // DELETE FROM MEAL PLAN

    public bool DeleteMealPlan(int id)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();

        cmd.CommandText = "delete from mealplan where meal_id = @id";
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

        return DeleteData(dbConn, cmd);
    }
    
    //DELETE ALL MEAL PLANS (reset entire week)
public bool DeleteAllMealPlans()
{
    var dbConn = new NpgsqlConnection(ConnectionString);
    var cmd = dbConn.CreateCommand();

    cmd.CommandText = "DELETE FROM mealplan";

    return DeleteData(dbConn, cmd);
}
}

