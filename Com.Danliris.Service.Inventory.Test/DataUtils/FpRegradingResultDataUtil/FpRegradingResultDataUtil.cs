using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.Test.Interfaces;
using Com.Danliris.Service.Inventory.Lib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Test.DataUtils.IntegrationDataUtil;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.FpRegradingResultDataUtil
{
    public class FpRegradingResultDataUtil : BasicDataUtil<InventoryDbContext, FpRegradingResultDocsService, FpRegradingResultDocs>, IEmptyData<FpRegradingResultDocsViewModel>
    {
        private readonly HttpClientTestService client;
        private readonly FpRegradingResultDetailsDataUtil fpRegradingResultDetailsDataUtil;
        public FpRegradingResultDataUtil(InventoryDbContext dbContext, FpRegradingResultDocsService service, HttpClientTestService client, FpRegradingResultDetailsDataUtil fpRegradingResultDetailsDataUtil) : base(dbContext, service)
        {
            this.client = client;
            this.fpRegradingResultDetailsDataUtil = fpRegradingResultDetailsDataUtil;
        }
        public FpRegradingResultDocsViewModel GetEmptyData()
        {
            FpRegradingResultDocsViewModel Data = new FpRegradingResultDocsViewModel();

            Data.Bon = new FpRegradingResultDocsViewModel.noBon();
            Data.Machine = new FpRegradingResultDocsViewModel.machine();
            Data.Product = new FpRegradingResultDocsViewModel.product();
            Data.Supplier = new FpRegradingResultDocsViewModel.supplier();
            Data.Details = new List<FpRegradingResultDetailsDocsViewModel> { new FpRegradingResultDetailsDocsViewModel() };
            Data.Date = null;
            Data.Shift = string.Empty;
            Data.TotalLength = 0;
            Data.OriginalGrade = string.Empty;
            Data.Remark = string.Empty;

            return Data;
        }

        public override FpRegradingResultDocs GetNewData()
        {
            UnitReceiptNoteViewModel unit = UnitReceiptNoteDataUtil.GetUnitReceiptNote(client);
            //ProductViewModel product = ProductDataUtil.GetProduct(client);
            MachineViewModel machine = MachineDataUtil.GetMachine(client);
            //SupplierViewModel supplier = SupplierDataUtil.GetSupplier(client);

            FpRegradingResultDocs TestData = new FpRegradingResultDocs
            {
                Date = DateTimeOffset.UtcNow,
                NoBon = unit.no,
                NoBonId = unit._id,

                MachineId = machine._id,
                MachineCode = machine.code,
                MachineName = machine.name,

                ProductId = unit.items[0].product._id,
                ProductCode = unit.items[0].product.code,
                ProductName = unit.items[0].product.name,

                SupplierId = unit.supplier._id,
                SupplierCode = unit.supplier.code,
                SupplierName = unit.supplier.name,
                Operator = "operator test",
                Shift = "shift 1 test",
                TotalLength = 100,
                OriginalGrade = "test grade",
                Remark = "test remark",
                UnitName = "PRINTING",

                Details = new List<FpRegradingResultDocsDetails> { fpRegradingResultDetailsDataUtil.GetNewData() }

            };

            return TestData;
        }

        public override async Task<FpRegradingResultDocs> GetTestData()
        {
            FpRegradingResultDocs Data = GetNewData();
            this.Service.Token = HttpClientTestService.Token;
            await this.Service.CreateModel(Data);
            return Data;
        }
    }
}
