using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report
{
    public class ReportGreigeWeavingPerGradeViewModel
    {
        public string MaterialName { get; set; }
        public string WovenType { get; set; }
        public string Yarn1 { get; set; }
        public string Yarn2 { get; set; }
        public string YarnType1 { get; set; }
        public string YarnType2 { get; set; }
        public string YarnOrigin1 { get; set; }
        public string YarnOrigin2 { get; set; }
        public string Width { get; set; }
        public string Construction { get; set; }

        public double InQuantity { get; set; }
        public double InQuantityPiece { get; set; }
        public double OutQuantity { get; set; }
        public double OutQuantityPiece { get; set; }
        public double BeginQuantity { get; set; }
        public double BeginQuantityPiece { get; set; }
        public double EndingQuantity { get; set; }
        public double EndingQuantityPiece { get; set; }
        public int Nomor { get; set; }
        public string Grade { get; set; }
    }

    public class ReportGreigeWeavingPerGradeAllViewModel : BasicViewModel
    {
        public string MaterialName { get; set; }
        public string WovenType { get; set; }
        public string Yarn1 { get; set; }
        public string Yarn2 { get; set; }
        public string YarnType1 { get; set; }
        public string YarnType2 { get; set; }
        public string YarnOrigin1 { get; set; }
        public string YarnOrigin2 { get; set; }
        public string Width { get; set; }
        public string Construction { get; set; }
        public DateTimeOffset Date { get; set; }
        public double Quantity { get; set; }
        public double QuantityPiece { get; set; }
        public string Grade { get; set; }
        public string Type { get; set; }
        public int IdMove { get; set; }
        public int IdDoc { get; set; }


    }
}
