using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricConfigs
{
    public class GarmentLeftoverWarehouseReceiptFabricConfig : IEntityTypeConfiguration<GarmentLeftoverWarehouseReceiptFabric>
    {
        public void Configure(EntityTypeBuilder<GarmentLeftoverWarehouseReceiptFabric> builder)
        {
            builder.Property(p => p.ReceiptNoteNo).HasMaxLength(25);

            builder.Property(p => p.UnitFromCode).HasMaxLength(25);
            builder.Property(p => p.UnitFromName).HasMaxLength(100);

            builder.Property(p => p.UENNo).HasMaxLength(25);

            builder.Property(p => p.StorageFromCode).HasMaxLength(25);
            builder.Property(p => p.StorageFromName).HasMaxLength(100);

            builder.Property(p => p.Remark).HasMaxLength(3000);

            builder
                .HasMany(h => h.Items)
                .WithOne(w => w.GarmentLeftoverWarehouseReceiptFabric)
                .HasForeignKey(f => f.GarmentLeftoverWarehouseReceiptFabricId)
                .IsRequired();
        }
    }
}
