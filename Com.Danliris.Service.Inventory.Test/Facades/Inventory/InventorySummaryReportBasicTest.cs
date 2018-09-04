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
    public class InventorySummaryReportBasicTest
    {
        private IServiceProvider ServiceProvider { get; set; }

        public InventorySummaryReportBasicTest(ServiceProviderFixture fixture)
        {
            ServiceProvider = fixture.ServiceProvider;

            IdentityService identityService = (IdentityService)ServiceProvider.GetService(typeof(IdentityService));
            identityService.Username = "Unit Test";
        }
        private InventorySummaryDataUtil DataUtil
        {
            get { return (InventorySummaryDataUtil)ServiceProvider.GetService(typeof(InventorySummaryDataUtil)); }
        }
        private InventorySummaryReportFacade Facade
        {
            get { return (InventorySummaryReportFacade)ServiceProvider.GetService(typeof(InventorySummaryReportFacade)); }
        }

        [Fact]
        public async void Should_Success_Get_Report_Data()
        {
            InventorySummary model = await DataUtil.GetTestData("Unit test");
            var Response = Facade.GetReport(model.StorageCode, model.ProductCode, 1, 25, "{}", 7);
            Assert.NotEqual(Response.Item2, 0);
        }

        [Fact]
        public async void Should_Success_Get_Report_Data_Null_Parameter()
        {
            InventorySummary model = await DataUtil.GetTestData("Unit test");
            var Response = Facade.GetReport("", "", 1, 25, "{}", 7);
            Assert.NotEqual(Response.Item2, 0);
        }

        [Fact]
        public async void Should_Success_Get_Report_Data_Excel()
        {
            InventorySummary model = await DataUtil.GetTestData("Unit test");
            var Response = Facade.GenerateExcel(model.StorageCode, model.ProductCode, 7);
            Assert.IsType(typeof(System.IO.MemoryStream), Response);
        }

        [Fact]
        public async void Should_Success_Get_Report_Data_Excel_Null_Parameter()
        {
            InventorySummary model = await DataUtil.GetTestData("Unit test");
            var Response = Facade.GenerateExcel("", "", 7);
            Assert.IsType(typeof(System.IO.MemoryStream), Response);
        }
    }
}