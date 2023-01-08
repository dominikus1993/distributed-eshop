﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Catalog.Infrastructure.Migrations
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
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description" }),
                    datecreated = table.Column<DateTime>(name: "date_created", type: "timestamp with time zone", nullable: false, defaultValueSql: "(now() at time zone 'utc')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.productid);
                });

            migrationBuilder.CreateIndex(
                name: "ix_products_date_created",
                table: "products",
                column: "date_created");

            migrationBuilder.CreateIndex(
                name: "ix_products_price_promotional_price",
                table: "products",
                columns: new[] { "price", "promotional_price" });

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
