-- migrate:up
CREATE TABLE IF NOT EXISTS "products" (
    "id" INT NOT NULL GENERATED ALWAYS AS IDENTITY,
    "name" VARCHAR(255) NOT NULL,
    "price" DECIMAL(10, 2) NOT NULL,
    "description" TEXT,
    "created_at" TIMESTAMPTZ NOT NULL DEFAULT now(),
    "updated_at" TIMESTAMPTZ NOT NULL DEFAULT now(),
    CONSTRAINT "products_pkey" PRIMARY KEY ("id")
);

CREATE INDEX IF NOT EXISTS "idx_product_name" ON "products"("name");
CREATE INDEX IF NOT EXISTS "idx_product_name_like" ON "products" USING gin ("name" gin_trgm_ops);

CREATE OR REPLACE TRIGGER "tgr_products_updated_at"
    BEFORE UPDATE ON "products"
    FOR EACH ROW
    EXECUTE PROCEDURE moddatetime("updated_at");

-- migrate:down
DROP TABLE IF EXISTS "products";
