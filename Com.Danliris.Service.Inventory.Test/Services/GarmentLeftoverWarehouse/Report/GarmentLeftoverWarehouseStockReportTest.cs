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
            var result = utilService.GetMonitoringAcc( DateTime.Now, DateTime.Now, 1, 1, 1, "{}", 7);


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

            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseBalanceStockService _balanceservice = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptFinishedGoodService receiptservice = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data_Balance = _dataUtilbalanceStock(_balanceservice).GetTestData_FINISHEDGOOD();

            var dataReceiptFInishedGood = _dataUtilReceiptFinishedGood(receiptservice).GetTestData();

            var dataFinishedGood = await _dataUtilFinishedGood(service).GetTestData();
            var result = utilService.GetMonitoringFinishedGood(DateTime.Now, DateTime.Now, 1, 1, 1, "{}", 7);


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

            GarmentLeftoverWarehouseStockReportService utilService = new GarmentLeftoverWarehouseStockReportService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseBalanceStockService _balanceservice = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptFinishedGoodService receiptservice = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data_Balance = _dataUtilbalanceStock(_balanceservice).GetTestData_FINISHEDGOOD();

            var dataReceiptFInishedGood = _dataUtilReceiptFinishedGood(receiptservice).GetTestData();

            var dataFinishedGood = await _dataUtilFinishedGood(service).GetTestData();
            var result = utilService.GenerateExcelFinishedGood(DateTime.Now, DateTime.Now, 1, 7);


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
            var result = utilService.GetMonitoringFabric(DateTime.Now, DateTime.Now, 1, 1, 1, "{}", 7);


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
            var result = utilService.GetMonitoringAval(DateTime.Now, DateTime.Now, 1, 1, 1, "{}",7, "AVAL FABRIC");
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
    }
}

