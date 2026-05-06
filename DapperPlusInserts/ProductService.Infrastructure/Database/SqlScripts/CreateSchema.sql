IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'products_identity')
    EXEC('CREATE SCHEMA products_identity');
GO

IF OBJECT_ID('products_identity.users', 'U') IS NULL
BEGIN
    CREATE TABLE products_identity.users
    (
        id       INT IDENTITY(1,1) NOT NULL,
        username NVARCHAR(50)      NOT NULL,
        email    NVARCHAR(100)     NOT NULL,
        CONSTRAINT pk_users PRIMARY KEY (id)
    );
END
GO

IF OBJECT_ID('products_identity.products', 'U') IS NULL
BEGIN
    CREATE TABLE products_identity.products
    (
        id             INT IDENTITY(1,1)               NOT NULL,
        name           NVARCHAR(250)                   NOT NULL,
        price          DECIMAL(18, 2)                  NOT NULL,
        description    NVARCHAR(1000)                  NOT NULL,
        sku            NVARCHAR(50)                    NOT NULL,
        barcode        NVARCHAR(20)                    NOT NULL,
        category       NVARCHAR(100)                   NOT NULL,
        brand          NVARCHAR(100)                   NOT NULL,
        manufacturer   NVARCHAR(100)                   NOT NULL,
        stock_quantity INT                             NOT NULL,
        weight         DECIMAL(10, 3)                  NOT NULL,
        is_active      BIT             DEFAULT (1)     NOT NULL,
        created_at     DATETIME2                       NOT NULL,
        updated_at     DATETIME2                       NULL,
        CONSTRAINT pk_products PRIMARY KEY (id)
    );

    CREATE UNIQUE INDEX ix_products_sku
        ON products_identity.products (sku);
END
GO

IF OBJECT_ID('products_identity.product_carts', 'U') IS NULL
BEGIN
    CREATE TABLE products_identity.product_carts
    (
        id         INT IDENTITY(1,1) NOT NULL,
        quantity   INT               NOT NULL,
        user_id    INT               NOT NULL,
        created_on DATETIME2         NOT NULL,
        CONSTRAINT pk_product_carts PRIMARY KEY (id),
        CONSTRAINT fk_product_carts_users_user_id
            FOREIGN KEY (user_id)
                REFERENCES products_identity.users (id)
                ON DELETE CASCADE
    );

    CREATE INDEX ix_product_carts_user_id
        ON products_identity.product_carts (user_id);
END
GO

IF OBJECT_ID('products_identity.product_cart_items', 'U') IS NULL
BEGIN
    CREATE TABLE products_identity.product_cart_items
    (
        id              INT IDENTITY(1,1)         NOT NULL,
        product_cart_id INT                       NOT NULL,
        product_id      INT                       NOT NULL,
        quantity        INT      DEFAULT (1)      NOT NULL,
        CONSTRAINT pk_product_cart_items PRIMARY KEY (id),
        CONSTRAINT fk_product_cart_items_product_carts_product_cart_id
            FOREIGN KEY (product_cart_id)
                REFERENCES products_identity.product_carts (id)
                ON DELETE CASCADE,
        CONSTRAINT fk_product_cart_items_products_product_id
            FOREIGN KEY (product_id)
                REFERENCES products_identity.products (id)
                ON DELETE NO ACTION
    );

    CREATE INDEX ix_product_cart_items_product_cart_id
        ON products_identity.product_cart_items (product_cart_id);

    CREATE INDEX ix_product_cart_items_product_id
        ON products_identity.product_cart_items (product_id);
END
GO
