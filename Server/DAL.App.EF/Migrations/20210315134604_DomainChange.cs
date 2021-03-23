using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.App.EF.Migrations
{
    public partial class DomainChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "TypeValues",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "DataType",
                table: "AttributeTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AttributeTypes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Attributes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "TypeValues");

            migrationBuilder.DropColumn(
                name: "DataType",
                table: "AttributeTypes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AttributeTypes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Attributes");
        }
    }
}
