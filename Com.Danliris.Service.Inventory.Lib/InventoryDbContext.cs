using Com.Moonlay.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Com.Danliris.Service.Inventory.Lib.Models;
using Com.Danliris.Service.Inventory.Lib.Configs;
using Com.Danliris.Service.Inventory.Lib.Configs.MaterialDistributionNoteConfig;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel;
using Com.Danliris.Service.Inventory.Lib.Models.StockTransferNoteModel;
using Com.Danliris.Service.Inventory.Lib.Configs.StockTransferNoteConfig;
using Com.Danliris.Service.Inventory.Lib.Configs.MaterialsRequestNoteConfig;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using Com.Danliris.Service.Inventory.Lib.Configs.FPReturnInvToPurchasingConfig;
using Com.Danliris.Service.Inventory.Lib.Models.FPReturnInvToPurchasingModel;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.Configs.InventoriesConfig;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricModels;
using Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricConfigs;

namespace Com.Danliris.Service.Inventory.Lib
{
    public class InventoryDbContext : BaseDbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        public DbSet<MaterialsRequestNote> MaterialsRequestNotes { get; set; }
        public DbSet<MaterialsRequestNote_Item> MaterialsRequestNote_Items { get; set; }
        public DbSet<FpRegradingResultDocsDetails> fpRegradingResultDocsDetails { get; set; }
        public DbSet<FpRegradingResultDocs> fpRegradingResultDocs { get; set; }
        public DbSet<MaterialDistributionNote> MaterialDistributionNotes { get; set; }
        public DbSet<MaterialDistributionNoteItem> MaterialDistributionNoteItems { get; set; }
        public DbSet<MaterialDistributionNoteDetail> MaterialDistributionNoteDetails { get; set; }
        public DbSet<StockTransferNote> StockTransferNotes { get; set; }
        public DbSet<StockTransferNote_Item> StockTransferNoteItems { get; set; }
        public DbSet<FPReturnInvToPurchasing> FPReturnInvToPurchasings { get; set; }
        public DbSet<FPReturnInvToPurchasingDetail> FPReturnInvToPurchasingDetails { get; set; }

        public DbSet<InventoryDocument> InventoryDocuments { get; set; }
        public DbSet<InventoryDocumentItem> InventoryDocumentItems { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
        public DbSet<InventorySummary> InventorySummaries { get; set; }

        public DbSet<GarmentLeftoverWarehouseReceiptFabric> GarmentLeftoverWarehouseReceiptFabrics { get; set; }
        public DbSet<GarmentLeftoverWarehouseReceiptFabricItem> GarmentLeftoverWarehouseReceiptFabricItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new MaterialsRequestNoteConfig());
            modelBuilder.ApplyConfiguration(new MaterialsRequestNote_ItemConfig());
            modelBuilder.ApplyConfiguration(new FpRegradingResultDocsDetailsConfig());
            modelBuilder.ApplyConfiguration(new FpRegradingResultDocsConfig());
            modelBuilder.ApplyConfiguration(new MaterialDistributionNoteConfig());
            modelBuilder.ApplyConfiguration(new MaterialDistributionNoteItemConfig());
            modelBuilder.ApplyConfiguration(new MaterialDistributionNoteDetailConfig());
            modelBuilder.ApplyConfiguration(new StockTransferNoteConfig());
            modelBuilder.ApplyConfiguration(new StockTransferNoteItemConfig());
            modelBuilder.ApplyConfiguration(new FPReturnInvToPurchasingConfig());
            modelBuilder.ApplyConfiguration(new FPReturnInvToPurchasingDetailConfig());
            modelBuilder.ApplyConfiguration(new InventoryDocumentConfig());
            modelBuilder.ApplyConfiguration(new InventoryDocumentItemConfig());
            modelBuilder.ApplyConfiguration(new InventoryMovementConfig());
            modelBuilder.ApplyConfiguration(new InventorySummaryConfig());

            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseReceiptFabricConfig());
            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseReceiptFabricItemConfig());
        }
    }
}