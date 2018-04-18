using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models.StockTransferNoteModel;
using Com.Danliris.Service.Inventory.Lib.Services.StockTransferNoteService;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.StockTransferNoteViewModel;
using Com.Danliris.Service.Inventory.Test.DataUtils.IntegrationDataUtil;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.Test.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.StockTransferNoteDataUtil
{
    public class StockTransferNoteDataUtil : BasicDataUtil<InventoryDbContext, StockTransferNoteService, StockTransferNote>, IEmptyData<StockTransferNoteViewModel>
    {
        private readonly HttpClientService client;
        private readonly StockTransferNoteItemDataUtil stockTransferNoteItemDataUtil;

        public StockTransferNoteDataUtil(InventoryDbContext dbContext, StockTransferNoteService service, HttpClientService client, StockTransferNoteItemDataUtil stockTransferNoteItemDataUtil) : base(dbContext, service)
        {
            this.client = client;
            this.stockTransferNoteItemDataUtil = stockTransferNoteItemDataUtil;
        }

        public StockTransferNoteViewModel GetEmptyData()
        {
            StockTransferNoteViewModel Data = new StockTransferNoteViewModel();

            Data.ReferenceNo = string.Empty;
            Data.ReferenceType = string.Empty;
            Data.SourceStorage = new StorageViewModel();
            Data.TargetStorage = new StorageViewModel();
            Data.StockTransferNoteItems = new List<StockTransferNote_ItemViewModel> { new StockTransferNote_ItemViewModel() };

            return Data;
        }

        public override StockTransferNote GetNewData()
        {
            StorageViewModel storage = StorageDataUtil.GetPrintingGreigeStorage(client);
            InventorySummaryViewModel inventorySummary = InventorySummaryDataUtil.GetInventorySummary(client);

            StockTransferNote TestData = new StockTransferNote
            {
                ReferenceNo = "Reference No Test",
                ReferenceType = "Reference Type Test",
                SourceStorageId = inventorySummary.storageId,
                SourceStorageCode = inventorySummary.storageCode,
                SourceStorageName = inventorySummary.storageName,
                TargetStorageId = storage._id,
                TargetStorageCode = storage.code,
                TargetStorageName = storage.name,
                IsApproved = false,
                StockTransferNoteItems = new List<StockTransferNote_Item> { stockTransferNoteItemDataUtil.GetNewData(inventorySummary) }
            };

            return TestData;
        }


        public override async Task<StockTransferNote> GetTestData()
        {
            StockTransferNote Data = GetNewData();
            this.Service.Token = HttpClientService.Token;
            await this.Service.CreateModel(Data);
            return Data;
        }
    }
}
