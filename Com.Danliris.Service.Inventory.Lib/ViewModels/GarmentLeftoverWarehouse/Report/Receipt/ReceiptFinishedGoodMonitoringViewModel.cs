using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Receipt
{
    public class ReceiptFinishedGoodMonitoringViewModel
    {
        public int index { get; set; }
        public string ReceiptNoteNo { get; set; }
        public DateTimeOffset ReceiptDate { get; set; }

        public string UnitFromCode { get; set; }
        public string ExpenditureGoodNo { get; set; }
        public string RONo { get; set; }
        public string ComodityCode { get; set; }
        public string UnitComodityCode { get; set; }
        public string ComodityName { get; set; }
        public double Quantity { get; set; }
        public string UomUnit { get; set; }
        public double Price { get; set; }
        //public string PoSerialNumber { get; set; }
        public List<string> PoSerialNumbers { get; set; }
        public List<string> CustomsNo { get; set; }
        public List<string> CustomsType { get; set; }
        public List<DateTimeOffset> CustomsDate { get; set; }

    }
}
