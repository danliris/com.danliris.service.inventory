using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Bookkeeping
{
    public class GarmentLeftoverWarehouseFlowStockMonitoringViewModel: BasicViewModel
    { 

        public string UnitName { get; set; } 
        public string PONo { get; set; }
        public string RO { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double BeginingbalanceQty { get; set; }
        public double BeginingbalancePrice { get; set; }
        public double QuantityReceipt { get; set; }
        public double PriceReceipt { get; set; }
        public double QuantityUnitExpend { get; set; }
        public double PriceUnitExpend { get; set; }
        public double QuantitySampleExpend { get; set; }
        public double PriceSampleExpend { get; set; }
        public double QuantityLocalExpend { get; set; }
        public double PriceLocalExpend { get; set; }
        public double QuantityOtherExpend { get; set; }
        public double PriceOtherExpend { get; set; }
        public double EndbalanceQty { get; set; }
        public double EndbalancePrice { get; set; }
        public string UomUnit { get; set; } 
        public double BasicPrice { get; set; }
    }
}
