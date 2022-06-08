using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class addColumnInventoryWeaving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "InventoryWeavingMovements",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProductionOrderDate",
                table: "InventoryWeavingMovements",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "InventoryWeavingDocumentItems",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProductionOrderDate",
                table: "InventoryWeavingDocumentItems",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "InventoryWeavingMovements");

            migrationBuilder.DropColumn(
                name: "ProductionOrderDate",
                table: "InventoryWeavingMovements");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "InventoryWeavingDocumentItems");

            migrationBuilder.DropColumn(
                name: "ProductionOrderDate",
                table: "InventoryWeavingDocumentItems");
        }
    }
}
