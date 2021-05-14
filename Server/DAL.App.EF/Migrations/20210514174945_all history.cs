using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.App.EF.Migrations
{
    public partial class allhistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MasterId",
                table: "TypeValues",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MasterId",
                table: "TypeUnits",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MasterId",
                table: "AttributeTypes",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MasterId",
                table: "TypeValues");

            migrationBuilder.DropColumn(
                name: "MasterId",
                table: "TypeUnits");

            migrationBuilder.DropColumn(
                name: "MasterId",
                table: "AttributeTypes");
        }
    }
}
