using Com.Danliris.Service.Inventory.Lib.Models.StockTransferNoteModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.StockTransferNoteViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.StockTransferNoteDataUtil
{
    public class StockTransferNoteItemDataUtil
    {
        public StockTransferNote_Item GetNewData(InventorySummaryViewModel inventorySummary)
        {
            return new StockTransferNote_Item
            {
                ProductId = inventorySummary.productId,
                ProductCode = inventorySummary.productCode,
                ProductName = inventorySummary.productName,
                StockQuantity = inventorySummary.quantity,
                UomUnit = inventorySummary.uom,
                UomId = inventorySummary.uomId,
                TransferedQuantity = 1
            };
        }
    }
}
