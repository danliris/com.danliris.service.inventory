using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class addIntegerForeignColumnInMaterialRequestDistributionNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnitIntegerId",
                table: "MaterialsRequestNotes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderTypeIntegerId",
                table: "MaterialsRequestNote_Items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductIntegerId",
                table: "MaterialsRequestNote_Items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "ProductionOrderLongId",
                table: "MaterialsRequestNote_Items",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "UnitIntegerId",
                table: "MaterialDistributionNotes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductIntegerId",
                table: "MaterialDistributionNoteDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "ProductionOrderLongId",
                table: "MaterialDistributionNoteDetails",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "SupplierIntegerId",
                table: "MaterialDistributionNoteDetails",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitIntegerId",
                table: "MaterialsRequestNotes");

            migrationBuilder.DropColumn(
                name: "OrderTypeIntegerId",
                table: "MaterialsRequestNote_Items");

            migrationBuilder.DropColumn(
                name: "ProductIntegerId",
                table: "MaterialsRequestNote_Items");

            migrationBuilder.DropColumn(
                name: "ProductionOrderLongId",
                table: "MaterialsRequestNote_Items");

            migrationBuilder.DropColumn(
                name: "UnitIntegerId",
                table: "MaterialDistributionNotes");

            migrationBuilder.DropColumn(
                name: "ProductIntegerId",
                table: "MaterialDistributionNoteDetails");

            migrationBuilder.DropColumn(
                name: "ProductionOrderLongId",
                table: "MaterialDistributionNoteDetails");

            migrationBuilder.DropColumn(
                name: "SupplierIntegerId",
                table: "MaterialDistributionNoteDetails");
        }
    }
}
