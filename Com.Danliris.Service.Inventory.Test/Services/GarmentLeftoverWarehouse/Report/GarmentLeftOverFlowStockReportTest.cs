using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricServices;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodServices;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Bookkeeping;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricDataUtils;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodDataUtils;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.Stock;
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
   public class GarmentLeftOverFlowStockReportTest
    {
        private const string ENTITY = "flowStockReport";

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
        public async Task Should_Success_GetFlowStockReportTypeFabric()
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

            GarmentLeftoverWarehouseFlowStockReportService utilService = new GarmentLeftoverWarehouseFlowStockReportService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseExpenditureFabricService service = new GarmentLeftoverWarehouseExpenditureFabricService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseBalanceStockService _balanceservice = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptFabricService receiptFabservice = new GarmentLeftoverWarehouseReceiptFabricService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataFabric =await _dataUtilFabric(service).GetTestData();
             
            var data_Balance = _dataUtilbalanceStock(_balanceservice).GetNewData_FABRIC();

            var dataReceiptAcc = _dataUtilReceiptFabric(receiptFabservice).GetTestData();

            var result = utilService.GetMonitoringFlowStock("FABRIC", DateTime.Now, DateTime.Now, 1, 1, 1, "{}", 7);
             
       
           Assert.NotNull(result);
        }
        [Fact]
        public async Task Should_Success_GetFlowStockReportTypeAcc()
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

            GarmentLeftoverWarehouseFlowStockReportService utilService = new GarmentLeftoverWarehouseFlowStockReportService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseExpenditureAccessoriesService service = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseBalanceStockService _balanceservice = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptAccessoriesService receiptAccservice = new GarmentLeftoverWarehouseReceiptAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataFabric = await _dataUtilAcc(service).GetTestData(); 

            var data_Balance = _dataUtilbalanceStock(_balanceservice).GetTestData_FABRIC();

            var dataReceiptAcc= _dataUtilReceiptAcc(receiptAccservice).GetTestData();
            var result = utilService.GetMonitoringFlowStock("ACCESSORIES", DateTime.Now, DateTime.Now, 1, 1, 1, "{}", 7);


            Assert.NotNull(result);
        }
        [Fact]
        public async Task Should_Success_GetFlowStockReportTypeFinishedGood()
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

            GarmentLeftoverWarehouseFlowStockReportService utilService = new GarmentLeftoverWarehouseFlowStockReportService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseBalanceStockService _balanceservice = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptFinishedGoodService receiptFInishedGoodservice = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataFInishedGood = await _dataUtilReceiptFinishedGood(receiptFInishedGoodservice).GetTestData();

            var data_Balance = _dataUtilbalanceStock(_balanceservice).GetTestData_FINISHEDGOOD();

            var dataReceiptFinishedGood = _dataUtilFinishedGood(service).GetTestData();
            var result = utilService.GetMonitoringFlowStock("BARANG JADI", DateTime.Now, DateTime.Now, 1, 1, 1, "{}", 7);


            Assert.NotNull(result);
        }
        [Fact]
        public async Task Should_Success_GetExcelFlowStockReportTypeFinishedGood()
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

            GarmentLeftoverWarehouseFlowStockReportService utilService = new GarmentLeftoverWarehouseFlowStockReportService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseBalanceStockService _balanceservice = new GarmentLeftoverWarehouseBalanceStockService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            GarmentLeftoverWarehouseReceiptFinishedGoodService receiptFInishedGoodservice = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataFInishedGood = await _dataUtilReceiptFinishedGood(receiptFInishedGoodservice).GetTestData();

            var data_Balance = _dataUtilbalanceStock(_balanceservice).GetTestData_FINISHEDGOOD();

            var dataReceiptFinishedGood = _dataUtilFinishedGood(service).GetTestData();
            var result = utilService.GenerateExcelFlowStock("BARANG JADI", DateTime.Now, DateTime.Now, 1, 7);


            Assert.NotNull(result);
        }
    }
}
