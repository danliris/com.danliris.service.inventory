using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricConfigs
{
    public class GarmentLeftoverWarehouseReceiptFabricItemConfig : IEntityTypeConfiguration<GarmentLeftoverWarehouseReceiptFabricItem>
    {
        public void Configure(EntityTypeBuilder<GarmentLeftoverWarehouseReceiptFabricItem> builder)
        {
            builder.Property(p => p.ProductCode).HasMaxLength(25);
            builder.Property(p => p.ProductName).HasMaxLength(100);
            builder.Property(p => p.ProductRemark).HasMaxLength(3000);

            builder.Property(p => p.UomUnit).HasMaxLength(100);

        }
    }
}
