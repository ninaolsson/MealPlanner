namespace MealPlanner.Model.Repositories;

using MealPlanner.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

    public class MealPlanRepository : BaseRepository
    {
        public MealPlanRepository(IConfiguration configuration) : base(configuration) { }

        //Get one meal plan entry by id
        public MealPlan GetMealPlanById(int id)
        {
            NpgsqlConnection dbConn = null;

            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();

                cmd.CommandText = @"
                    SELECT 
                        mp.meal_id,
                        mp.recipe_id,
                        r.name AS recipe_name,
                        mp.day_of_week,
                        mp.meal_type
                    FROM mealplan mp
                    JOIN recipe r ON mp.recipe_id = r.recipe_id
                    WHERE mp.meal_id = @id;
                ";

                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

                var data = GetData(dbConn, cmd);

                if (data.Read())
                {
                    return new MealPlan
                    {
                        MealId = (int)data["meal_id"],
                        RecipeId = (int)data["recipe_id"],
                        RecipeName = data["recipe_name"].ToString(),
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


        //Get meal plans for a specific day of the week
        public List<MealPlan> GetMealPlansByDay(string dayOfWeek)
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
                    WHERE m.day_of_week = @day
                    ORDER BY m.meal_id;
                ";

                cmd.Parameters.AddWithValue("@day", NpgsqlDbType.Text, dayOfWeek);

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


        //Get all meal plans
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


    // Insert new meal plan entry
        public bool InsertMealPlan(MealPlan m)
        {
                var dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();

                cmd.CommandText = @"
                    INSERT INTO mealplan (recipe_id, day_of_week, meal_type)
                    VALUES (
                    @recipeId,
                    @dayOfWeek::day_of_week_enum,
                    @mealType::meal_type_enum
                    );
                ";

            cmd.Parameters.AddWithValue("@recipeId", m.RecipeId);
            cmd.Parameters.AddWithValue("@dayOfWeek", m.DayOfWeek.ToString());
            cmd.Parameters.AddWithValue("@mealType", m.MealType.ToString());

            return InsertData(dbConn, cmd);
        }
        



    // Update meal plan entry
    public bool UpdateMealPlan(MealPlan m)
    {
        try
        {
            var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();

            cmd.CommandText = @"
                UPDATE mealplan SET
                recipe_id = @recipe_id,
                day_of_week = @day_of_week::day_of_week_enum,
                meal_type = @meal_type::meal_type_enum
                WHERE meal_id = @id
                ";

            cmd.Parameters.AddWithValue("@recipe_id", NpgsqlDbType.Integer, m.RecipeId);
            cmd.Parameters.AddWithValue("@day_of_week", NpgsqlDbType.Text, m.DayOfWeek);
            cmd.Parameters.AddWithValue("@meal_type", NpgsqlDbType.Text, m.MealType);
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, m.MealId);

            return UpdateData(dbConn, cmd);
        }
        catch (Exception ex)
        {
            Console.WriteLine("=== MEAL PLAN UPDATE ERROR ===");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            return false;
        }
    }


        //Delete one entry
        public bool DeleteMealPlan(int id)
        {
            var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();

            cmd.CommandText = "DELETE FROM mealplan WHERE meal_id = @id";
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

            return DeleteData(dbConn, cmd);
        }


        // Delete all meal plans/reset week
        public bool DeleteAllMealPlans()
        {
            var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();

            cmd.CommandText = "DELETE FROM mealplan";

            return DeleteData(dbConn, cmd);
        }
    }
