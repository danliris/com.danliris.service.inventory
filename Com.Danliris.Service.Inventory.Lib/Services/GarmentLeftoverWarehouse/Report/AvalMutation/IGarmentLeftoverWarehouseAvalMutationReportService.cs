using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseAval;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.AvalMutation
{
    public interface IGarmentLeftoverWarehouseAvalMutationReportService
    {
        //List<GarmentLeftoverWarehouseAval> GetAvalBesarIn(DateTime? dateFrom, DateTime? dateTo, int page, int size);
        Tuple<List<GarmentLeftoverWarehouseAval>, int> GetAvalBesarIn(DateTime? dateFrom, DateTime? dateTo, int page, int size);
        Tuple<List<GarmentLeftoverWarehouseAval>, int> GetAvalBesarOut(DateTime? dateFrom, DateTime? dateTo, int page, int size);
        Tuple<List<GarmentLeftoverWarehouseAval>, int> GetAvalKomponenIn(DateTime? dateFrom, DateTime? dateTo, int page, int size);
        Tuple<List<GarmentLeftoverWarehouseAval>, int> GetAvalKomponenOut(DateTime? dateFrom, DateTime? dateTo, int page, int size);
        MemoryStream GenerateExcelAvalBesar(DateTime dateFrom, DateTime dateTo);
        MemoryStream GenerateExcelAvalBesarOut(DateTime dateFrom, DateTime dateTo);
        MemoryStream GenerateExcelAvalKomponenIn(DateTime dateFrom, DateTime dateTo);
        MemoryStream GenerateExcelAvalKomponenOut(DateTime dateFrom, DateTime dateTo);
    }
}
