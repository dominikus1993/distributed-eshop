﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Catalog.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    [DbContext(typeof(ProductsDbContext))]
    partial class ProductsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Catalog.Infrastructure.Model.EfProduct", b =>
                {
                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_id");

                    b.Property<int>("AvailableQuantity")
                        .HasColumnType("integer")
                        .HasColumnName("available_quantity");

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_created")
                        .HasDefaultValueSql("(now() at time zone 'utc')");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric")
                        .HasColumnName("price");

                    b.Property<decimal?>("PromotionalPrice")
                        .HasColumnType("numeric")
                        .HasColumnName("promotional_price");

                    b.Property<NpgsqlTsVector>("SearchVector")
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("tsvector")
                        .HasColumnName("search_vector")
                        .HasAnnotation("Npgsql:TsVectorConfig", "english")
                        .HasAnnotation("Npgsql:TsVectorProperties", new[] { "Name", "Description", "TagsIndex" });

                    b.Property<List<string>>("Tags")
                        .HasColumnType("text[]")
                        .HasColumnName("tags");

                    b.Property<string>("TagsIndex")
                        .HasColumnType("text")
                        .HasColumnName("tags_index");

                    b.HasKey("ProductId")
                        .HasName("pk_products");

                    b.HasIndex("DateCreated")
                        .HasDatabaseName("ix_products_date_created");

                    b.HasIndex("SearchVector")
                        .HasDatabaseName("ix_products_search_vector");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("SearchVector"), "GIN");

                    b.HasIndex("Price", "PromotionalPrice")
                        .HasDatabaseName("ix_products_price_promotional_price");

                    b.ToTable("products", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
