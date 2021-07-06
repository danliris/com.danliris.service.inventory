using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportGreigeWeavingPerGrade
{
    public interface IReportGreigeWeavingPerGradeService
    {
        Tuple<List<ReportGreigeWeavingPerGradeViewModel>, int> GetStockGrade(DateTime? dateTo, int offset, int page, int size, string Order);
        MemoryStream GenerateExcel(DateTime? dateTo, int offset);
    }
}
