using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricServices;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Receipt;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricDataUtils;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Test.Helpers;
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
    public class ReceiptMonitoringTests
    {
        private const string ENTITY = "GarmentLeftoverWarehouseReceiptMonitoring";

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

        private GarmentLeftoverWarehouseReceiptFabricDataUtil _dataUtilFabric(GarmentLeftoverWarehouseReceiptFabricService service)
        {
            return new GarmentLeftoverWarehouseReceiptFabricDataUtil(service);
        }

        private GarmentLeftoverWarehouseReceiptAccessoriesDataUtil _dataUtilAcc(GarmentLeftoverWarehouseReceiptAccessoriesService service)
        {
            return new GarmentLeftoverWarehouseReceiptAccessoriesDataUtil(service);
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
        public async Task Should_Success_GetReport_Fabric()
        {
            var serviceProvider1 = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider1
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider1
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            var serviceProvider = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"POSerialNumber\":\"PONo\",\"BeacukaiNo\":\"BC001\",\"CustomsType\":\"A\",\"BeacukaiDate\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-beacukai/by-poserialnumber"))))
                .ReturnsAsync(message);

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            GarmentLeftoverWarehouseReceiptFabricService service = new GarmentLeftoverWarehouseReceiptFabricService(_dbContext(GetCurrentMethod()), serviceProvider1.Object);
            ReceiptMonitoringService Reportservice = new ReceiptMonitoringService(_dbContext(GetCurrentMethod()), serviceProvider.Object);


            var dataFabric = _dataUtilFabric(service).GetNewData();

            dataFabric.ReceiptDate = dataFabric.ReceiptDate.AddDays(-1);

            await service.CreateAsync(dataFabric);

            var result1 = Reportservice.GetFabricReceiptMonitoring(null, DateTime.Now, 1, 1, "{}", 7);
            Assert.NotNull(result1);


        }

        [Fact]
        public async Task Should_Success_GetReport_Fabric_page2()
        {
            var serviceProvider1 = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider1
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider1
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            var serviceProvider = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"POSerialNumber\":\"PONo\",\"BeacukaiNo\":\"BC001\",\"CustomsType\":\"A\",\"BeacukaiDate\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-beacukai/by-poserialnumber"))))
                .ReturnsAsync(message);

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            GarmentLeftoverWarehouseReceiptFabricService service = new GarmentLeftoverWarehouseReceiptFabricService(_dbContext(GetCurrentMethod()), serviceProvider1.Object);
            ReceiptMonitoringService Reportservice = new ReceiptMonitoringService(_dbContext(GetCurrentMethod()), serviceProvider.Object);


            var dataFabric = await _dataUtilFabric(service).GetTestData();
            var dataFabric2 = await _dataUtilFabric(service).GetTestData();
            var result1 = Reportservice.GetFabricReceiptMonitoring(DateTime.Now, DateTime.Now, 2, 1, "{}", 7);
            Assert.NotNull(result1);


        }

        [Fact]
        public async Task Should_Success_GetXlsReport()
        {
            var serviceProvider1 = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider1
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider1
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            var serviceProvider = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"POSerialNumber\":\"PONo\",\"BeacukaiNo\":\"BC001\",\"CustomsType\":\"A\",\"BeacukaiDate\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-beacukai/by-poserialnumber"))))
                .ReturnsAsync(message);

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);


            GarmentLeftoverWarehouseReceiptFabricService service = new GarmentLeftoverWarehouseReceiptFabricService(_dbContext(GetCurrentMethod()), serviceProvider1.Object);
            ReceiptMonitoringService Reportservice = new ReceiptMonitoringService(_dbContext(GetCurrentMethod()), serviceProvider.Object);


            var dataFabric = _dataUtilFabric(service).GetNewData();

            dataFabric.ReceiptDate = dataFabric.ReceiptDate.AddDays(-1);

            await service.CreateAsync(dataFabric); dataFabric = await _dataUtilFabric(service).GetTestData();

            var result1 = Reportservice.GenerateExcelFabric(null, DateTime.Now, 7);
            Assert.NotNull(result1);

        }

        [Fact]
        public async Task Should_Success_GetXlsReport_zero_data()
        {


            var serviceProvider1 = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider1
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider1
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            var serviceProvider = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"POSerialNumber\":\"PONo\",\"BeacukaiNo\":\"BC001\",\"CustomsType\":\"A\",\"BeacukaiDate\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-beacukai/by-poserialnumber"))))
                .ReturnsAsync(message);

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);
 
            GarmentLeftoverWarehouseReceiptFabricService service = new GarmentLeftoverWarehouseReceiptFabricService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            ReceiptMonitoringService Reportservice = new ReceiptMonitoringService(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            var result = Reportservice.GenerateExcelFabric(DateTime.Now, DateTime.Now, 7);


            Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Success_GetReport_Acc()
        {
            var serviceProvider1 = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider1
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider1
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            var serviceProvider = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"POSerialNumber\":\"PONo\",\"BeacukaiNo\":\"BC001\",\"CustomsType\":\"A\",\"BeacukaiDate\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-beacukai/by-poserialnumber"))))
                .ReturnsAsync(message);

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            GarmentLeftoverWarehouseReceiptAccessoriesService service = new GarmentLeftoverWarehouseReceiptAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider1.Object);
            ReceiptMonitoringService Reportservice = new ReceiptMonitoringService(_dbContext(GetCurrentMethod()), serviceProvider.Object);


            var datAcc = _dataUtilAcc(service).GetNewData();

            datAcc.StorageReceiveDate = datAcc.StorageReceiveDate.AddDays(-1);

            await service.CreateAsync(datAcc);
            var result1 = Reportservice.GetAccessoriesReceiptMonitoring(null, DateTime.Now, 1, 1, "{}", 7);

            Assert.NotNull(result1);


        }


        //[Fact] 
        //public async Task Should_Success_GetReport_Acc_page2()
        //{
        //    var serviceProvider1 = GetServiceProvider();

        //    var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
        //    stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
        //        .ReturnsAsync(1);

        //    stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
        //      .ReturnsAsync(1);

        //    serviceProvider1
        //        .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
        //        .Returns(stockServiceMock.Object);

        //    serviceProvider1
        //        .Setup(x => x.GetService(typeof(IHttpService)))
        //        .Returns(new HttpTestService());

        //    var serviceProvider = new Mock<IServiceProvider>();

        //    var httpClientService = new Mock<IHttpService>();

        //    HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        //    message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"POSerialNumber\":\"PONo\",\"BeacukaiNo\":\"BC001\",\"CustomsType\":\"A\",\"BeacukaiDate\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

        //    httpClientService
        //        .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-beacukai/by-poserialnumber"))))
        //        .ReturnsAsync(message);

        //    serviceProvider
        //        .Setup(x => x.GetService(typeof(IIdentityService)))
        //        .Returns(new IdentityService() { Token = "Token", Username = "Test" });

        //    serviceProvider
        //        .Setup(x => x.GetService(typeof(IHttpService)))
        //        .Returns(httpClientService.Object);

        //    GarmentLeftoverWarehouseReceiptAccessoriesService service = new GarmentLeftoverWarehouseReceiptAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider1.Object);
        //    ReceiptMonitoringService Reportservice = new ReceiptMonitoringService(_dbContext(GetCurrentMethod()), serviceProvider.Object);


        //    var dataAcc = await _dataUtilAcc(service).GetTestData();
        //    var dataAcc2 = await _dataUtilAcc(service).GetTestData();
        //    var result1 = Reportservice.GetAccessoriesReceiptMonitoring(DateTime.Now, DateTime.Now, 2, 1, "{}", 7);
        //    Assert.NotNull(result1);


        //}

        [Fact]
        public async Task Should_Success_GetXlsReport_Acc()
        {
            var serviceProvider1 = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider1
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider1
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            var serviceProvider = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"POSerialNumber\":\"PONo\",\"BeacukaiNo\":\"BC001\",\"CustomsType\":\"A\",\"BeacukaiDate\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-beacukai/by-poserialnumber"))))
                .ReturnsAsync(message);

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);


            GarmentLeftoverWarehouseReceiptAccessoriesService service = new GarmentLeftoverWarehouseReceiptAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider1.Object);
            ReceiptMonitoringService Reportservice = new ReceiptMonitoringService(_dbContext(GetCurrentMethod()), serviceProvider.Object);


            var dataAcc = await _dataUtilAcc(service).GetTestData();
            var result1 = Reportservice.GenerateExcelAccessories(DateTime.Now, DateTime.Now, 7);
            Assert.NotNull(result1);

        }

        [Fact]
        public async Task Should_Success_GetXlsReport_zero_data_Acc()
        {


            var serviceProvider1 = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider1
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider1
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            var serviceProvider = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"POSerialNumber\":\"PONo\",\"BeacukaiNo\":\"BC001\",\"CustomsType\":\"A\",\"BeacukaiDate\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-beacukai/by-poserialnumber"))))
                .ReturnsAsync(message);

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            GarmentLeftoverWarehouseReceiptAccessoriesService service = new GarmentLeftoverWarehouseReceiptAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider1.Object);
            ReceiptMonitoringService Reportservice = new ReceiptMonitoringService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var result = Reportservice.GenerateExcelAccessories(DateTime.Now, DateTime.Now, 7);


            Assert.NotNull(result);
        }

    }
}