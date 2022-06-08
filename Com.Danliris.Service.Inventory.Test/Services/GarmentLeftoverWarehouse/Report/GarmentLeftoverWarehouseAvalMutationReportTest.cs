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
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.AvalMutation;
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
    public class GarmentLeftoverWarehouseAvalMutationReportTest
    {
        private const string ENTITY = "GarmentLeftoverWarehouseAvalMutationReportService";

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
            return new GarmentLeftoverWarehouseReceiptAvalDataUtil(service);
        }

        private GarmentLeftoverWarehouseExpenditureAvalDataUtil _dataUtilExpendAval(GarmentLeftoverWarehouseExpenditureAvalService service, string testName)
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

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProvider;
        }
        [Fact]
        public async Task Should_Success_GetReport_AvalBesarIn()
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

            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            GarmentLeftoverWarehouseAvalMutationReportService reportService = new GarmentLeftoverWarehouseAvalMutationReportService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataAvalFabric = await _dataUtilReceiptAval(service).GetTestData();
            var result1 = reportService.GetAvalBesarIn(DateTime.Now, DateTime.Now, 1, 1);
            Assert.NotNull(result1);

        }
        //
        [Fact]
        public async Task Should_Success_GetXLSReport_AvalBesarIn()
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

            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            GarmentLeftoverWarehouseAvalMutationReportService reportService = new GarmentLeftoverWarehouseAvalMutationReportService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataAvalFabric = await _dataUtilReceiptAval(service).GetTestData();

            var result1 = reportService.GenerateExcelAvalBesar(DateTime.Now, DateTime.Now);
            Assert.NotNull(result1);
        }

        [Fact]
        public async Task Should_Success_GetXLSReport_AvalBesarIn_Null()
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

            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            GarmentLeftoverWarehouseAvalMutationReportService reportService = new GarmentLeftoverWarehouseAvalMutationReportService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataAvalFabric = await _dataUtilReceiptAval(service).GetTestData();

            var result1 = reportService.GenerateExcelAvalBesar(DateTime.MaxValue, DateTime.MaxValue);
            Assert.NotNull(result1);
        }
        //
        [Fact]
        public async Task Should_Success_GetReport_AvalBesarOut()
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
            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            var dataAvalFabric = await _dataUtilReceiptAval(service).GetTestData();
            GarmentLeftoverWarehouseExpenditureAvalService serviceExpend = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            GarmentLeftoverWarehouseAvalMutationReportService reportService = new GarmentLeftoverWarehouseAvalMutationReportService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataExpendAvalFabric = await _dataUtilExpendAval(serviceExpend,GetCurrentMethod()).GetTestDataFabric();
            var result1 = reportService.GetAvalBesarOut(null, DateTime.Now, 1, 1);
            Assert.NotNull(result1);

        }
        //
        [Fact]
        public async Task Should_Success_GetXLSReport_AvalBesarOut()
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
            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            var dataAvalFabric = await _dataUtilReceiptAval(service).GetTestData();
            GarmentLeftoverWarehouseExpenditureAvalService serviceExpend = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            GarmentLeftoverWarehouseAvalMutationReportService reportService = new GarmentLeftoverWarehouseAvalMutationReportService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataExpendAvalFabric = await _dataUtilExpendAval(serviceExpend, GetCurrentMethod()).GetTestDataFabric();
            var result1 = reportService.GenerateExcelAvalBesarOut(DateTime.Now, DateTime.Now);
            Assert.NotNull(result1);
        }

        [Fact]
        public async Task Should_Success_GetXLSReport_AvalBesarOut_Null()
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
            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            var dataAvalFabric = await _dataUtilReceiptAval(service).GetTestData();
            GarmentLeftoverWarehouseExpenditureAvalService serviceExpend = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            GarmentLeftoverWarehouseAvalMutationReportService reportService = new GarmentLeftoverWarehouseAvalMutationReportService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataExpendAvalFabric = await _dataUtilExpendAval(serviceExpend, GetCurrentMethod()).GetTestDataFabric();
            var result1 = reportService.GenerateExcelAvalBesarOut(DateTime.MaxValue, DateTime.MaxValue);
            Assert.NotNull(result1);
        }
        //
        [Fact]
        public async Task Should_Success_GetReport_AvalKomponenIn()
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

            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            GarmentLeftoverWarehouseAvalMutationReportService reportService = new GarmentLeftoverWarehouseAvalMutationReportService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataAvalFabric = await _dataUtilReceiptAval(service).GetTestData1();
            var result1 = reportService.GetAvalKomponenIn(DateTime.Now, DateTime.Now, 1, 1);
            Assert.NotNull(result1);

        }
        //
        [Fact]
        public async Task Should_Success_GetXLSReport_AvalKomponenIn()
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

            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            GarmentLeftoverWarehouseAvalMutationReportService reportService = new GarmentLeftoverWarehouseAvalMutationReportService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataAvalFabric = await _dataUtilReceiptAval(service).GetTestData1();
            var result1 = reportService.GenerateExcelAvalKomponenIn(DateTime.Now, DateTime.Now);
            Assert.NotNull(result1);
        }

        [Fact]
        public async Task Should_Success_GetXLSReport_AvalKomponenIn_Null()
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

            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            GarmentLeftoverWarehouseAvalMutationReportService reportService = new GarmentLeftoverWarehouseAvalMutationReportService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataAvalFabric = await _dataUtilReceiptAval(service).GetTestData1();
            var result1 = reportService.GenerateExcelAvalKomponenIn(DateTime.MaxValue, DateTime.MaxValue);
            Assert.NotNull(result1);
        }
        //
        [Fact]
        public async Task Should_Success_GetReport_AvalKomponenOut()
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
            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            var dataAvalFabric = await _dataUtilReceiptAval(service).GetTestData();
            GarmentLeftoverWarehouseExpenditureAvalService serviceExpend = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            GarmentLeftoverWarehouseAvalMutationReportService reportService = new GarmentLeftoverWarehouseAvalMutationReportService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataExpendAvalFabric = await _dataUtilExpendAval(serviceExpend, GetCurrentMethod()).GetTestDataFabric1();
            var result1 = reportService.GetAvalKomponenOut(null, DateTime.Now, 1, 1);
            Assert.NotNull(result1);
        }

        [Fact]
        public async Task Should_Success_GetXLSReport_AvalKomponenOut()
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
            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            var dataAvalFabric = await _dataUtilReceiptAval(service).GetTestData();
            GarmentLeftoverWarehouseExpenditureAvalService serviceExpend = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            GarmentLeftoverWarehouseAvalMutationReportService reportService = new GarmentLeftoverWarehouseAvalMutationReportService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataExpendAvalFabric = await _dataUtilExpendAval(serviceExpend, GetCurrentMethod()).GetTestDataFabric1();
            var result1 = reportService.GenerateExcelAvalKomponenOut(DateTime.Now, DateTime.Now);
            Assert.NotNull(result1);
        }

        [Fact]
        public async Task Should_Success_GetXLSReport_AvalKomponenOut_Null()
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
            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            var dataAvalFabric = await _dataUtilReceiptAval(service).GetTestData();
            GarmentLeftoverWarehouseExpenditureAvalService serviceExpend = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            GarmentLeftoverWarehouseAvalMutationReportService reportService = new GarmentLeftoverWarehouseAvalMutationReportService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataExpendAvalFabric = await _dataUtilExpendAval(serviceExpend, GetCurrentMethod()).GetTestDataFabric1();
            var result1 = reportService.GenerateExcelAvalKomponenOut(DateTime.MaxValue, DateTime.MaxValue);
            Assert.NotNull(result1);
        }
        //
    }
}
