using MealPlanner.Model.Entities;
using MealPlanner.Model.Repositories;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add repositories
builder.Services.AddScoped<RecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IngredientRepository, IngredientRepository>();
builder.Services.AddScoped<MealPlanRepository, MealPlanRepository>();

var app = builder.Build();

// Use CORS
app.UseCors("AllowAngular");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();