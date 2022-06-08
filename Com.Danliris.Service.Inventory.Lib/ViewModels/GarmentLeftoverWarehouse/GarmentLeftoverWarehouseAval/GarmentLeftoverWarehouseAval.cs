using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseAval
{
    public class GarmentLeftoverWarehouseAval
    {
        public string BonNo { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public string Keterangan { get; set; }
        public string Product { get; set; }
        public double Quantity { get; set; }
        public string UomUnit { get; set; }
    }
}
