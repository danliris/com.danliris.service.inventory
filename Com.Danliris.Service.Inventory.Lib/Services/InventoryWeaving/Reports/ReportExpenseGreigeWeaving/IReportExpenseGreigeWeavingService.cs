using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportExpenseGreigeWeaving
{
    public interface IReportExpenseGreigeWeavingService
    {
        Tuple<List<ExpenseReportViewModel>, int> GetReportExpense(string bonType, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);
        MemoryStream GenerateExcelExpense(string bonType, DateTime? dateFrom, DateTime? dateTo, int offSet);
    }
}
