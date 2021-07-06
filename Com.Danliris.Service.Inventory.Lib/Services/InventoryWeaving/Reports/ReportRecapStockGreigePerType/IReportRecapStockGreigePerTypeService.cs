using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportRecapStockGreigePerType
{
    public interface IReportRecapStockGreigePerTypeService
    {
        Task<Tuple<List<InventoryWeavingInOutViewModel>, int>> GetRecapStocktGreige( DateTime? dateTo, int offset, int page, int size, string Order);
        Task<MemoryStream> GenerateExcel(DateTime? dateTo, int offset);
    }
}
