using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseExpenditureFinishedGoodControllers;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.GarmentLeftoverWarehouse.ExpenditureFinishedGood
{
    public class BasicTest : BaseControllerTest<GarmentLeftoverWarehouseExpenditureFinishedGoodController, GarmentLeftoverWarehouseExpenditureFinishedGood, GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel, IGarmentLeftoverWarehouseExpenditureFinishedGoodService>
    {

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK_JUAL_LOKAL()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel vm = new GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel()
            {
                FinishedGoodExpenditureNo = "no",
                Buyer = new BuyerViewModel
                {
                    Name = "bName",
                    Code = "bCode",
                    Id = 1
                },
                OtherDescription = "desc",
                ExpenditureDate = DateTimeOffset.Now,
                Description = "afafa",
                ExpenditureTo = "JUAL LOKAL",
                LocalSalesNoteNo = "LSNNo",
                Items = new List<GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel
                    {
                        Unit=new UnitViewModel
                        {
                            Id="1",
                            Code="code",
                            Name="name"
                        },
                        BasicPrice=10,
                        ExpenditureQuantity=10,
                        LeftoverComodity=new LeftoverComodityViewModel
                        {
                            Id=1,
                            Code="code",
                            Name="name"
                        },
                        RONo="ro"
                    }
                }
            };
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureFinishedGood>())).Returns(vm);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel vm = new GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel()
            {
                FinishedGoodExpenditureNo = "no",
                Buyer = new BuyerViewModel
                {
                    Name = "bName",
                    Code = "bCode",
                    Id = 1
                },
                OtherDescription = "desc",
                ExpenditureDate = DateTimeOffset.Now,
                Description = "afafa",
                ExpenditureTo = "UNIT",
                LocalSalesNoteNo = "LSNNo",
                Items = new List<GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel
                    {
                        Unit=new UnitViewModel
                        {
                            Id="1",
                            Code="code",
                            Name="name"
                        },
                        BasicPrice=10,
                        ExpenditureQuantity=10,
                        LeftoverComodity=new LeftoverComodityViewModel
                        {
                            Id=1,
                            Code="code",
                            Name="name"
                        },
                        RONo="ro"
                    }
                }
            };
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureFinishedGood>())).Returns(vm);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdf_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureFinishedGood>())).Returns(ViewModel);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_GetReport()
        {
            var mocks = GetMocks();

            mocks.Service.Setup(f => f.GetReport(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), 7))
                .Returns(new Tuple<List<ExpenditureFInishedGoodReportViewModel>, int>(new List<ExpenditureFInishedGoodReportViewModel>(), 1));


            var response = GetController(mocks).GetReportAll(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), "");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }
        [Fact]
        public void Should_Error_GetReport()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), 7))
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
