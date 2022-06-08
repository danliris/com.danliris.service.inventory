using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportGreigeWeavingPerGrade;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using Com.Danliris.Service.Inventory.Test.DataUtils.InventoryWeavingDataUtils;
using Com.Danliris.Service.Inventory.Test.DataUtils.InventoryWeavingDataUtils.ReportGreigeWeavingPerGradeDataUtil;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Services.InventoryWeaving.InventoryWeavingUpload
{
    public class InventoryDocumentAdjServiceTest
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

        private InventoryWeavingDocumentOutDataUtil _dataUtilDocOut(InventoryWeavingDocumentOutService service)
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
        public void Should_Succes_Create()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            InventoryWeavingDocumentAdjService serviceAdj = new InventoryWeavingDocumentAdjService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var data = _dataUtilDoc(service).GetNewDataAdj();
            var Response = serviceAdj.Create(data);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Succes_Read()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            InventoryWeavingDocumentAdjService serviceAdj = new InventoryWeavingDocumentAdjService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var data = _dataUtilDoc(service).GetNewDataAdj();
            var dataAdj = serviceAdj.Create(data);

            var Response = serviceAdj.Read(1, 25, "{}", "", "");

            Assert.NotNull(Response);
        }


        [Fact]
        public void Should_Succes_GetMaterialItemList_Null_Material()
        {
            InventoryWeavingDocumentUploadService serviceDoc = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            ReportGreigeWeavingPerGradeService service = new ReportGreigeWeavingPerGradeService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            InventoryWeavingDocumentAdjService serviceAdj = new InventoryWeavingDocumentAdjService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            InventoryWeavingDocumentDataUtils dataDoc1 = new InventoryWeavingDocumentDataUtils(serviceDoc);

            
            var data = _dataUtil(service, dataDoc1).GetTestData();

            //var dataDoc = _dataUtilDoc(serviceDoc).GetNewDataAdj();
            //var dataAdj = serviceAdj.Create(dataDoc);

            var Response = serviceAdj.GetMaterialItemList(null);

            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Succes_GetMaterialItemList_NotNull_Material()
        {
            InventoryWeavingDocumentUploadService serviceDoc = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            ReportGreigeWeavingPerGradeService service = new ReportGreigeWeavingPerGradeService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            InventoryWeavingDocumentAdjService serviceAdj = new InventoryWeavingDocumentAdjService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            InventoryWeavingDocumentDataUtils dataDoc1 = new InventoryWeavingDocumentDataUtils(serviceDoc);


            var data = _dataUtil(service, dataDoc1).GetTestData();

            //var dataDoc = _dataUtilDoc(serviceDoc).GetNewDataAdj();
            //var dataAdj = serviceAdj.Create(dataDoc);

            var Response = serviceAdj.GetMaterialItemList("Name");

            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_MapToModel()
        {
            InventoryWeavingDocumentAdjService serviceAdj = new InventoryWeavingDocumentAdjService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            InventoryWeavingDocumentOutService serviceOut = new InventoryWeavingDocumentOutService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            
            var data = _dataUtilDocOut(serviceOut).GetCSVDownloadOut();

            var Response = serviceAdj.MapToModel(data);
            Assert.NotNull(Response);
        }

    }
}
