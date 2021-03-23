using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.App.EF.Migrations
{
    public partial class OrderExecutionTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderAttributes_TypeValues_TypeValueId",
                table: "OrderAttributes");

            migrationBuilder.RenameColumn(
                name: "TypeValueId",
                table: "OrderAttributes",
                newName: "AttributeTypeValueId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderAttributes_TypeValueId",
                table: "OrderAttributes",
                newName: "IX_OrderAttributes_AttributeTypeValueId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExecutionDateTime",
                table: "Orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAttributes_TypeValues_AttributeTypeValueId",
                table: "OrderAttributes",
                column: "AttributeTypeValueId",
                principalTable: "TypeValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderAttributes_TypeValues_AttributeTypeValueId",
                table: "OrderAttributes");

            migrationBuilder.DropColumn(
                name: "ExecutionDateTime",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "AttributeTypeValueId",
                table: "OrderAttributes",
                newName: "TypeValueId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderAttributes_AttributeTypeValueId",
                table: "OrderAttributes",
                newName: "IX_OrderAttributes_TypeValueId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAttributes_TypeValues_TypeValueId",
                table: "OrderAttributes",
                column: "TypeValueId",
                principalTable: "TypeValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
