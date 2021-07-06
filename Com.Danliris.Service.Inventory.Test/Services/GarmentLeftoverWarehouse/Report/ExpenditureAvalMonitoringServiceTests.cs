using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalServices;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Expenditure.Aval;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalDataUtils;
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
    public class ExpenditureAvalMonitoringServiceTests
    {
        private const string ENTITY = "GarmentLeftoverWarehouseExpenditureAvalMonitoring";

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

        private GarmentLeftoverWarehouseExpenditureAvalDataUtil _dataUtil(GarmentLeftoverWarehouseExpenditureAvalService service, string testName)
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
        public async Task Should_Success_GetReport()
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

            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            ExpenditureAvalMonitoringService Reportservice = new ExpenditureAvalMonitoringService(_dbContext(GetCurrentMethod()), serviceProvider.Object);


            var dataAvalFabric = await _dataUtil(service, GetCurrentMethod()).GetTestDataFabric(); ;
            var result1 = Reportservice.GetMonitoring(DateTime.Now, DateTime.Now, "AVAL FABRIC", 1, 1, "{}", 7);
            Assert.NotNull(result1);

            var dataAvalComponent = await _dataUtil(service,GetCurrentMethod()).GetTestDataCOMPONEN();
            var dataAvalComponent2 = await _dataUtil(service,GetCurrentMethod()).GetTestDataCOMPONEN();
            var result2 = Reportservice.GetMonitoring(DateTime.Now, DateTime.Now, "AVAL KOMPONEN", 2, 1, "{}", 7);
            Assert.NotNull(result2);

            var dataAvalBP = await _dataUtil(service,GetCurrentMethod()).GetTestDataAcc(); ;
            var result3 = Reportservice.GetMonitoring(DateTime.Now, DateTime.Now, "AVAL BAHAN PENOLONG", 1, 1, "{}", 7);
            Assert.NotNull(result3);

        }

        [Fact]
        public async Task Should_Success_GetXlsReport()
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

            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            ExpenditureAvalMonitoringService Reportservice = new ExpenditureAvalMonitoringService(_dbContext(GetCurrentMethod()), serviceProvider.Object);


            var dataAval = await _dataUtil(service,GetCurrentMethod()).GetTestDataFabric();
            var dataAval2 = await _dataUtil(service,GetCurrentMethod()).GetTestDataFabric();

            var result = Reportservice.GenerateExcel(DateTime.Now, DateTime.Now, "AVAL FABRIC", 7);


            Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Success_GetXlsReport_zero_data()
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

            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            ExpenditureAvalMonitoringService Reportservice = new ExpenditureAvalMonitoringService(_dbContext(GetCurrentMethod()), serviceProvider.Object);


            var dataAval = await _dataUtil(service,GetCurrentMethod()).GetTestDataFabric();

            var result = Reportservice.GenerateExcel(DateTime.Now, DateTime.Now, "AVAL KOMPONEN", 7);


            Assert.NotNull(result);
        }
    }
}
