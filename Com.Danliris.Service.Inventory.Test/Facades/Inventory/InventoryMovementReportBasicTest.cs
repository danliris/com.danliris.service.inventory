using System;
using System.Collections.Generic;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.Facades.InventoryFacades;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Test.DataUtils.InventoryDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Facades.Inventory
{
    [Collection("ServiceProviderFixture Collection")]
    public class InventoryMovementReportBasicTest
    {
        private IServiceProvider ServiceProvider { get; set; }

        public InventoryMovementReportBasicTest(ServiceProviderFixture fixture)
        {
            ServiceProvider = fixture.ServiceProvider;

            IdentityService identityService = (IdentityService)ServiceProvider.GetService(typeof(IdentityService));
            identityService.Username = "Unit Test";
        }
        private InventoryMovementDataUtil DataUtil
        {
            get { return (InventoryMovementDataUtil)ServiceProvider.GetService(typeof(InventoryMovementDataUtil)); }
        }
        private InventoryMovementReportFacade Facade
        {
            get { return (InventoryMovementReportFacade)ServiceProvider.GetService(typeof(InventoryMovementReportFacade)); }
        }

        //[Fact]
        //public async void Should_Success_Get_Report_Data()
        //{
        //    InventoryMovement model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.GetReport(model.StorageCode, model.ProductCode, model.Type, null, null, 1, 25, "{}", 7);
        //    Assert.NotEqual(Response.Item2, 0);
        //}

        //[Fact]
        //public async void Should_Success_Get_Report_Data_Null_Parameter()
        //{
        //    InventoryMovement model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.GetReport(null, null, null, null, null, 1, 25, "{}", 7);
        //    Assert.NotEqual(Response.Item2, 0);
        //}

        [Fact]
        public async void Should_Success_Get_Report_Data_Excel()
        {
            InventoryMovement model = await DataUtil.GetTestData("Unit test");
            var Response = Facade.GenerateExcel(model.StorageCode, model.ProductCode, model.Type, null, null, 7);
            Assert.IsType(typeof(System.IO.MemoryStream), Response);
        }

        [Fact]
        public async void Should_Success_Get_Report_Data_Excel_Null_Parameter()
        {
            InventoryMovement model = await DataUtil.GetTestData("Unit test");
            var Response = Facade.GenerateExcel("", "", "", null, null, 7);
            Assert.IsType(typeof(System.IO.MemoryStream), Response);
        }
    }
}