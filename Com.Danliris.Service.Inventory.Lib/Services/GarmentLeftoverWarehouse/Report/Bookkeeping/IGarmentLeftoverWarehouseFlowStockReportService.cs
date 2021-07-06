using Com.Danliris.Service.Inventory.Lib.Interfaces;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Bookkeeping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Bookkeeping
{
    public interface IGarmentLeftoverWarehouseFlowStockReportService 
    
    {
        Tuple<List<GarmentLeftoverWarehouseFlowStockMonitoringViewModel>, int> GetMonitoringFlowStock(string category, DateTime? dateFrom, DateTime? dateTo, int unit, int page, int size, string order, int offset);
        MemoryStream GenerateExcelFlowStock(string category, DateTime? dateFrom, DateTime? dateTo, int unit, int offset);
    }
}
