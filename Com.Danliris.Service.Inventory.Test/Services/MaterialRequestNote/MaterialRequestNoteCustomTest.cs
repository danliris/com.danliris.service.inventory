//using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
//using Com.Danliris.Service.Inventory.Lib.Services.MaterialsRequestNoteServices;
//using Com.Danliris.Service.Inventory.Test.DataUtils.MaterialRequestNoteDataUtil;
//using Com.Danliris.Service.Inventory.Test.Helpers;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Xunit;
//using static Com.Danliris.Service.Inventory.Lib.Services.MaterialsRequestNoteServices.MaterialsRequestNoteService;
//using model = Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;

//namespace Com.Danliris.Service.Inventory.Test.Services.MaterialRequestNote
//{
//    [Collection("ServiceProviderFixture Collection")]
//    public class MaterialRequestNoteCustomTest
//    {
//        //private readonly MaterialRequestNoteDataUtil dataUtil;
//        //private readonly MaterialsRequestNoteService service;

//        private IServiceProvider serviceProvider { get; set; }
//        //private readonly List<string> Keys;
//        public MaterialRequestNoteCustomTest(ServiceProviderFixture fixture)
//        {
//            serviceProvider = fixture.ServiceProvider;
//            //Keys = keys;
//        }

//        protected MaterialRequestNoteDataUtil DataUtil
//        {
//            get { return (MaterialRequestNoteDataUtil)this.serviceProvider.GetService(typeof(MaterialRequestNoteDataUtil)); }
//        }

//        protected MaterialsRequestNoteService Service
//        {
//            get
//            {
//                MaterialsRequestNoteService service = (MaterialsRequestNoteService)this.serviceProvider.GetService(typeof(MaterialsRequestNoteService));
//                service.Username = "Unit Test";
//                service.Token = HttpClientTestService.Token;

//                return service;
//            }
//        }

//        protected MaterialsRequestNoteService DbContext
//        {
//            get { return (MaterialsRequestNoteService)this.serviceProvider.GetService(typeof(MaterialsRequestNoteService)); }
//        }

//        [Fact]
//        public async void Should_Success_Update_Data()
//        {
//            model.MaterialsRequestNote Data = await DataUtil.GetTestData();

//            foreach (MaterialsRequestNote_Item item in Data.MaterialsRequestNote_Items)
//            {
//                if (item.ProductionOrderId == "testCustom")
//                {
//                    Data.MaterialsRequestNote_Items.Remove(item);
//                }
//            }

//            int AffectedRows = await this.Service.UpdateModel(Data.Id, Data);

//            this.Service.UpdateIsCompleted(Data.Id, Data);

//            Assert.True(AffectedRows > 0);

//        }

//    }


//}
