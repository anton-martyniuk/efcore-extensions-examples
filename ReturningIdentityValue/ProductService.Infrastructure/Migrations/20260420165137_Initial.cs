using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "products_identity");

            migrationBuilder.CreateTable(
                name: "products",
                schema: "products_identity",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    sku = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    barcode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    stock_quantity = table.Column<int>(type: "int", nullable: false),
                    weight = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "products_identity",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product_carts",
                schema: "products_identity",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    created_on = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_carts", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_carts_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "products_identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_cart_items",
                schema: "products_identity",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_cart_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_cart_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_cart_items_product_carts_product_cart_id",
                        column: x => x.product_cart_id,
                        principalSchema: "products_identity",
                        principalTable: "product_carts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_product_cart_items_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "products_identity",
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_product_cart_items_product_cart_id",
                schema: "products_identity",
                table: "product_cart_items",
                column: "product_cart_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_cart_items_product_id",
                schema: "products_identity",
                table: "product_cart_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_carts_user_id",
                schema: "products_identity",
                table: "product_carts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_sku",
                schema: "products_identity",
                table: "products",
                column: "sku",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_cart_items",
                schema: "products_identity");

            migrationBuilder.DropTable(
                name: "product_carts",
                schema: "products_identity");

            migrationBuilder.DropTable(
                name: "products",
                schema: "products_identity");

            migrationBuilder.DropTable(
                name: "users",
                schema: "products_identity");
        }
    }
}
