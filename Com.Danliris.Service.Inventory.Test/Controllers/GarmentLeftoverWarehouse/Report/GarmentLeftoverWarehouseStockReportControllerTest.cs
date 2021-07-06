using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.Report;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.GarmentLeftoverWarehouse.Report
{
    public class GarmentLeftoverWarehouseStockReportControllerTest
    {
        protected (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IGarmentLeftoverWarehouseStockReportService> Service) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), Service: new Mock<IGarmentLeftoverWarehouseStockReportService>());
        }

        protected GarmentLeftoverWarehouseStockReportController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IGarmentLeftoverWarehouseStockReportService> Service) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentLeftoverWarehouseStockReportController controller = new GarmentLeftoverWarehouseStockReportController(mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Service.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            return controller;
        }
        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }
        [Fact]
        public void Should_Success_GetFabricReport()
        {
            var mocks = GetMocks();

            mocks.Service.Setup(f => f.GetMonitoringFabric( It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new Tuple<List<GarmentLeftoverWarehouseStockMonitoringViewModel>, int>(new List<GarmentLeftoverWarehouseStockMonitoringViewModel>(), 1));


            var response = GetController(mocks).GetReportStockFabric( It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), "");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }
        [Fact]
        public void Should_Error_GetFabricReport()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetMonitoringFabric(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetReportStockFabric(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), "");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetXlsFabricReport()
        {
            var mocks = GetMocks();

            mocks.Service.Setup(f => f.GenerateExcelFabric( It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>())
               ).Returns(new MemoryStream());
            var response = GetController(mocks).GetXlStockFabric(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(),"");
            Assert.NotNull(response);

        }

        [Fact]
        public void Should_Error_GetXlsFabricReport()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcelFabric(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>())
            ).Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetXlStockFabric(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), "");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_GetAccReport()
        {
            var mocks = GetMocks();

            mocks.Service.Setup(f => f.GetMonitoringAcc(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new Tuple<List<GarmentLeftoverWarehouseStockMonitoringViewModel>, int>(new List<GarmentLeftoverWarehouseStockMonitoringViewModel>(), 1));


            var response = GetController(mocks).GetReportStockAcc(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), "");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }
        [Fact]
        public void Should_Error_GetAccReport()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetMonitoringAcc(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetReportStockAcc(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), "");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetXlsAccReport()
        {
            var mocks = GetMocks();

            mocks.Service.Setup(f => f.GenerateExcelAcc(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>())
               ).Returns(new MemoryStream());
            var response = GetController(mocks).GetXlsStockAcc(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), "");
            Assert.NotNull(response);

        }

        [Fact]
        public void Should_Error_GetXlsAcccReport()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcelAcc(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>())
            ).Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetXlsStockAcc(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), "");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetFinishedGoodReport()
        {
            var mocks = GetMocks();

            mocks.Service.Setup(f => f.GetMonitoringFinishedGood(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new Tuple<List<GarmentLeftoverWarehouseStockMonitoringViewModel>, int>(new List<GarmentLeftoverWarehouseStockMonitoringViewModel>(), 1));


            var response = GetController(mocks).GetReportStockFinishedGood(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), "");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }
        [Fact]
        public void Should_Error_GetFInishedGoodReport()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetMonitoringFinishedGood(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetReportStockFinishedGood(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), "");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetXlsFinishedGoodReport()
        {
            var mocks = GetMocks();

            mocks.Service.Setup(f => f.GenerateExcelFinishedGood(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>())
               ).Returns(new MemoryStream());
            var response = GetController(mocks).GetXlsStockFinishedGood(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), "");
            Assert.NotNull(response);

        }

        [Fact]
        public void Should_Error_GetXlsFinishedGoodReport()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcelFinishedGood(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>())
            ).Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetXlsStockFinishedGood(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), "");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetAvakReport()
        {
            var mocks = GetMocks();

            mocks.Service.Setup(f => f.GetMonitoringAval(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new Tuple<List<GarmentLeftoverWarehouseStockMonitoringViewModel>, int>(new List<GarmentLeftoverWarehouseStockMonitoringViewModel>(), 1));


            var response = GetController(mocks).GetReportStockAval(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),"");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }
        [Fact]
        public void Should_Error_GetAvalReport()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetMonitoringAval(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetReportStockAval(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), "");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetXlsAvalReport()
        {
            var mocks = GetMocks();

            mocks.Service.Setup(f => f.GenerateExcelAval(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(),"")
               ).Returns(new MemoryStream());
            var response = GetController(mocks).GetXlsStockAval(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), "");
            Assert.NotNull(response);

        }

        [Fact]
        public void Should_Error_GetXlsAvalReport()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcelAval(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), "")
            ).Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetXlsStockAval(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), "");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}

