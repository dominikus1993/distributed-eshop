using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Catalog.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    productid = table.Column<Guid>(name: "product_id", type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    promotionalprice = table.Column<decimal>(name: "promotional_price", type: "numeric", nullable: true),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    availablequantity = table.Column<int>(name: "available_quantity", type: "integer", nullable: false),
                    searchvector = table.Column<NpgsqlTsVector>(name: "search_vector", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.productid);
                });

            migrationBuilder.CreateIndex(
                name: "ix_products_search_vector",
                table: "products",
                column: "search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products");
        }
    }
}
