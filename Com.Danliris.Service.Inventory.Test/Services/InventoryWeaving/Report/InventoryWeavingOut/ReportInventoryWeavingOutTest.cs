using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportGreigeWeavingPerGrade;
using Com.Danliris.Service.Inventory.Test.DataUtils.InventoryWeavingDataUtils.ReportGreigeWeavingPerGradeDataUtil;
using Com.Danliris.Service.Inventory.Test.DataUtils.InventoryWeavingDataUtils;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Moonlay.NetCore.Lib;
using Com.Danliris.Service.Inventory.Lib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using Microsoft.Extensions.Primitives;

namespace Com.Danliris.Service.Inventory.Test.Services.InventoryWeaving.Report.InventoryWeavingOut
{
    public class ReportInventoryWeavingOutTest
    {
        private const string ENTITY = "ReportGreigeWeavingOut";
        //private string username;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }


        private InventoryDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<InventoryDbContext> optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            InventoryDbContext dbContext = new InventoryDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private ReportGreigeWeavingPerGradeDataUtil _dataUtil(ReportGreigeWeavingPerGradeService service)
        {
            GetServiceProvider();
            return new ReportGreigeWeavingPerGradeDataUtil(service);
        }

        private InventoryWeavingDocumentOutDataUtil _dataUtilDoc(InventoryWeavingDocumentOutService service)
        {
            GetServiceProvider();
            return new InventoryWeavingDocumentOutDataUtil(service);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProvider;
        }

        private Mock<IServiceProvider> GetFailServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpFailTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });


            return serviceProvider;
        }

        [Fact]
        public async void Should_Succes_GetDistinctMaterialList()
        {
            InventoryWeavingDocumentOutService service = new InventoryWeavingDocumentOutService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var data = _dataUtilDoc(service).GetNewData();
            var Response = service.GetDistinctMaterial(25, 1, "{}", "{}", "{}");
            Assert.NotNull(Response);
        }

        [Fact]
        public async void Should_Success_GetMaterial()
        {
            InventoryWeavingDocumentOutService service = new InventoryWeavingDocumentOutService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var data = _dataUtilDoc(service).GetNewData();
            var Response = service.GetMaterialItemList("{}");
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_MapToModel()
        {
            InventoryWeavingDocumentOutService service = new InventoryWeavingDocumentOutService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var data = _dataUtilDoc(service).GetNewData1();
            var Response = service.MapToModel(data);
            Assert.NotNull(Response);
        }
        
        [Fact]
        public void Should_success_GetCsv()
        {
            InventoryWeavingDocumentOutService service = new InventoryWeavingDocumentOutService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var dataDoc = _dataUtilDoc(service).GetTestData();
            //var Responses =  Utilservice.Create(data);

            // var Service = new ReportGreigeWeavingPerGradeService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = service.DownloadCSVOut(DateTime.Now, DateTime.Now, 7, null);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_ReadById()
        {
            InventoryWeavingDocumentOutService service = new InventoryWeavingDocumentOutService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var data = _dataUtilDoc(service).GetTestData();

            var Response = service.ReadById(data.Result.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Read()
        {
            InventoryWeavingDocumentOutService service = new InventoryWeavingDocumentOutService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var data = _dataUtilDoc(service).GetTestData();

            var Response = service.Read(1, 25, "{}", "", "");
            Assert.NotNull(Response);
        }
    }
}
