using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel
{
    public class InventoryWeavingDocumentCsvOutViewModel
    {

        //"BonNo","Tanggal","Benang","Anyaman","Lusi","Pakan","Lebar","JL","JP","AL","AP","Grade","Piece","Qty","QtyPiece"
        public string BonNo { get; set; }
        public string Tanggal { get; set; }
        public string Benang { get; set; }
        public string Anyaman { get; set; }
        public string Lusi { get; set; }
        public string Pakan { get; set; }
        public string Lebar { get; set; }
        public string JL { get; set; }
        public string JP { get; set; }
        public string AL { get; set; }
        public string AP { get; set; }
        
        public string Grade { get; set; }
        public string Piece { get; set; }
        public double Qty { get; set; }
        public double QtyPiece { get; set; }

        public string Barcode { get; set; }
        public DateTime ProductionOrderDate { get; set; }
        //public string Construction { get; set; }



    }
}
