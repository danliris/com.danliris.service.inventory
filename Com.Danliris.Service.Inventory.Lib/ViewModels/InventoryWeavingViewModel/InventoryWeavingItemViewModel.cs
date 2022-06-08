using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel
{
    public class InventoryWeavingItemViewModel : BasicViewModel
    {
        public DateTimeOffset Date { get; set; }
        public string BonNo { get; set; }
        public string ReferenceNo { get; set; }
        public string Construction { get; set; }
        public string Grade { get; set; }
        public string Piece { get; set; }
        public double Quantity { get; set; }
        public double QuantityPiece { get; set; }
        public string Barcode { get; set; }
        public DateTime ProductionOrderDate { get; set; }
    }
}
