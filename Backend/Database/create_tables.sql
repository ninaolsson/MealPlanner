--ENUM TYPES

create type meal_type_enum as enum ('Breakfast', 'Lunch', 'Dinner');

create type day_of_week_enum as enum ('Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday');

--TABLES

create table recipe
(recipe_id serial primary key,
name varchar(100) not null,
cooking_time int not null,
instructions text);

create table ingredient
(ingredient_id serial primary key,
recipe_id int not null,
name varchar(100) not null,
quantity varchar(50),

foreign key (recipe_id) references recipe (recipe_id)
	on delete cascade
);

create table mealplan
(meal_id serial primary key,
recipe_id int not null,
day_of_week day_of_week_enum not null,
meal_type meal_type_enum not null,

foreign key (recipe_id) references recipe (recipe_id)
	on delete cascade,
constraint unique_meal_per_day unique (day_of_week, meal_type)
);