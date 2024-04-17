using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Stock;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Stock
{
    public interface IGarmentLeftoverWarehouseStockReportService 
    {
        MemoryStream GenerateExcelFabric(DateTime? dateFrom, DateTime? dateTo, int unitId, int offset);
        Tuple<List<GarmentLeftoverWarehouseStockMonitoringViewModel>, int> GetMonitoringFabric(DateTime? dateFrom, DateTime? dateTo, int unitId, int page, int size, string order, int offset);
        MemoryStream GenerateExcelAcc(DateTime? dateFrom, DateTime? dateTo, int unitId, int offset);
        Tuple<List<GarmentLeftoverWarehouseStockMonitoringViewModel>, int> GetMonitoringAcc(DateTime? dateFrom, DateTime? dateTo, int unitId, int page, int size, string order, int offset);
        MemoryStream GenerateExcelFinishedGood(DateTime? dateFrom, DateTime? dateTo, int unitId, int offset, string ro, string comodityCode);
        Tuple<List<GarmentLeftoverWarehouseStockMonitoringViewModel>, int> GetMonitoringFinishedGood(DateTime? dateFrom, DateTime? dateTo, int unitId, int page, int size, string order, int offset,string ro,string comodityCode);
        Tuple<List<GarmentLeftoverWarehouseStockMonitoringViewModel>, int> GetMonitoringAval(DateTime? dateFrom, DateTime? dateTo, int unitId, int page, int size, string order, int offset, string typeAval);
        MemoryStream GenerateExcelAval(DateTime? dateFrom, DateTime? dateTo, int unitId, int offset, string typeAval);
        List<GarmentLeftoverWarehouseStockMonitoringViewModel> GetReportQueryForMutationBB(DateTime? dateFrom, DateTime? dateTo, int offset);
    }
}
