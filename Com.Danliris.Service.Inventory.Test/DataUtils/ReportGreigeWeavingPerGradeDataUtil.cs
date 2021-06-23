using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportGreigeWeavingPerGrade;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report;
using Com.Danliris.Service.Inventory.Test.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils
{
    public class ReportGreigeWeavingPerGradeDataUtil
    {
        private readonly ReportGreigeWeavingPerGradeService service;

        public ReportGreigeWeavingPerGradeDataUtil(ReportGreigeWeavingPerGradeService service)
        {
            service = service;
        }

        public ReportGreigeWeavingPerGradeViewModel GetNewData(int offset)
        {
            ReportGreigeWeavingPerGradeViewModel TestData = new ReportGreigeWeavingPerGradeViewModel
            {
                
                Nomor = 1,
                Grade = "Grade",
                Construction = "Construction",
                InQuantityPiece = 1,
                InQuantity = 1,
                OutQuantityPiece = 1,
                OutQuantity = 1,
                BeginQuantityPiece = 1,
                BeginQuantity = 1,
                EndingQuantityPiece = 1,
                EndingQuantity = 1,

                MaterialName = "Test1",
                WovenType = "Test1",
                Yarn1 = "Test",
                Yarn2 = "test",
                YarnType1 = "Test",
                YarnType2 = "test",
                YarnOrigin1 = "Test",
                YarnOrigin2 = "Test2",
                Width = "Test"
            };

            return TestData;
        }

        public async Task<ReportGreigeWeavingPerGradeViewModel> GetTestData(int offset)
        {
            ReportGreigeWeavingPerGradeViewModel Data = GetNewData(offset);
            await this.service.GetStockGrade(Data);
            return Data;
        }
    }
}
