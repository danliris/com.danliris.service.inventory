using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Bookkeeping
{
    public class GarmentLeftoverWarehouseRecapStockReportViewModel
    {
        public string Unit { get; set; }
        public string ExpenditureTo { get; set; }
        public string Description { get; set; }
        public double FinishedGoodQty { get; set; }
        public string FinishedGoodUom { get; set; }
        public double FinishedGoodPrice { get; set; }

        public double AccPrice { get; set; }

        public double FabricQty { get; set; }
        public string FabricUom { get; set; }
        public double FabricPrice { get; set; }


        public double PriceTotal { get; set; }

    }
}
