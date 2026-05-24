UPDATE "products"
/**set**/
WHERE "id"= @Id
RETURNING "id", "name", "description", "price";
