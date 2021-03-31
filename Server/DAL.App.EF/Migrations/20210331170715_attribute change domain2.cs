using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.App.EF.Migrations
{
    public partial class attributechangedomain2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PreDefinedValues",
                table: "AttributeTypes",
                newName: "UsesDefinedValues");

            migrationBuilder.AddColumn<string>(
                name: "DefaultCustomValue",
                table: "AttributeTypes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SystemicType",
                table: "AttributeTypes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UsesDefinedUnits",
                table: "AttributeTypes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultCustomValue",
                table: "AttributeTypes");

            migrationBuilder.DropColumn(
                name: "SystemicType",
                table: "AttributeTypes");

            migrationBuilder.DropColumn(
                name: "UsesDefinedUnits",
                table: "AttributeTypes");

            migrationBuilder.RenameColumn(
                name: "UsesDefinedValues",
                table: "AttributeTypes",
                newName: "PreDefinedValues");
        }
    }
}
