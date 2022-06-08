using Com.Danliris.Service.Inventory.Lib.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Stock
{
    public class GarmentLeftoverWarehouseStockMonitoringViewModel
    {
        public int index { get; set; }
        public string UnitCode { get; set; }
        public string PONo { get; set; }
        public string RO { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductRemark { get; set; }
        public string FabricRemark { get; set; }
        public string ComodityCode { get; set; }
        public string ComodityUnitCode { get; set; }
        public string Comodity { get; set; }
        public double BeginingbalanceQty { get; set; }
        public double QuantityReceipt { get; set; }
        public double QuantityExpend { get; set; }
        public double EndbalanceQty { get; set; }
        public string UomUnit { get; set; }
        public string ReferenceType { get; set; }
    }

    public class ExpendGoodViewModel
    {
        public string RONo { get; set; }
        public GarmentComodity Comodity { get; set; }
    }

    public class GarmentComodity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
