using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel
{
    public class InventoryWeavingOutReportViewModel
    {
        public int Number { get; set; }
        public DateTimeOffset Date { get; set; }
        public string BonNo { get; set; }
        public string Construction { get; set; }
        public string Grade { get; set; }
        public string Piece { get; set; }
        public double Quantity { get; set; }
        public double QuantityPiece { get; set; }
        public double QuantityTot { get; set; }
        public double QuantityPieceTot { get; set; }
        public string Remark { get; set; }
        public int Id { get; set; }

        public string Barcode { get; set; }
        public DateTime ProductionOrderDate { get; set; }
    }
}
