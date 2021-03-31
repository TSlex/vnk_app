using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.App.EF.Migrations
{
    public partial class attributechangedomain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderAttributes_TypeValues_AttributeTypeValueId",
                table: "OrderAttributes");

            migrationBuilder.DropIndex(
                name: "IX_OrderAttributes_AttributeTypeValueId",
                table: "OrderAttributes");

            migrationBuilder.DropColumn(
                name: "AttributeTypeValueId",
                table: "OrderAttributes");

            migrationBuilder.DropColumn(
                name: "Displayed",
                table: "Attributes");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Templates",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "CustomValue",
                table: "OrderAttributes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Featured",
                table: "OrderAttributes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UnitId",
                table: "OrderAttributes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ValueId",
                table: "OrderAttributes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DefaultUnitId",
                table: "AttributeTypes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "DefaultValueId",
                table: "AttributeTypes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "PreDefinedValues",
                table: "AttributeTypes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AttributeTypeUnit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    AttributeTypeId = table.Column<long>(type: "bigint", nullable: false),
                    DeletedBy = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ChangedBy = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeTypeUnit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeTypeUnit_AttributeTypes_AttributeTypeId",
                        column: x => x.AttributeTypeId,
                        principalTable: "AttributeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderAttributes_UnitId",
                table: "OrderAttributes",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAttributes_ValueId",
                table: "OrderAttributes",
                column: "ValueId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeTypeUnit_AttributeTypeId",
                table: "AttributeTypeUnit",
                column: "AttributeTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAttributes_AttributeTypeUnit_UnitId",
                table: "OrderAttributes",
                column: "UnitId",
                principalTable: "AttributeTypeUnit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAttributes_TypeValues_ValueId",
                table: "OrderAttributes",
                column: "ValueId",
                principalTable: "TypeValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderAttributes_AttributeTypeUnit_UnitId",
                table: "OrderAttributes");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderAttributes_TypeValues_ValueId",
                table: "OrderAttributes");

            migrationBuilder.DropTable(
                name: "AttributeTypeUnit");

            migrationBuilder.DropIndex(
                name: "IX_OrderAttributes_UnitId",
                table: "OrderAttributes");

            migrationBuilder.DropIndex(
                name: "IX_OrderAttributes_ValueId",
                table: "OrderAttributes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "CustomValue",
                table: "OrderAttributes");

            migrationBuilder.DropColumn(
                name: "Featured",
                table: "OrderAttributes");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "OrderAttributes");

            migrationBuilder.DropColumn(
                name: "ValueId",
                table: "OrderAttributes");

            migrationBuilder.DropColumn(
                name: "DefaultUnitId",
                table: "AttributeTypes");

            migrationBuilder.DropColumn(
                name: "DefaultValueId",
                table: "AttributeTypes");

            migrationBuilder.DropColumn(
                name: "PreDefinedValues",
                table: "AttributeTypes");

            migrationBuilder.AddColumn<long>(
                name: "AttributeTypeValueId",
                table: "OrderAttributes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "Displayed",
                table: "Attributes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_OrderAttributes_AttributeTypeValueId",
                table: "OrderAttributes",
                column: "AttributeTypeValueId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAttributes_TypeValues_AttributeTypeValueId",
                table: "OrderAttributes",
                column: "AttributeTypeValueId",
                principalTable: "TypeValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
