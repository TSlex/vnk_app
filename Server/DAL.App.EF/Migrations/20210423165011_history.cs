using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.App.EF.Migrations
{
    public partial class history : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TypeValues",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "TypeValues",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TypeUnits",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "TypeUnits",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MasterId",
                table: "OrderAttributes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MasterId",
                table: "Attributes",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TypeValues");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "TypeValues");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TypeUnits");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "TypeUnits");

            migrationBuilder.DropColumn(
                name: "MasterId",
                table: "OrderAttributes");

            migrationBuilder.DropColumn(
                name: "MasterId",
                table: "Attributes");
        }
    }
}
