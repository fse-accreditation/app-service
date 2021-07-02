﻿// <auto-generated />
using System;
using CompaniesAPI.DBContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CompaniesAPI.Migrations
{
    [DbContext(typeof(CompanyContext))]
    [Migration("20210627195703_DeleteCascade")]
    partial class DeleteCascade
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CompaniesAPI.Models.Company", b =>
                {
                    b.Property<string>("CompanyCode")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CompanyCEO")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CompanyTurnOver")
                        .HasColumnType("int");

                    b.Property<string>("Website")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CompanyCode");

                    b.ToTable("Companies");

                    b.HasData(
                        new
                        {
                            CompanyCode = "Code1",
                            CompanyCEO = "XYZ",
                            CompanyName = "ABC",
                            CompanyTurnOver = 100,
                            Website = "website1"
                        });
                });

            modelBuilder.Entity("CompaniesAPI.Models.Stock", b =>
                {
                    b.Property<int>("StockId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CompanyCode")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("StockDateTime")
                        .HasColumnType("datetime2");

                    b.Property<double>("StockPrice")
                        .HasColumnType("float");

                    b.HasKey("StockId");

                    b.HasIndex("CompanyCode");

                    b.ToTable("Stocks");

                    b.HasData(
                        new
                        {
                            StockId = 101,
                            CompanyCode = "Code1",
                            StockDateTime = new DateTime(2021, 6, 28, 1, 27, 1, 896, DateTimeKind.Local).AddTicks(3800),
                            StockPrice = 100.89
                        });
                });

            modelBuilder.Entity("CompaniesAPI.Models.Stock", b =>
                {
                    b.HasOne("CompaniesAPI.Models.Company", "Company")
                        .WithMany("Stocks")
                        .HasForeignKey("CompanyCode")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Company");
                });

            modelBuilder.Entity("CompaniesAPI.Models.Company", b =>
                {
                    b.Navigation("Stocks");
                });
#pragma warning restore 612, 618
        }
    }
}
