using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportExpenseGreigeWeaving;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.WeavingInventory.Reports.ReportExpenseGreigeWeavingController;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.InventoryWeavingReport
{
    public class ReportExpenseGreigeWeavingControllerTest
    {
        protected Lib.Models.InventoryWeavingModel.InventoryWeavingMovement Model
        {
            get { return new Lib.Models.InventoryWeavingModel.InventoryWeavingMovement(); }
        }

        protected ExpenseReportViewModel ViewModel
        {
            get { return new ExpenseReportViewModel(); }
        }

        protected ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        protected (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IReportExpenseGreigeWeavingService> service) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), service: new Mock<IReportExpenseGreigeWeavingService>());
        }

        protected ReportExpenseGreigeWeavingController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IReportExpenseGreigeWeavingService> service) mocks)
        {
            Mock<ClaimsPrincipal> user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            ReportExpenseGreigeWeavingController controller = (ReportExpenseGreigeWeavingController)Activator.CreateInstance(typeof(ReportExpenseGreigeWeavingController), mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.service.Object);
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
        public void Get_Report_Success_GetExpense()
        {
            var mocks = GetMocks();
            mocks.service.Setup(f => f.GetReportExpense(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new Tuple<List<ExpenseReportViewModel>, int>(new List<ExpenseReportViewModel>(), 1));
            var controller = GetController(mocks);
            var response = controller.GetExpense("PRODUKSI", DateTime.MinValue, DateTime.Now, 1, 25, "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
            //Assert.NotNull(response);
        }


        [Fact]
        public void Get_Report_Fail_GetExpense()
        {
            var mocks = GetMocks();
            mocks.service.Setup(f => f.GetReportExpense(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = controller.GetExpense("PRODUKSI", DateTime.MinValue, DateTime.Now, 1, 25, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetXls_Report_Success_ReportExpense()
        {
            var mocks = GetMocks();
            mocks.service.Setup(f => f.GenerateExcelExpense(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime?>(), It.IsAny<int>()))
                .Returns(new MemoryStream());
            var controller = GetController(mocks);
            var response = controller.GetXlsAll("PRODUKSI", DateTime.MinValue, DateTime.Now);
            Assert.NotNull(response);
        }

        [Fact]
        public void GetXls_Report_Fail_ReportExpense()
        {
            var mocks = GetMocks();
            mocks.service.Setup(f => f.GenerateExcelExpense(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime?>(), It.IsAny<int>()))
                .Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = controller.GetXlsAll("PRODUKSI", DateTime.MinValue, DateTime.Now);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
