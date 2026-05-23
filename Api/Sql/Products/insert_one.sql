INSERT INTO "products" ("name", "description", "price")
VALUES (@Name, @Description, @Price)
RETURNING "id", "name", "description", "price";
