--TEST DATA

-- Sample recipes

INSERT INTO recipe (name, cooking_time, instructions) VALUES
('Oatmeal Bowl', 10, 'Mix oats with milk, cook for 10 minutes, and top with sliced fruit.'),
('Chicken Salad', 15, 'Chop chicken and vegetables, mix with olive oil and seasoning.'),
('Pasta Bolognese', 30, 'Cook pasta, prepare meat sauce, mix and serve warm.'),
('Smoothie Bowl', 5, 'Blend fruits and yogurt, pour into a bowl, and top with granola.'),
('Grilled Salmon', 20, 'Season salmon with salt and lemon, grill for 10 minutes per side.'),
('Vegetable Stir-Fry', 15, 'Stir-fry mixed vegetables with soy sauce, serve with cooked rice.'),
('Beef Tacos', 25, 'Cook minced beef with spices, fill taco shells, and add toppings.'),
('Avocado Toast', 5, 'Toast bread, spread mashed avocado, and season with salt and pepper.'),
('Tomato Soup', 30, 'Cook tomatoes with onion and garlic, blend until smooth, and serve hot.'),
('Fruit Salad', 10, 'Chop mixed fruits, combine gently, and drizzle with honey.');

--Ingredients per recipe

INSERT INTO ingredient (recipe_id, name, quantity) VALUES

-- 1. Oatmeal Bowl
(1, 'Oats', '80 g'),
(1, 'Milk', '200 ml'),
(1, 'Banana', '1 whole'),

-- 2. Chicken Salad
(2, 'Cooked Chicken Breast', '150 g'),
(2, 'Lettuce', '60 g'),
(2, 'Olive Oil', '15 ml'),

-- 3. Pasta Bolognese
(3, 'Pasta', '100 g'),
(3, 'Minced Beef', '120 g'),
(3, 'Tomato Sauce', '150 ml'),

-- 4. Smoothie Bowl
(4, 'Yogurt', '200 ml'),
(4, 'Mixed Berries', '75 g'),
(4, 'Granola', '40 g'),

-- 5. Grilled Salmon
(5, 'Salmon Fillet', '200 g'),
(5, 'Lemon Juice', '10 ml'),
(5, 'Salt', '2 g'),

-- 6. Vegetable Stir-Fry
(6, 'Broccoli', '100 g'),
(6, 'Carrot', '80 g'),
(6, 'Soy Sauce', '15 ml'),
(6, 'Cooked Rice', '150 g'),

-- 7. Beef Tacos
(7, 'Minced Beef', '150 g'),
(7, 'Taco Shells', '3 whole'),
(7, 'Lettuce', '40 g'),

-- 8. Avocado Toast
(8, 'Bread', '2 slices'),
(8, 'Avocado', '1 whole'),
(8, 'Salt', '1 g'),
(8, 'Black Pepper', '0.5 g'),

-- 9. Tomato Soup
(9, 'Tomatoes', '400 g'),
(9, 'Onion', '80 g'),
(9, 'Garlic', '2 cloves'),
(9, 'Vegetable Stock', '250 ml'),

-- 10. Fruit Salad
(10, 'Apple', '1 whole'),
(10, 'Banana', '1 whole'),
(10, 'Orange', '1 whole'),
(10, 'Honey', '10 ml');

--Meal plan entries
INSERT INTO mealplan (recipe_id, day_of_week, meal_type) VALUES
(1, 'Monday', 'Breakfast'),
(2, 'Monday', 'Lunch'),
(3, 'Monday', 'Dinner'),
(4, 'Tuesday', 'Breakfast'),
(5, 'Tuesday', 'Lunch'),
(6, 'Tuesday', 'Dinner'),
(7, 'Wednesday', 'Dinner'),
(8, 'Wednesday', 'Breakfast'),
(9, 'Thursday', 'Lunch'),
(10, 'Friday', 'Breakfast');
