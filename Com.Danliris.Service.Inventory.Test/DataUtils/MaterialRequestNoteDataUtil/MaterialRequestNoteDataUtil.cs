using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialsRequestNoteServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialsRequestNoteViewModel;
using Com.Danliris.Service.Inventory.Test.DataUtils.IntegrationDataUtil;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.Test.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.MaterialRequestNoteDataUtil
{
    public class MaterialRequestNoteDataUtil : BasicDataUtil<InventoryDbContext, MaterialsRequestNoteService, MaterialsRequestNote>, IEmptyData<MaterialsRequestNoteViewModel>
    {
        private readonly HttpClientTestService client;
        private readonly MaterialRequestNoteItemDataUtil materialRequestNoteItemDataUtil;

        public MaterialRequestNoteDataUtil(InventoryDbContext dbContext, MaterialsRequestNoteService service, HttpClientTestService client, MaterialRequestNoteItemDataUtil materialRequestNoteItemDataUtil) : base(dbContext, service)
        {
            this.client = client;
            this.materialRequestNoteItemDataUtil = materialRequestNoteItemDataUtil;
        }

        public MaterialsRequestNoteViewModel GetEmptyData()
        {
            MaterialsRequestNoteViewModel Data = new MaterialsRequestNoteViewModel();

            Data.RequestType = string.Empty;
            Data.Unit = new UnitViewModel();
            Data.Remark = string.Empty;
            Data.MaterialsRequestNote_Items = new List<MaterialsRequestNote_ItemViewModel> { new MaterialsRequestNote_ItemViewModel() };

            return Data;
        }

        public override MaterialsRequestNote GetNewData()
        {
            UnitViewModel fp = UnitDataUtil.GetFinishingUnit(client);

            MaterialsRequestNote TestData = new MaterialsRequestNote
            {
                UnitIntegerId = fp.Id,
                UnitCode = fp.Code,
                UnitName = fp.Name,
                Remark = "",
                RequestType = "AWAL",
                IsDistributed = false,
                IsCompleted = false,
                MaterialsRequestNote_Items = new List<MaterialsRequestNote_Item> { materialRequestNoteItemDataUtil.GetNewData() }
            };

            return TestData;
        }


        public override async Task<MaterialsRequestNote> GetTestData()
        {
            MaterialsRequestNote Data = GetNewData();
            this.Service.Token = HttpClientTestService.Token;
            await this.Service.CreateModel(Data);
            return Data;
        }

        public async Task<MaterialsRequestNote> GetTestDataCustom()
        {
            MaterialsRequestNote Data = GetNewData();
            Data.MaterialsRequestNote_Items = new List<MaterialsRequestNote_Item> { materialRequestNoteItemDataUtil.GetNewData(), materialRequestNoteItemDataUtil.GetNewDataCustom() };
            this.Service.Token = HttpClientTestService.Token;
            await this.Service.CreateModel(Data);
            return Data;
        }
    }
}
