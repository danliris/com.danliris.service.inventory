using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Receipt
{
    public class ReceiptMonitoringViewModel
    {
        public int index { get; set; }
        public string ReceiptNoteNo { get; set; }
        public string UENNo { get; set; }
        public string FabricRemark { get; set; }
        public UnitViewModel UnitFrom { get; set; }
        public DateTimeOffset ReceiptDate { get; set; }

        public string POSerialNumber { get; set; }
        public ProductViewModel Product { get; set; }
        public string ProductRemark { get; set; }
        public double Quantity { get; set; }
        public UomViewModel Uom { get; set; }
        public string Composition { get; set; }

        public List<string> CustomsNo { get; set; }
        public List<string> CustomsType { get; set; }
        public List<DateTimeOffset> CustomsDate { get; set; }
        public double Price { get; set; }


    }
}
