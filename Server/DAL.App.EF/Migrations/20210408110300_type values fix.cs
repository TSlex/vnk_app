using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.App.EF.Migrations
{
    public partial class typevaluesfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttributeTypeUnit_AttributeTypes_AttributeTypeId",
                table: "AttributeTypeUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderAttributes_AttributeTypeUnit_UnitId",
                table: "OrderAttributes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttributeTypeUnit",
                table: "AttributeTypeUnit");

            migrationBuilder.RenameTable(
                name: "AttributeTypeUnit",
                newName: "TypeUnits");

            migrationBuilder.RenameIndex(
                name: "IX_AttributeTypeUnit_AttributeTypeId",
                table: "TypeUnits",
                newName: "IX_TypeUnits_AttributeTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TypeUnits",
                table: "TypeUnits",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAttributes_TypeUnits_UnitId",
                table: "OrderAttributes",
                column: "UnitId",
                principalTable: "TypeUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TypeUnits_AttributeTypes_AttributeTypeId",
                table: "TypeUnits",
                column: "AttributeTypeId",
                principalTable: "AttributeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderAttributes_TypeUnits_UnitId",
                table: "OrderAttributes");

            migrationBuilder.DropForeignKey(
                name: "FK_TypeUnits_AttributeTypes_AttributeTypeId",
                table: "TypeUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TypeUnits",
                table: "TypeUnits");

            migrationBuilder.RenameTable(
                name: "TypeUnits",
                newName: "AttributeTypeUnit");

            migrationBuilder.RenameIndex(
                name: "IX_TypeUnits_AttributeTypeId",
                table: "AttributeTypeUnit",
                newName: "IX_AttributeTypeUnit_AttributeTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttributeTypeUnit",
                table: "AttributeTypeUnit",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeTypeUnit_AttributeTypes_AttributeTypeId",
                table: "AttributeTypeUnit",
                column: "AttributeTypeId",
                principalTable: "AttributeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAttributes_AttributeTypeUnit_UnitId",
                table: "OrderAttributes",
                column: "UnitId",
                principalTable: "AttributeTypeUnit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
