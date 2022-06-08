using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class AddColumnDestinationArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DestinationArea",
                table: "InventoryWeavingMovements",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationArea",
                table: "InventoryWeavingDocumentItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "InventoryWeavingDocumentItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationArea",
                table: "InventoryWeavingMovements");

            migrationBuilder.DropColumn(
                name: "DestinationArea",
                table: "InventoryWeavingDocumentItems");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "InventoryWeavingDocumentItems");
        }
    }
}
