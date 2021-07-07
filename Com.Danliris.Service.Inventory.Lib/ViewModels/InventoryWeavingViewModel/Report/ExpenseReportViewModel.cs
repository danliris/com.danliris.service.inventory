using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report
{
    public class ExpenseReportViewModel
    {
        public DateTimeOffset Date { get; set; }
        public string BonNo { get; set; }
        public string Construction { get; set; }
        public string Grade { get; set; }

        public string Type { get; set; }
        public string MaterialName { get; set; }
        public string WovenType { get; set; }
        public string Yarn1 { get; set; }
        public string Yarn2 { get; set; }
        public string YarnType1 { get; set; }
        public string YarnType2 { get; set; }
        public string YarnOrigin1 { get; set; }
        public string YarnOrigin2 { get; set; }
        public string YarnOrigin { get; set; }
        public string Width { get; set; }
        public string UomUnit { get; set; }

        public double Qty { get; set; }
        public double QtyPiece { get; set; }
    }
}
