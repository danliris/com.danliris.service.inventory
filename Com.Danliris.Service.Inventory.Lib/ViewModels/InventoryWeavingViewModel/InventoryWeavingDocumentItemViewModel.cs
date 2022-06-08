using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel
{
    public class InventoryWeavingDocumentItemViewModel : BasicViewModel
    {

        public string productOrderNo { get; set; }
        public string referenceNo { get; set; }
        public string construction { get; set; }
        public string grade { get; set; }
        public string piece { get; set; }

        /*construction */
        public string materialName { get; set; }
        public string wovenType { get; set; }
        public string yarn1 { get; set; }
        public string yarn2 { get; set; }
        public string yarnType1 { get; set; }
        public string yarnType2 { get; set; }
        public string yarnOrigin1 { get; set; }
        public string yarnOrigin2 { get; set; }
        public string width { get; set; }

        public string uomUnit { get; set; }
        public int uomId { get; set; }
        public double quantity { get; set; }
        public double quantityPiece { get; set; }
        public string productRemark { get; set; }

        public string barcode { get; set; }
        public DateTime productionOrderDate { get; set; }
        
        public int InventoryWeavingDocumentId { get; set; }
    }
}
