using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportGreigeWeavingPerGrade;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.InventoryDataUtils.InventoryWeavingDocumentUploadDataUtil
{
    class InventoryWeavingDocumentUploadDataUtil
    {
        private readonly InventoryWeavingDocumentUploadService Service;
        private readonly ReportGreigeWeavingPerGradeService serviceUtil;

        public InventoryWeavingDocumentUploadDataUtil(InventoryWeavingDocumentUploadService service)
            {
                Service = service;
                
        }

        public InventoryWeavingDocumentUploadDataUtil(ReportGreigeWeavingPerGradeService serviceUtil)
        {
            serviceUtil = serviceUtil;
        }
        

        public InventoryWeavingDocument GetNewData()
            {

                InventoryWeavingDocument TestData = new InventoryWeavingDocument
                {
                    BonNo = "No Nota",
                    BonType = "Produksi",
                    Date = DateTimeOffset.Now,
                    StorageId = 1,
                    StorageCode = "Code",
                    StorageName = "Name",
                    Remark = "Remark",
                    Type = "A",
                    Items = new List<InventoryWeavingDocumentItem> { new InventoryWeavingDocumentItem()
                {
                    Id = 1,
                    Active = true,
                    _CreatedBy = "Created",
                    _CreatedUtc = DateTime.Now,
                    _CreatedAgent = "Created",
                    _LastModifiedBy = "Last Modified",
                    _LastModifiedUtc = DateTime.Now,
                    _LastModifiedAgent = "Last Modified",
                    _IsDeleted = true,
                    ProductOrderName = "Product Order",
                    ReferenceNo = "No Ref",
                    Construction = "Construction",
                    Grade = "A",
                    Piece = "Piece",
                    MaterialName = "Material",
                    WovenType = "Woven",
                    Width = "Width",
                    Yarn1 = "Yarn1",
                    Yarn2 = "Yarn2",
                    YarnType1 = "Yarn Type1",
                    YarnType2 = "Yarn Type2",
                    YarnOrigin1 = "Yarn Origin1",
                    YarnOrigin2 = "Yarn Origin2",

                    UomId = 1,
                    UomUnit = "Unit",
                    Quantity = 1,
                    QuantityPiece = 1,
                    ProductRemark = "Remark",
                    InventoryWeavingDocumentId = 1
                }}
                };

                return TestData;
            }

        public async Task<InventoryWeavingDocument> GetTestData()
        {
            InventoryWeavingDocument Data = GetNewData();
            await Service.UploadData(Data, "Test");
            return Data;
        }

        public ReportGreigeWeavingPerGradeViewModel GetData()
        {
            ReportGreigeWeavingPerGradeViewModel Test = new ReportGreigeWeavingPerGradeViewModel
            {
                MaterialName = "Name",
                WovenType = "Type",
                Yarn1 = "A",
                Yarn2 = "B",
                YarnType1 = "A",
                YarnType2 = "B",
                YarnOrigin1 = "A",
                YarnOrigin2 = "B",
                Width = "Width",
                Construction = "construction",

                InQuantity = 1,
                InQuantityPiece = 1,
                OutQuantity = 1,
                OutQuantityPiece = 1,
                BeginQuantity = 1,
                BeginQuantityPiece = 1,
                EndingQuantity = 1,
                EndingQuantityPiece = 1,
                Nomor = 1,
                Grade = "A"
            };
            return Test;
    }

            

        public async Task<ReportGreigeWeavingPerGradeViewModel> GetTest()
        {
            ReportGreigeWeavingPerGradeViewModel data = GetData();
            await serviceUtil.GetQuery( DateTime.UtcNow, 7);
            return data;
        } 
        
    }
}
