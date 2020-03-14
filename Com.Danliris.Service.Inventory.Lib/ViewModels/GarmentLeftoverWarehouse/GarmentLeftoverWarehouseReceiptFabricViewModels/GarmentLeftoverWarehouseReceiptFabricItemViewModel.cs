using Com.Danliris.Service.Inventory.Lib.Helpers;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricViewModels
{
    public class GarmentLeftoverWarehouseReceiptFabricItemViewModel : BasicViewModel
    {
        public int GarmentLeftoverWarehouseReceiptFabricId { get; set; }

        public long UENItemId { get; set; }

        public ProductViewModel Product { get; set; }
        public string ProductRemark { get; set; }

        public decimal Quantity { get; set; }

        public long UomId { get; set; }
        public string UomUnit { get; set; }
    }
}
