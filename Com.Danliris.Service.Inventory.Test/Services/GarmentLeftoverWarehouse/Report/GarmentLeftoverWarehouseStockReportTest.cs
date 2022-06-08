using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalServices;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricServices;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodServices;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalDataUtils;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricDataUtils;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodDataUtils;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Services.GarmentLeftoverWarehouse.Report
{
    public class GarmentLeftoverWarehouseStockReportTest
    {
        private const string ENTITY = "GarmentLeftoverWarehouseStockReport";

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
        private GarmentLeftoverWarehouseReceiptAvalDataUtil _dataUtilReceiptAval(GarmentLeftoverWarehouseReceiptAvalService service)
        {

            GetServiceProvider();
            return new GarmentLeftoverWarehouseReceiptAvalDataUtil(service);
        }
        private GarmentLeftoverWarehouseExpenditureAvalDataUtil _dataUtilAval(GarmentLeftoverWarehouseExpenditureAvalService service, string testName)
        {
            var serviceProvider = GetServiceProvider();
            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseReceiptAvalService receiptService = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(testName), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptAvalDataUtil ReceiptAvalDataUtil = new GarmentLeftoverWarehouseReceiptAvalDataUtil(receiptService);
            return new GarmentLeftoverWarehouseExpenditureAvalDataUtil(service, ReceiptAvalDataUtil);
        }
        private GarmentLeftoverWarehouseReceiptFabricDataUtil _dataUtilReceiptFabric(GarmentLeftoverWarehouseReceiptFabricService service)
        {

            GetServiceProvider();
            return new GarmentLeftoverWarehouseReceiptFabricDataUtil(service);
        }
      
        private GarmentLeftoverWarehouseExpenditureFabricDataUtil _dataUtilFabric(GarmentLeftoverWarehouseExpenditureFabricService service)
        {

            GetServiceProvider();
            return new GarmentLeftoverWarehouseExpenditureFabricDataUtil(service);
        }
        private GarmentLeftoverWarehouseReceiptAccessoriesDataUtil _dataUtilReceiptAcc(GarmentLeftoverWarehouseReceiptAccessoriesService service)
        {

            GetServiceProvider();
            return new GarmentLeftoverWarehouseReceiptAccessoriesDataUtil(service);
        }
        private GarmentLeftoverWarehouseExpenditureAccessoriesDataUtil _dataUtilAcc(GarmentLeftoverWarehouseExpenditureAccessoriesService service)
        {

            GetServiceProvider();
            return new GarmentLeftoverWarehouseExpenditureAccessoriesDataUtil(service);
        }
        private GarmentLeftoverWarehouseExpenditureFinishedGoodDataUtil _dataUtilFinishedGood(GarmentLeftoverWarehouseExpenditureFinishedGoodService service)
        {

            GetServiceProvider();
            return new GarmentLeftoverWarehouseExpenditureFinishedGoodDataUtil(service);
        }
        private GarmentLeftoverWarehouseReceiptFinishedGoodDataUtil _dataUtilReceiptFinishedGood(GarmentLeftoverWarehouseReceiptFinishedGoodService service)
        {

            GetServiceProvider();
            return new GarmentLeftoverWarehouseReceiptFinishedGoodDataUtil(service);
        }
        private GarmentLeftoverWarehouseBalanceStockDataUtil _dataUtilbalanceStock(GarmentLeftoverWarehouseBalanceStockService service)
        {

            GetServiceProvider();
            return new GarmentLeftoverWarehouseBalanceStockDataUtil(service);
        }
        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

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
        public async Task Should_Success_GetACCReport()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseStockReportService utilService = new GarmentLeftoverWarehouseStockReportService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseExpenditureAccessoriesService service = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseBalanceStockService _balanceservice = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptAccessoriesService receiptFabservice = new GarmentLeftoverWarehouseReceiptAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            
            var data_Balance = _dataUtilbalanceStock(_balanceservice).GetTestData_ACC();

            var dataReceiptAcc = _dataUtilReceiptAcc(receiptFabservice).GetTestData();

            var dataAcc = await _dataUtilAcc(service).GetTestData();
            var result = utilService.GetMonitoringAcc( DateTime.Now, DateTime.Now, 1, 1, 25, "{}", 7);


            Assert.NotNull(result);
        }
        [Fact]
        public async Task Should_Success_GetACCExcelReport()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseStockReportService utilService = new GarmentLeftoverWarehouseStockReportService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseExpenditureAccessoriesService service = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseBalanceStockService _balanceservice = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptAccessoriesService receiptFabservice = new GarmentLeftoverWarehouseReceiptAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data_Balance = _dataUtilbalanceStock(_balanceservice).GetTestData_ACC();

            var dataReceiptAcc = _dataUtilReceiptAcc(receiptFabservice).GetTestData();

            var dataAcc = await _dataUtilAcc(service).GetTestData();
            var result = utilService.GenerateExcelAcc(DateTime.Now, DateTime.Now, 1, 7);


            Assert.NotNull(result);
        }
        [Fact]
        public async Task Should_Success_GetFInishedGoodReport()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            

            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseBalanceStockService _balanceservice = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptFinishedGoodService receiptservice = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data_Balance = _dataUtilbalanceStock(_balanceservice).GetTestData_FINISHEDGOOD();

            var dataReceiptFInishedGood = _dataUtilReceiptFinishedGood(receiptservice).GetTestData();

            var dataFinishedGood = await _dataUtilFinishedGood(service).GetTestData();

            var httpClientService = new Mock<IHttpService>();

            var ExpendGood = new List<ExpendGoodViewModel> {
                new ExpendGoodViewModel
                {
                    RONo = "roNo",
                    Comodity = new GarmentComodity
                    {
                        Code = "Code",
                        Id = 1,
                        Name = "Name"
                    }
                }
            };

            Dictionary<string, object> result2 =
                new ResultFormatter("1.0", WebApi.Helpers.General.OK_STATUS_CODE, WebApi.Helpers.General.OK_MESSAGE)
                .Ok(ExpendGood);

            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("expenditure-goods/traceable-by-ro")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent( JsonConvert.SerializeObject(result2)) });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            GarmentLeftoverWarehouseStockReportService utilService = new GarmentLeftoverWarehouseStockReportService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var result = utilService.GetMonitoringFinishedGood(DateTime.Now, DateTime.Now, 1, 1, 25, "{}", 7);

            var httpClientService2 = new Mock<IHttpService>();

            httpClientService2
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("expenditure-goods/traceable-by-ro")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService2.Object);

            GarmentLeftoverWarehouseStockReportService utilService2 = new GarmentLeftoverWarehouseStockReportService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var resultinternalserver = utilService.GetMonitoringFinishedGood(DateTime.Now, DateTime.Now, 1, 1, 25, "{}", 7);


            Assert.NotNull(result);
        }
        [Fact]
        public async Task Should_Success_GetFInishedGoodExcelReport()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            

            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseBalanceStockService _balanceservice = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptFinishedGoodService receiptservice = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data_Balance = _dataUtilbalanceStock(_balanceservice).GetTestData_FINISHEDGOOD();

            var dataReceiptFInishedGood = _dataUtilReceiptFinishedGood(receiptservice).GetTestData();

            var httpClientService = new Mock<IHttpService>();

            var ExpendGood = new List<ExpendGoodViewModel> {
                new ExpendGoodViewModel
                {
                    RONo = "roNo",
                    Comodity = new GarmentComodity
                    {
                        Code = "Code",
                        Id = 1,
                        Name = "Name"
                    }
                }
            };

            Dictionary<string, object> result2 =
                new ResultFormatter("1.0", WebApi.Helpers.General.OK_STATUS_CODE, WebApi.Helpers.General.OK_MESSAGE)
                .Ok(ExpendGood);

            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("expenditure-goods/traceable-by-ro")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(result2)) });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);



            var dataFinishedGood = await _dataUtilFinishedGood(service).GetTestData();

            GarmentLeftoverWarehouseStockReportService utilService = new GarmentLeftoverWarehouseStockReportService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            var result = utilService.GenerateExcelFinishedGood(DateTime.Now, DateTime.Now, 1, 7);


            Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Failed_GetFInishedGoodExcelReport()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());



            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseBalanceStockService _balanceservice = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptFinishedGoodService receiptservice = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data_Balance = _dataUtilbalanceStock(_balanceservice).GetTestData_FINISHEDGOOD();

            var dataReceiptFInishedGood = _dataUtilReceiptFinishedGood(receiptservice).GetTestData();

            var httpClientService = new Mock<IHttpService>();

            var ExpendGood = new List<ExpendGoodViewModel> {
                new ExpendGoodViewModel
                {
                    RONo = "roNo",
                    Comodity = new GarmentComodity
                    {
                        Code = "Code",
                        Id = 10,
                        Name = "Name"
                    }
                }
            };

            Dictionary<string, object> result2 =
                new ResultFormatter("1.0", WebApi.Helpers.General.OK_STATUS_CODE, WebApi.Helpers.General.OK_MESSAGE)
                .Ok(ExpendGood);

            httpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("expenditure-goods/traceable-by-ro")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(result2)) });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);



            var dataFinishedGood = await _dataUtilFinishedGood(service).GetTestData();

            GarmentLeftoverWarehouseStockReportService utilService = new GarmentLeftoverWarehouseStockReportService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            var result = utilService.GenerateExcelFinishedGood(DateTime.Now, DateTime.Now, 10, 7);


            Assert.NotNull(result);
        }
        [Fact]
        public async Task Should_Success_GetFabricReport()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseStockReportService utilService = new GarmentLeftoverWarehouseStockReportService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseExpenditureFabricService service = new GarmentLeftoverWarehouseExpenditureFabricService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseBalanceStockService _balanceservice = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptFabricService receiptservice = new GarmentLeftoverWarehouseReceiptFabricService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data_Balance = _dataUtilbalanceStock(_balanceservice).GetTestData_FABRIC();

            var dataReceiptFabric = _dataUtilReceiptFabric(receiptservice).GetTestData();

            var dataFabric = await _dataUtilFabric(service).GetTestData();

            GarmentLeftoverWarehouseStockService serviceStock = new GarmentLeftoverWarehouseStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
            {
                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FABRIC,
                UnitId = 1,
                UnitCode = "Unit",
                UnitName = "UnitFromName",
                PONo = "PONo",
                ProductCode ="ProductCode",
                ProductName ="ProductName",
                Quantity = 1,
                UomId = 1,
                UomUnit = "Uom"
            };

            var resultStock = await serviceStock.StockIn(stock, "StockReferenceNo", 1, 1);
            await serviceStock.StockIn(stock, "StockReferenceNo", 1, 1);
            var result = utilService.GetMonitoringFabric(DateTime.Now, DateTime.Now, 1, 1, 25, "{}", 7);


            Assert.NotNull(result);
        }
        [Fact]
        public async Task Should_Success_GetFabricExcelReport()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseStockReportService utilService = new GarmentLeftoverWarehouseStockReportService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseExpenditureFabricService service = new GarmentLeftoverWarehouseExpenditureFabricService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseBalanceStockService _balanceservice = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptFabricService receiptservice = new GarmentLeftoverWarehouseReceiptFabricService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data_Balance = _dataUtilbalanceStock(_balanceservice).GetTestData_FABRIC();

            var dataReceiptFabric = _dataUtilReceiptFabric(receiptservice).GetTestData();

            var dataFabric = await _dataUtilFabric(service).GetTestData();

            GarmentLeftoverWarehouseStockService serviceStock = new GarmentLeftoverWarehouseStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
            {
                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FABRIC,
                UnitId = 1,
                UnitCode = "Unit",
                UnitName = "UnitFromName",
                PONo = "PONo",
                ProductCode = "ProductCode",
                ProductName = "ProductName",
                Quantity = 1,
                UomId = 1,
                UomUnit = "Uom"
            };

            var resultStock = await serviceStock.StockIn(stock, "StockReferenceNo", 1, 1);
            await serviceStock.StockIn(stock, "StockReferenceNo", 1, 1);
            var result = utilService.GenerateExcelFabric(DateTime.Now, DateTime.Now, 1, 7);


            Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Success_GetAvalFabricReport()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseStockReportService utilService = new GarmentLeftoverWarehouseStockReportService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseBalanceStockService _balanceservice = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptAvalService receiptservice = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data_Balance = _dataUtilbalanceStock(_balanceservice).GetTestData_FABRIC();

            var dataReceiptAval = _dataUtilReceiptAval(receiptservice).GetTestData();

            var dataAval = await _dataUtilAval(service, GetCurrentMethod()).GetNewDataFabric();

            GarmentLeftoverWarehouseStockService serviceStock = new GarmentLeftoverWarehouseStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            var result = utilService.GetMonitoringAval(DateTime.Now, DateTime.Now, 1, 1, 25, "{}",7, "AVAL FABRIC");
            Assert.NotNull(result);
        }

       
        [Fact]
        public async Task Should_Success_GetAvalAccReport()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseStockReportService utilService = new GarmentLeftoverWarehouseStockReportService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseBalanceStockService _balanceservice = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptAvalService receiptservice = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data_Balance = _dataUtilbalanceStock(_balanceservice).GetTestData_FABRIC();

            var dataReceiptAval = _dataUtilReceiptAval(receiptservice).GetTestData();

            var dataAval = await _dataUtilAval(service, GetCurrentMethod()).GetNewDataFabric();

            GarmentLeftoverWarehouseStockService serviceStock = new GarmentLeftoverWarehouseStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);


            var result = utilService.GetMonitoringAval(DateTime.Now, DateTime.Now, 1, 1, 1, "{}", 7, "AVAL BAHAN PENOLONG");


            Assert.NotNull(result);
        }
        [Fact]
        public async Task Should_Success_GetAvalAccExcelReport()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseStockReportService utilService = new GarmentLeftoverWarehouseStockReportService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseBalanceStockService _balanceservice = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptAvalService receiptservice = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data_Balance = _dataUtilbalanceStock(_balanceservice).GetTestData_FABRIC();

            var dataReceiptAval = _dataUtilReceiptAval(receiptservice).GetTestData();

            var dataAval = await _dataUtilAval(service, GetCurrentMethod()).GetNewDataFabric();

            GarmentLeftoverWarehouseStockService serviceStock = new GarmentLeftoverWarehouseStockService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);


            var result = utilService.GenerateExcelAval(DateTime.Now, DateTime.Now, 1,  7, "AVAL BAHAN PENOLONG");


            Assert.NotNull(result);
        }

        [Fact]
        private async Task TestSendError()
        {
            HttpService httpService = new HttpService(new IdentityService());
            await Assert.ThrowsAnyAsync<Exception>(() => httpService.SendAsync(HttpMethod.Get, null, null));
        }
    }
}

