using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodModels;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodViewModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Receipt;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodControllers;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodControllerTests
{
    public class BasicTest : BaseControllerTest<GarmentLeftoverWarehouseReceiptFinishedGoodController, GarmentLeftoverWarehouseReceiptFinishedGood, GarmentLeftoverWarehouseReceiptFinishedGoodViewModel, IGarmentLeftoverWarehouseReceiptFinishedGoodService>
    {
        private GarmentLeftoverWarehouseReceiptFinishedGoodViewModel vm = new GarmentLeftoverWarehouseReceiptFinishedGoodViewModel()
        {
            FinishedGoodReceiptNo = "no",
            ReceiptDate = DateTimeOffset.Now,
            ContractNo = "no",
            Description = "jhalh",
            ExpenditureDesc="dbasjdb",
            UnitFrom = new UnitViewModel
            {
                Name = "bName",
                Code = "bCode",
                Id = "1"
            },
            Items = new List<GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel>
                {
                    new GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel
                    {
                        Buyer=new BuyerViewModel
                        {
                            Id=1,
                            Name="name",
                            Code="code"
                        },
                        Quantity=10,
                        Uom=new UomViewModel
                        {
                            Unit="uom"
                        },
                        Article="apo",
                        Comodity=new ComodityViewModel
                        {
                            Name="name",
                            Code="code"
                        },
                        ExpenditureGoodNo="a",
                        LeftoverComodity= new LeftoverComodityViewModel
                        {
                            Name="name",
                            Code="code"
                        }
                    }
                }
        };
        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK_COMPONENT()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseReceiptFinishedGood>())).Returns(vm);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdf_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseReceiptFinishedGood>())).Returns(ViewModel);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_GetReport()
        {
            var mocks = GetMocks();

            mocks.Service.Setup(f => f.GetMonitoring(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), 7))
                .Returns(new Tuple<List<ReceiptFinishedGoodMonitoringViewModel>, int>(new List<ReceiptFinishedGoodMonitoringViewModel>(), 1));


            var response = GetController(mocks).GetReportAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), "");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }
        [Fact]
        public void Should_Error_GetReport()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetMonitoring(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), 7))
                .Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetReportAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), "");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetXlsReport()
        {
            var mocks = GetMocks();

            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>())
               ).Returns(new MemoryStream());
            var response = GetController(mocks).GetXlsAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>());
            Assert.NotNull(response);

        }

        [Fact]
        public void Should_Error_GetXlsReport()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>())
            ).Throws(new Exception());
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = GetController(mocks).GetXlsAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
