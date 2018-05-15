using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialsRequestNoteServices;
using Com.Danliris.Service.Inventory.Test.DataUtils.MaterialRequestNoteDataUtil;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static Com.Danliris.Service.Inventory.Lib.Services.MaterialsRequestNoteServices.MaterialsRequestNoteService;
using model = Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;

namespace Com.Danliris.Service.Inventory.Test.Services.MaterialRequestNote
{
    [Collection("ServiceProviderFixture Collection")]
    public class MaterialRequestNoteCustomTest
    {
        private readonly MaterialRequestNoteDataUtil dataUtil;
        private readonly MaterialsRequestNoteService service;
        public MaterialRequestNoteCustomTest(MaterialRequestNoteDataUtil dataUtil, MaterialsRequestNoteService materialsRequestNoteService)
        {
            this.dataUtil = dataUtil;
            this.service = materialsRequestNoteService;
        }

        [Fact]
        public async void Should_Success_Update_Data()
        {
            model.MaterialsRequestNote Data = await this.dataUtil.GetTestDataCustom();

            foreach (MaterialsRequestNote_Item item in Data.MaterialsRequestNote_Items)
            {
                if (item.ProductionOrderId == "testCustom")
                {
                    Data.MaterialsRequestNote_Items.Remove(item);
                }
            }

            int AffectedRows = await this.service.UpdateModel(Data.Id, Data);

            this.service.UpdateIsCompleted(Data.Id, Data);

            Assert.True(AffectedRows > 0);
        }

    }


}
