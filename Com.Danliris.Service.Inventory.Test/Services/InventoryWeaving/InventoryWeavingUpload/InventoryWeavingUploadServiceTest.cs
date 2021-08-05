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

namespace Com.Danliris.Service.Inventory.Test.Services.InventoryWeaving.InventoryWeavingUpload
{
    public class InventoryWeavingUploadServiceTest
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



        [Fact]
        public async Task Create_Success_Upload()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var data = _dataUtilDoc(service).GetNewData();

            var Response = service.UploadData(data, "test");
            Assert.NotNull(Response);
        }

        [Fact]
        public async void Should_Success_UploadValidate()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            //Models.GarmentCurrency model = await DataUtil.GetTestDataAsync();
            List<InventoryWeavingDocumentCsvViewModel> CSV = new List<InventoryWeavingDocumentCsvViewModel>
            {
                new InventoryWeavingDocumentCsvViewModel
                {
                    ReferenceNo = "referencce",
                    Construction = "CD",
                    Grade = "a",
                    Piece = "1",
                    MaterialName = "CD",
                    WovenType = "a",
                    Yarn1 = "yarn1",
                    Yarn2 = "yarn2",
                    YarnType1 = "yt1",
                    YarnType2 = "yt2",
                    YarnOrigin1 = "yo1",
                    YarnOrigin2 = "yo2",
                    Width = "1",
                    Qty = "1",
                    QtyPiece = "1",
                    ProductionOrderNo = "a"
                }
            };

            List<KeyValuePair<string, StringValues>> body = new List<KeyValuePair<string, StringValues>>();
            KeyValuePair<string, StringValues> keyValue = new KeyValuePair<string, StringValues>("date", "2020-01-10");
            body.Add(keyValue);

            var Response = service.UploadValidate( ref CSV, body);
            Assert.NotNull(Response);
        }


        [Fact]
        public async Task Should_Success_MapToViewModel()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            //Models.GarmentCurrency model = await DataUtil.GetTestDataAsync();
            List<InventoryWeavingDocumentCsvViewModel> CSV = new List<InventoryWeavingDocumentCsvViewModel>
            {
                new InventoryWeavingDocumentCsvViewModel
                {
                    ReferenceNo = "referencce",
                    Construction = "CD",
                    Grade = "a",
                    Piece = "1",
                    MaterialName = "CD",
                    WovenType = "a",
                    Yarn1 = "yarn1",
                    Yarn2 = "yarn2",
                    YarnType1 = "yt1",
                    YarnType2 = "yt2",
                    YarnOrigin1 = "yo1",
                    YarnOrigin2 = "yo2",
                    Width = "1",
                    Qty = "1",
                    QtyPiece = "1",
                    ProductionOrderNo = "a"
                }
            };

            var Response = service.MapToViewModel(CSV, DateTime.Now, "test");
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_MapToModel()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

           
            var data = _dataUtilDoc(service).GetNewData1();
            var Response = await service.MapToModel(data);
            Assert.NotNull(Response);
        }


        [Fact]
        public async Task Should_Success_ReadById()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var data = _dataUtilDoc(service).GetTestData();

            var Response = service.ReadById(data.Result.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Read()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var data = _dataUtilDoc(service).GetTestData();

            var Response = service.Read(1, 25, "{}", "", "");
            Assert.NotNull(Response);
        }


        [Fact]
        public void Should_Success_CheckNota()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            //Models.GarmentCurrency model = await DataUtil.GetTestDataAsync();
            List<InventoryWeavingDocumentCsvViewModel> CSV = new List<InventoryWeavingDocumentCsvViewModel>
            {
                new InventoryWeavingDocumentCsvViewModel
                {
                    ReferenceNo = "referencce",
                    Construction = "CD",
                    Grade = "a",
                    Piece = "1",
                    MaterialName = "CD",
                    WovenType = "a",
                    Yarn1 = "yarn1",
                    Yarn2 = "yarn2",
                    YarnType1 = "yt1",
                    YarnType2 = "yt2",
                    YarnOrigin1 = "yo1",
                    YarnOrigin2 = "yo2",
                    Width = "1",
                    Qty = "1",
                    QtyPiece = "1",
                    ProductionOrderNo = "a"
                }
            };

            var Response = service.checkNota(CSV);
            //Assert.NotNull(Response);
        }



        [Fact]
        public void Should_Success_ReadInputWeaving()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var data = _dataUtilDoc(service).GetTestData();

            var Response = service.ReadInputWeaving("", null, null, 1, 25, "{}",7);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_success_GenerateExcel()
        {
            

         

            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var dataDoc = _dataUtilDoc(service).GetTestData();
            //var Responses =  Utilservice.Create(data);

           // var Service = new ReportGreigeWeavingPerGradeService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var Response = service.GenerateExcel("",null,null, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }


        [Fact]
        public async Task Update_Success()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IInventoryWeavingMovementService>();
            stockServiceMock.Setup(s => s.UpdateAsync(It.IsAny<InventoryWeavingMovement>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IInventoryWeavingMovementService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService(serviceProvider.Object, _dbContext(GetCurrentMethod()));

            var dataUtil = _dataUtilDoc(service);
            var oldData = dataUtil.GetNewData();
            oldData.Items.Add(new InventoryWeavingDocumentItem
            {
                ProductOrderName = "product2",
                ReferenceNo = "referencce2",
                Construction = "CD2",
                Grade = "A2",
                Piece = "12",
                MaterialName = "CD2",
                WovenType = "",
                Yarn1 = "yarn12",
                Yarn2 = "yarn22",
                YarnType1 = "yt12",
                YarnType2 = "yt22",
                YarnOrigin1 = "yo12",
                YarnOrigin2 = "yo22",
                Width = "12",
                UomUnit = "MTR2",
                UomId = 2,
                Quantity = 2,
                QuantityPiece = 2,
                ProductRemark = "",
                InventoryWeavingDocumentId = 1,
            });

            await service.Create(oldData);

            var newData = dataUtil.CopyData(oldData);
            //newData.ExpenditureDate = newData.ExpenditureDate.AddDays(-1);
            newData.Remark = "New" + newData.Remark;
            //newData.LocalSalesNoteNo = "New" + newData.LocalSalesNoteNo;
            var firsItem = newData.Items.First();
            firsItem.Quantity++;
            var lastItem = newData.Items.Last();
            lastItem.Id = 0;

            var result = await service.UpdateAsync(newData.Id, newData);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Update_Error()
        {
            InventoryWeavingDocumentUploadService service = new InventoryWeavingDocumentUploadService( GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            await Assert.ThrowsAnyAsync<Exception>(() => service.UpdateAsync(0, null));
        }

    }
}
