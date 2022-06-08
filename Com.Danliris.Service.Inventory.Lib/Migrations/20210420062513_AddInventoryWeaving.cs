using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Inventory.Lib.Migrations
{
    public partial class AddInventoryWeaving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryWeavingDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    _CreatedUtc = table.Column<DateTime>(nullable: false),
                    _CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    _LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _IsDeleted = table.Column<bool>(nullable: false),
                    _DeletedUtc = table.Column<DateTime>(nullable: false),
                    _DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    Date = table.Column<DateTimeOffset>(nullable: false),
                    BonNo = table.Column<string>(maxLength: 255, nullable: true),
                    BonType = table.Column<string>(maxLength: 255, nullable: true),
                    StorageId = table.Column<int>(maxLength: 255, nullable: false),
                    StorageCode = table.Column<string>(maxLength: 255, nullable: true),
                    StorageName = table.Column<string>(maxLength: 255, nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryWeavingDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryWeavingMovement",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    _CreatedUtc = table.Column<DateTime>(nullable: false),
                    _CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    _LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _IsDeleted = table.Column<bool>(nullable: false),
                    _DeletedUtc = table.Column<DateTime>(nullable: false),
                    _DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    ProductOrderName = table.Column<string>(maxLength: 255, nullable: true),
                    BonNo = table.Column<string>(nullable: true),
                    ReferenceNo = table.Column<string>(maxLength: 255, nullable: true),
                    Construction = table.Column<string>(maxLength: 255, nullable: true),
                    Grade = table.Column<string>(maxLength: 255, nullable: true),
                    Piece = table.Column<string>(maxLength: 1000, nullable: true),
                    MaterialName = table.Column<string>(nullable: true),
                    WovenType = table.Column<string>(nullable: true),
                    Yarn1 = table.Column<string>(nullable: true),
                    Yarn2 = table.Column<string>(nullable: true),
                    YarnType1 = table.Column<string>(nullable: true),
                    YarnType2 = table.Column<string>(nullable: true),
                    YarnOrigin1 = table.Column<string>(nullable: true),
                    YarnOrigin2 = table.Column<string>(nullable: true),
                    Width = table.Column<string>(nullable: true),
                    UomUnit = table.Column<string>(maxLength: 255, nullable: true),
                    UomId = table.Column<int>(maxLength: 255, nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    QuantityPiece = table.Column<double>(nullable: false),
                    ProductRemark = table.Column<string>(nullable: true),
                    InventoryWeavingDocumentId = table.Column<int>(nullable: false),
                    InventoryWeavingDocumentItemId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryWeavingMovement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryWeavingDocumentItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    _CreatedUtc = table.Column<DateTime>(nullable: false),
                    _CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    _LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _IsDeleted = table.Column<bool>(nullable: false),
                    _DeletedUtc = table.Column<DateTime>(nullable: false),
                    _DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    ProductOrderName = table.Column<string>(maxLength: 255, nullable: true),
                    ReferenceNo = table.Column<string>(maxLength: 255, nullable: true),
                    Construction = table.Column<string>(maxLength: 255, nullable: true),
                    Grade = table.Column<string>(maxLength: 255, nullable: true),
                    Piece = table.Column<string>(nullable: true),
                    MaterialName = table.Column<string>(nullable: true),
                    WovenType = table.Column<string>(nullable: true),
                    Yarn1 = table.Column<string>(nullable: true),
                    Yarn2 = table.Column<string>(nullable: true),
                    YarnType1 = table.Column<string>(nullable: true),
                    YarnType2 = table.Column<string>(nullable: true),
                    YarnOrigin1 = table.Column<string>(nullable: true),
                    YarnOrigin2 = table.Column<string>(nullable: true),
                    Width = table.Column<string>(nullable: true),
                    UomUnit = table.Column<string>(maxLength: 255, nullable: true),
                    UomId = table.Column<int>(maxLength: 255, nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    QuantityPiece = table.Column<double>(nullable: false),
                    ProductRemark = table.Column<string>(nullable: true),
                    InventoryWeavingDocumentId = table.Column<int>(nullable: false),
                   
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryWeavingDocumentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryWeavingDocumentItems_InventoryWeavingDocuments_InventoryWeavingDocumentId",
                        column: x => x.InventoryWeavingDocumentId,
                        principalTable: "InventoryWeavingDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryWeavingDocumentItems_InventoryWeavingDocumentId",
                table: "InventoryWeavingDocumentItems",
                column: "InventoryWeavingDocumentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryWeavingDocumentItems");

            migrationBuilder.DropTable(
                name: "InventoryWeavingMovement");

            migrationBuilder.DropTable(
                name: "InventoryWeavingDocuments");
        }
    }
}
