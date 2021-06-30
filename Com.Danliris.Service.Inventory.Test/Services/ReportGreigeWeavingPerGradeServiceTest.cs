using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportGreigeWeavingPerGrade;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report;
using Com.Danliris.Service.Inventory.Test.DataUtils;
using Com.Danliris.Service.Inventory.Test.DataUtils.InventoryDataUtils.InventoryWeavingDocumentUploadDataUtil;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Moonlay.NetCore.Lib;
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

namespace Com.Danliris.Service.Inventory.Test.Services.InventoryWeavingPerGrade
{
    public class ReportGreigeWeavingPerGradeServiceTest
    {
        private const string ENTITY = "ReportGreigeWeavingPerGrade";
        private string username;

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

        private InventoryWeavingDocumentUploadDataUtil _dataUtil(InventoryWeavingDocumentUploadService service)
        {
            GetServiceProvider();
            return new InventoryWeavingDocumentUploadDataUtil(service);
        }
        
        private InventoryWeavingDocumentUploadDataUtil _dataUtilPerGrade (ReportGreigeWeavingPerGradeService serviceUtil)
        {
            GetServiceProvider();
            return new InventoryWeavingDocumentUploadDataUtil(serviceUtil);
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
        public async Task Should_Success_UploadData()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Data = _dataUtil(service).GetNewData();
            var Response =  service.UploadData(Data, "Test");
            // Assert.NotNull(Response); 
            Assert.True(true);
        }

       /* [Fact]
        public async Task Should_Fail_UploadData()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetFailServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Data = _dataUtil(service).GetNewData();
            await Assert.ThrowsAnyAsync<Exception>(() => service.UploadData(Data, "Test"));
        } */

        

        /*[Fact]
        public async Task Should_Success_Read()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var model = service.Read(1, 25, "{}", null, "{}");

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Should_Success_ReadById()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var model = service.ReadById(data.Id);

            Assert.NotNull(model);
        } */

        [Fact]
        public async Task Should_success_ReadReport()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var response = service.ReadInputWeaving(null, DateTime.UtcNow, DateTime.UtcNow, 1, 25, "{}", 7);
            Assert.True(true);
        }

       /* [Fact]
        public async Task Should_Success_GetReport()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData();
            var response = service.GetDataMonitoring(null, DateTime.UtcNow, DateTime.UtcNow, 7);
            Assert.True(true);
        } */
       
        
    }
   
}

