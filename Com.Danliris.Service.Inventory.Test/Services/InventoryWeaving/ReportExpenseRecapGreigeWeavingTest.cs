using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportExpenseRecapGreigeWeaving;
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
    public class ReportExpenseRecapGreigeWeavingTest
    {
        private const string ENTITY = "ReportExpenseRecapGreigeWeaving";
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

         [Fact]
        public async Task Should_success_GetStock()
        {
            ReportGreigeWeavingPerGradeService service = new ReportGreigeWeavingPerGradeService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var Utilservice = new ReportExpenseRecapGreigeWeavingService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetTestData();

            InventoryWeavingDocumentUploadService serviceDoc = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var dataDoc = _dataUtilDoc(serviceDoc).GetTestData();
            //var Responses =  Utilservice.Create(data);

            var Service = new ReportExpenseRecapGreigeWeavingService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = Utilservice.GetReportExpenseRecap("PRODUKSI", DateTime.MinValue, DateTime.UtcNow, 25, 1, "{}", 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_success_GenerateExcel()
        {
            ReportGreigeWeavingPerGradeService service = new ReportGreigeWeavingPerGradeService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var Utilservice = new ReportExpenseRecapGreigeWeavingService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetTestData();

            InventoryWeavingDocumentUploadService serviceDoc = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var dataDoc = _dataUtilDoc(serviceDoc).GetTestData();
            //var Responses =  Utilservice.Create(data);

            var Service = new ReportGreigeWeavingPerGradeService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = Utilservice.GenerateExcelExpenseRecap(null, DateTime.MinValue, DateTime.UtcNow, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }
    }
}
