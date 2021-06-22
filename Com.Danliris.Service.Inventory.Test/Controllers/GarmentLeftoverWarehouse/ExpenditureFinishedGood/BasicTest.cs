using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseExpenditureFinishedGoodControllers;
using Moq;
using System;
using System.Collections.Generic;
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
    }
}
