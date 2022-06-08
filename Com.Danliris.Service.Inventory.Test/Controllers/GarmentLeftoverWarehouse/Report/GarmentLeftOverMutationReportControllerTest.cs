using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.AvalMutation;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Mutation;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseAval;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Mutation;
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
    public class GarmentLeftOverMutationReportControllerTest
    {
        protected (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IGarmentLeftoverWarehouseMutationReportService> Service, Mock<IGarmentLeftoverWarehouseAvalMutationReportService> Service2) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), Service: new Mock<IGarmentLeftoverWarehouseMutationReportService>(), Service2: new Mock<IGarmentLeftoverWarehouseAvalMutationReportService>());
        }

        protected GarmentLeftOverMutationReportController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IGarmentLeftoverWarehouseMutationReportService> Service,Mock<IGarmentLeftoverWarehouseAvalMutationReportService> Service2) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentLeftOverMutationReportController controller = new GarmentLeftOverMutationReportController(mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Service.Object, mocks.Service2.Object);
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
        public void Should_Success_GetMutationReport()
        {
            var mocks = GetMocks();

            mocks.Service.Setup(f => f.GetMutation(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new Tuple<List<GarmentLeftoverWarehouseMutationReportViewModel>, int>(new List<GarmentLeftoverWarehouseMutationReportViewModel>(), 1));


            var response = GetController(mocks).GetMutation(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }

        [Fact]
        public void Should_Error_GetMutationReport()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetMutation(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetMutation(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetXlsMutationReport()
        {
            var mocks = GetMocks();

            mocks.Service.Setup(f => f.GenerateExcelMutation(It.IsAny<DateTime>(), It.IsAny<DateTime>())
               ).Returns(new MemoryStream());
            var response = GetController(mocks).GetXlsScrap(It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.NotNull(response);

        }

        [Fact]
        public void Should_Error_GetXlsMutationReport()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcelMutation(It.IsAny<DateTime>(), It.IsAny<DateTime>())
            ).Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetXlsScrap(It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_GetAvalBesarInReport()
        {
            var mocks = GetMocks();

            mocks.Service2.Setup(f => f.GetAvalBesarIn(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new Tuple<List<GarmentLeftoverWarehouseAval>, int>(new List<GarmentLeftoverWarehouseAval>(), 1));


            var response = GetController(mocks).GetAvalBesar(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }

        [Fact]
        public void Should_Error_GetAvalBesarInReport()
        {
            var mocks = GetMocks();
            mocks.Service2.Setup(f => f.GetAvalBesarIn(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetAvalBesar(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_GetAvalBesarOutReport()
        {
            var mocks = GetMocks();

            mocks.Service2.Setup(f => f.GetAvalBesarOut(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new Tuple<List<GarmentLeftoverWarehouseAval>, int>(new List<GarmentLeftoverWarehouseAval>(), 1));


            var response = GetController(mocks).GetAvalBesarOut(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }

        [Fact]
        public void Should_Error_GetAvalBesarOutReport()
        {
            var mocks = GetMocks();
            mocks.Service2.Setup(f => f.GetAvalBesarOut(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetAvalBesarOut(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetAvalKomponenInReport()
        {
            var mocks = GetMocks();

            mocks.Service2.Setup(f => f.GetAvalKomponenIn(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new Tuple<List<GarmentLeftoverWarehouseAval>, int>(new List<GarmentLeftoverWarehouseAval>(), 1));


            var response = GetController(mocks).GetAvalKomponenIn(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }

        [Fact]
        public void Should_Error_GetAvalKomponenInReport()
        {
            var mocks = GetMocks();
            mocks.Service2.Setup(f => f.GetAvalKomponenIn(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetAvalKomponenIn(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetAvalKomponenOutReport()
        {
            var mocks = GetMocks();

            mocks.Service2.Setup(f => f.GetAvalKomponenOut(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new Tuple<List<GarmentLeftoverWarehouseAval>, int>(new List<GarmentLeftoverWarehouseAval>(), 1));


            var response = GetController(mocks).GetAvalKomponenOut(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }

        [Fact]
        public void Should_Error_GetAvalKomponenOutReport()
        {
            var mocks = GetMocks();
            mocks.Service2.Setup(f => f.GetAvalKomponenOut(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetAvalKomponenOut(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetXlsAvalBesarInReport()
        {
            var mocks = GetMocks();

            mocks.Service2.Setup(f => f.GenerateExcelAvalBesar(It.IsAny<DateTime>(), It.IsAny<DateTime>())
               ).Returns(new MemoryStream());
            var response = GetController(mocks).GetXlsAvalBesar(It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.NotNull(response);

        }

        [Fact]
        public void Should_Error_GetXlsAvalBesarInReport()
        {
            var mocks = GetMocks();
            mocks.Service2.Setup(f => f.GenerateExcelAvalBesar(It.IsAny<DateTime>(), It.IsAny<DateTime>())
            ).Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetXlsAvalBesar(It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_GetXlsAvalBesarOutReport()
        {
            var mocks = GetMocks();

            mocks.Service2.Setup(f => f.GenerateExcelAvalBesarOut(It.IsAny<DateTime>(), It.IsAny<DateTime>())
               ).Returns(new MemoryStream());
            var response = GetController(mocks).GetXlsAvalBesarOut(It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.NotNull(response);

        }

        [Fact]
        public void Should_Error_GetXlsAvalBesarOutReport()
        {
            var mocks = GetMocks();
            mocks.Service2.Setup(f => f.GenerateExcelAvalBesarOut(It.IsAny<DateTime>(), It.IsAny<DateTime>())
            ).Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetXlsAvalBesarOut(It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_GetXlsAvalKomponenInReport()
        {
            var mocks = GetMocks();

            mocks.Service2.Setup(f => f.GenerateExcelAvalKomponenIn(It.IsAny<DateTime>(), It.IsAny<DateTime>())
               ).Returns(new MemoryStream());
            var response = GetController(mocks).GetXlsAvalKomponen(It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.NotNull(response);

        }

        [Fact]
        public void Should_Error_GetXlsAvalKomponenInReport()
        {
            var mocks = GetMocks();
            mocks.Service2.Setup(f => f.GenerateExcelAvalKomponenIn(It.IsAny<DateTime>(), It.IsAny<DateTime>())
            ).Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetXlsAvalKomponen(It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetXlsAvalKomponenOutReport()
        {
            var mocks = GetMocks();

            mocks.Service2.Setup(f => f.GenerateExcelAvalKomponenOut(It.IsAny<DateTime>(), It.IsAny<DateTime>())
               ).Returns(new MemoryStream());
            var response = GetController(mocks).GetXlsAvalKomponenOut(It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.NotNull(response);

        }

        [Fact]
        public void Should_Error_GetXlsAvalKomponenOutReport()
        {
            var mocks = GetMocks();
            mocks.Service2.Setup(f => f.GenerateExcelAvalKomponenOut(It.IsAny<DateTime>(), It.IsAny<DateTime>())
            ).Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetXlsAvalKomponenOut(It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
