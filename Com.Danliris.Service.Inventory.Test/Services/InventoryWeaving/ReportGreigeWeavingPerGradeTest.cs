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

namespace Com.Danliris.Service.Inventory.Test.Services.InventoryWeaving
{
        public class ReportGreigeWeavingPerGradeServiceTest
        {
            private const string ENTITY = "ReportGreigeWeavingPerGrade";
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

            private ReportGreigeWeavingPerGradeDataUtil _dataUtil(ReportGreigeWeavingPerGradeService service, InventoryWeavingDocumentDataUtils dataDoc)
            {
                GetServiceProvider();
                return new ReportGreigeWeavingPerGradeDataUtil(service, dataDoc);
            }

        private InventoryWeavingDocumentDataUtils _dataUtilDoc(InventoryWeavingDocumentUploadService service)
        {
            GetServiceProvider();
            return new InventoryWeavingDocumentDataUtils(service);
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

        //[Fact]
        //public async Task Should_Success_Create()
        //{
        //    ReportGreigeWeavingPerGradeService service = new ReportGreigeWeavingPerGradeService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var Data = _dataUtil(service).GetNewData();
        //    var Response = service.Create(Data);
        //    //Assert.NotEqual(0, Response); 
        //    Assert.True(true);
        //}

        //[Fact]
        //public async Task Should_Fail_Create()
        //{
        //    ReportGreigeWeavingPerGradeService service = new ReportGreigeWeavingPerGradeService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var Data = _dataUtil(service).GetNewData();
        //    await Assert.ThrowsAnyAsync<Exception>(() => service.Create(Data));
        //}

        [Fact]
        public void Should_success_GetStock()
        {
            ReportGreigeWeavingPerGradeService service = new ReportGreigeWeavingPerGradeService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            InventoryWeavingDocumentUploadService serviceDoc = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            InventoryWeavingDocumentDataUtils dataDoc1 = new InventoryWeavingDocumentDataUtils(serviceDoc);

            var Utilservice = new ReportGreigeWeavingPerGradeService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var dataDoc = _dataUtilDoc(serviceDoc).GetTestData();

            var data = _dataUtil(service, dataDoc1).GetTestData();

            var Service = new ReportGreigeWeavingPerGradeService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = Utilservice.GetStockGrade(DateTime.UtcNow, 7 , 1, 25, "{}");
            Assert.NotNull(Response);
        }

        //[Fact]
        //public async Task Should_success_GetQuery()
        //{
        //    ReportGreigeWeavingPerGradeService service = new ReportGreigeWeavingPerGradeService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var data = await _dataUtil(service).GetTestData();
        //    var response = service.GetQuery(DateTime.UtcNow, 7);
        //    Assert.True(true);
        //}

        [Fact]
        public void Should_success_GenerateExcel()
        {
            ReportGreigeWeavingPerGradeService service = new ReportGreigeWeavingPerGradeService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            InventoryWeavingDocumentUploadService serviceDoc = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            InventoryWeavingDocumentDataUtils dataDoc1 = new InventoryWeavingDocumentDataUtils(serviceDoc);

            var Utilservice = new ReportGreigeWeavingPerGradeService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var dataDoc = _dataUtilDoc(serviceDoc).GetTestData();

            var data = _dataUtil(service, dataDoc1).GetTestData();

            var Service = new ReportGreigeWeavingPerGradeService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = Utilservice.GenerateExcel(DateTime.UtcNow, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

    }
}
