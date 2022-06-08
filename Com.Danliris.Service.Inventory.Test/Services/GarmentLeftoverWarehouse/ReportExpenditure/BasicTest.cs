using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Expenditure;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Test.DataUtils.IntegrationDataUtil;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Services.GarmentLeftoverWarehouse.ReportExpenditure
{
    public class BasicTest
    {
        const string ENTITY = "GarmentLeftoverWarehouseExpenditureFabric";

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

        private GarmentLeftoverWarehouseExpenditureFabricDataUtil _dataUtilFabric(GarmentLeftoverWarehouseExpenditureFabricService service)
        {
            return new GarmentLeftoverWarehouseExpenditureFabricDataUtil(service);
        }

        private GarmentLeftoverWarehouseExpenditureAccessoriesDataUtil _dataUtilAcc(GarmentLeftoverWarehouseExpenditureAccessoriesService service)
        {
            return new GarmentLeftoverWarehouseExpenditureAccessoriesDataUtil(service);
        }

        [Fact]
        public async Task GetReportFabric_Success()
        {
            var serviceProvider21 = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();

            //httpClientService
            //    .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
            //    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            //httpClientService
            //    .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
            //    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            //serviceProvider21
            //    .Setup(x => x.GetService(typeof(IIdentityService)))
            //    .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            //serviceProvider21
            //    .Setup(x => x.GetService(typeof(IHttpService)))
            //    .Returns(httpClientService.Object);

            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"POSerialNumber\":\"PONo\",\"BeacukaiNo\":\"BC001\",\"CustomsType\":\"A\",\"BeacukaiDate\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            HttpResponseMessage message2 = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message2.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":{\"Id\":\"7\",\"Composition\":\"Composition\",\"Const\":\"Const\",\"Yarn\":\"Yarn\",\"Width\":\"Width\"},\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            HttpResponseMessage message3 = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message3.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":\"7\",\"bcNo\":\"bcNo\",\"bcDate\":\"2018/10/20\",\"noteNo\":\"LocalSalesNoteNo\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-beacukai/by-poserialnumber"))))
                .ReturnsAsync(message);

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
                .ReturnsAsync(message3);



            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(message2);


            serviceProvider21
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider21
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            //serviceProvider21
            //    .Setup(x => x.GetService(typeof(IHttpService)))
            //    .Returns(new HttpTestService());

            GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object, _dbContext(GetCurrentMethod()));

            GarmentLeftoverWarehouseExpenditureFabricService serviceexpend = new GarmentLeftoverWarehouseExpenditureFabricService(_dbContext(GetCurrentMethod()), serviceProvider21.Object);


            var dataFabric = _dataUtilFabric(serviceexpend).GetNewData();

            await serviceexpend.CreateAsync(dataFabric);

            //GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object,_dbContext(GetCurrentMethod()));

            var result = service.GetReport(DateTime.Now.AddDays(-1), DateTime.Now, "FABRIC", 1, 25, "{}" ,1);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetReportAccessories_Success()
        {
            var serviceProvider21 = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();

            //httpClientService
            //    .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
            //    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            //httpClientService
            //    .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
            //    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            //serviceProvider21
            //    .Setup(x => x.GetService(typeof(IIdentityService)))
            //    .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            //serviceProvider21
            //    .Setup(x => x.GetService(typeof(IHttpService)))
            //    .Returns(httpClientService.Object);

            //GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object, _dbContext(GetCurrentMethod()));

            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"POSerialNumber\":\"PONo\",\"BeacukaiNo\":\"BC001\",\"CustomsType\":\"A\",\"BeacukaiDate\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            HttpResponseMessage message2 = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message2.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":{\"Id\":\"7\",\"Composition\":\"Composition\",\"Const\":\"Const\",\"Yarn\":\"Yarn\",\"Width\":\"Width\"},\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            HttpResponseMessage message3 = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message3.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":\"7\",\"bcNo\":\"bcNo\",\"bcDate\":\"2018/10/20\",\"noteNo\":\"LocalSalesNoteNo\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-beacukai/by-poserialnumber"))))
                .ReturnsAsync(message);

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
                .ReturnsAsync(message3);



            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(message2);


            serviceProvider21
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider21
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            //serviceProvider21
            //    .Setup(x => x.GetService(typeof(IHttpService)))
            //    .Returns(new HttpTestService());

            GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object, _dbContext(GetCurrentMethod()));

            GarmentLeftoverWarehouseExpenditureAccessoriesService serviceexpend = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider21.Object);


            var dataFabric = _dataUtilAcc(serviceexpend).GetNewData();

            await serviceexpend.CreateAsync(dataFabric);

            var result = service.GetReport(DateTime.Now.AddDays(-1), DateTime.Now, "ACCESSORIES", 1, 25, "{}", 1);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetReportNullAccessories_Success()
        {
            var serviceProvider21 = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();

            //httpClientService
            //    .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
            //    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            //httpClientService
            //    .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
            //    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            //serviceProvider21
            //    .Setup(x => x.GetService(typeof(IIdentityService)))
            //    .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            //serviceProvider21
            //    .Setup(x => x.GetService(typeof(IHttpService)))
            //    .Returns(httpClientService.Object);

            //GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object, _dbContext(GetCurrentMethod()));

            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"POSerialNumber\":\"PONo\",\"BeacukaiNo\":\"BC001\",\"CustomsType\":\"A\",\"BeacukaiDate\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            HttpResponseMessage message2 = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message2.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":{\"Id\":\"7\",\"Composition\":\"Composition\",\"Const\":\"Const\",\"Yarn\":\"Yarn\",\"Width\":\"Width\"},\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            HttpResponseMessage message3 = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message3.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":\"7\",\"bcNo\":\"bcNo\",\"bcDate\":\"2018/10/20\",\"noteNo\":\"LocalSalesNoteNo\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-beacukai/by-poserialnumber"))))
                .ReturnsAsync(message);

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
                .ReturnsAsync(message3);



            //httpClientService
            //    .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
            //    .ReturnsAsync(message2);


            serviceProvider21
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider21
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            //serviceProvider21
            //    .Setup(x => x.GetService(typeof(IHttpService)))
            //    .Returns(new HttpTestService());

            GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object, _dbContext(GetCurrentMethod()));

            GarmentLeftoverWarehouseExpenditureAccessoriesService serviceexpend = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider21.Object);


            var dataFabric = _dataUtilAcc(serviceexpend).GetNewData();

            await serviceexpend.CreateAsync(dataFabric);

            var result = service.GetReport(DateTime.Now.AddDays(-1), DateTime.Now, "ACCESSORIES", 1, 25, "{}", 1);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetReportAllType_Success()
        {
            var serviceProvider21 = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object, _dbContext(GetCurrentMethod()));

            var result = service.GetReport(null, DateTime.Now, "", 1, 25, "{}", 1);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Excel_ReportFabricSuccess()
        {
            var serviceProvider21 = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();

            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"POSerialNumber\":\"PONo\",\"BeacukaiNo\":\"BC001\",\"CustomsType\":\"A\",\"BeacukaiDate\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            HttpResponseMessage message2 = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message2.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":{\"Id\":\"7\",\"Composition\":\"Composition\",\"Const\":\"Const\",\"Yarn\":\"Yarn\",\"Width\":\"Width\"},\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-beacukai/by-poserialnumber"))))
                .ReturnsAsync(message);

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(message2);


            serviceProvider21
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider21
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            //serviceProvider21
            //    .Setup(x => x.GetService(typeof(IHttpService)))
            //    .Returns(new HttpTestService());

            GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object, _dbContext(GetCurrentMethod()));

            GarmentLeftoverWarehouseExpenditureFabricService serviceexpend = new GarmentLeftoverWarehouseExpenditureFabricService(_dbContext(GetCurrentMethod()), serviceProvider21.Object);


            var dataFabric = _dataUtilFabric(serviceexpend).GetNewData();

            await serviceexpend.CreateAsync(dataFabric);

            var result = service.GenerateExcel(DateTime.Now.AddDays(-1), DateTime.Now, "FABRIC", 1);

            Assert.IsType<MemoryStream>(result);


        }


        [Fact]
        public async Task Excel_NullProduct_ReportFabricSuccess()
        {
            var serviceProvider21 = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();

            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"POSerialNumber\":\"PONo\",\"BeacukaiNo\":\"BC001\",\"CustomsType\":\"A\",\"BeacukaiDate\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            HttpResponseMessage message2 = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message2.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":{\"Id\":\"7\",\"Composition\":null,\"Const\":null,\"Yarn\":null,\"Width\":null},\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-beacukai/by-poserialnumber"))))
                .ReturnsAsync(message);

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });



            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(message2);


            serviceProvider21
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider21
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            //serviceProvider21
            //    .Setup(x => x.GetService(typeof(IHttpService)))
            //    .Returns(new HttpTestService());

            GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object, _dbContext(GetCurrentMethod()));

            GarmentLeftoverWarehouseExpenditureFabricService serviceexpend = new GarmentLeftoverWarehouseExpenditureFabricService(_dbContext(GetCurrentMethod()), serviceProvider21.Object);


            var dataFabric = _dataUtilFabric(serviceexpend).GetNewData();

            await serviceexpend.CreateAsync(dataFabric);

            var result = service.GenerateExcel(DateTime.Now.AddDays(-1), DateTime.Now, "FABRIC", 1);

            Assert.IsType<MemoryStream>(result);


        }

        [Fact]
        public async Task Excel_ReportAccessoriesSuccess()
        {
            var serviceProvider21 = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();

            //httpClientService
            //    .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
            //    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            //httpClientService
            //    .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
            //    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });


            //serviceProvider21
            //    .Setup(x => x.GetService(typeof(IIdentityService)))
            //    .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            //serviceProvider21
            //    .Setup(x => x.GetService(typeof(IHttpService)))
            //    .Returns(httpClientService.Object);

            //GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object, _dbContext(GetCurrentMethod()));

            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"POSerialNumber\":\"PONo\",\"BeacukaiNo\":\"BC001\",\"CustomsType\":\"A\",\"BeacukaiDate\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            HttpResponseMessage message2 = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message2.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":{\"Id\":\"7\",\"Composition\":\"Composition\",\"Const\":\"Const\",\"Yarn\":\"Yarn\",\"Width\":\"Width\"},\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            HttpResponseMessage message3 = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message3.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":\"7\",\"bcNo\":\"bcNo\",\"bcDate\":\"2018/10/20\",\"noteNo\":\"LocalSalesNoteNo\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"CustomsType\",\"BeacukaiDate\",\"BeacukaiNo\",,\"POSerialNumber\"]}}");

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-beacukai/by-poserialnumber"))))
                .ReturnsAsync(message);

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
                .ReturnsAsync(message3);



            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(message2);


            serviceProvider21
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProvider21
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            //serviceProvider21
            //    .Setup(x => x.GetService(typeof(IHttpService)))
            //    .Returns(new HttpTestService());

            GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object, _dbContext(GetCurrentMethod()));

            GarmentLeftoverWarehouseExpenditureAccessoriesService serviceexpend = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider21.Object);


            var dataFabric = _dataUtilAcc(serviceexpend).GetNewData();

            await serviceexpend.CreateAsync(dataFabric);

            var result = service.GenerateExcel(DateTime.Now, DateTime.Now, "ACCESSORIES", 1);

            Assert.IsType<MemoryStream>(result);


        }

        [Fact]
        public void Excel_ReportAllTypeSuccess()
        {
            var serviceProvider21 = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/local-cover-letters"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garmentProducts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<Dictionary<string, Object>>())) });


            serviceProvider21
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            GarmentLeftoverWarehouseReportExpenditureService service = new GarmentLeftoverWarehouseReportExpenditureService(serviceProvider21.Object, _dbContext(GetCurrentMethod()));



            var result = service.GenerateExcel(DateTime.Now.AddDays(-1), DateTime.Now, "", 1);

            Assert.IsType<MemoryStream>(result);


        }
    }
}
