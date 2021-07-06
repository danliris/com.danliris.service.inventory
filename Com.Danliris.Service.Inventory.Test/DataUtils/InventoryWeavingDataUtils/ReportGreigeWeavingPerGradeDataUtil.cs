using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportGreigeWeavingPerGrade;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.InventoryWeavingDataUtils.ReportGreigeWeavingPerGradeDataUtil
{
    public class ReportGreigeWeavingPerGradeDataUtil
    {
        private readonly ReportGreigeWeavingPerGradeService Service;

        public ReportGreigeWeavingPerGradeDataUtil(ReportGreigeWeavingPerGradeService service)
        {
            Service = service;
        }

        public InventoryWeavingMovement GetNewData()
        {
            InventoryWeavingMovement TestData = new InventoryWeavingMovement
            {
                
                ProductOrderName = "Name",
                BonNo = "nota",
                ReferenceNo = "Ref",
                _CreatedUtc = DateTime.UtcNow,
                Construction = "construction",
                MaterialName = "Name",
                WovenType = "Type",
                Yarn1 = "Yarn1",
                Yarn2 = "Yarn2",
                Width = "Width",
                YarnOrigin1 = "YarnOrigin1",
                YarnOrigin2 = "YarnOrigin2",
                YarnType1 = "YarnType1",
                YarnType2 = "YarnType2",
                QuantityPiece = 10,
                Quantity = 10,
                Grade = "Grade",
                Type = "IN",
                InventoryWeavingDocumentId = 1,
            };
            return TestData;
        }

        public async Task<InventoryWeavingMovement> GetTestData()
        {
            InventoryWeavingMovement Data = GetNewData();
            await Service.Create(Data);
            return Data;
        }
    }
}
