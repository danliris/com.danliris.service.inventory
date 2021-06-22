using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalModels;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalViewModels;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalControllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalControllerTests
{
    public class BasicTest : BaseControllerTest<GarmentLeftoverWarehouseReceiptAvalController, GarmentLeftoverWarehouseReceiptAval, GarmentLeftoverWarehouseReceiptAvalViewModel, IGarmentLeftoverWarehouseReceiptAvalService>
    {
        private GarmentLeftoverWarehouseReceiptAvalViewModel vm = new GarmentLeftoverWarehouseReceiptAvalViewModel()
        {
            AvalReceiptNo = "no",
            ReceiptDate = DateTimeOffset.Now,
            AvalType = "AVAL FABRIC",
            Remark = "jhalh",
            UnitFrom = new UnitViewModel
            {
                Name = "bName",
                Code = "bCode",
                Id = "1"
            },
            TotalAval=10,
            Items = new List<GarmentLeftoverWarehouseReceiptAvalItemViewModel>
                {
                    new GarmentLeftoverWarehouseReceiptAvalItemViewModel
                    {
                        Product=new ProductViewModel
                        {
                            Id="1",
                            Name="product",
                            Code="product"
                        },
                        Quantity=10,
                        RONo="ro",
                        Uom=new UomViewModel
                        {
                            Unit="uom"
                        },
                        ProductRemark="apo",
                        Article="a",
                        AvalComponentNo="a",
                    }
                }
        };

        private GarmentLeftoverWarehouseReceiptAvalViewModel vmBP = new GarmentLeftoverWarehouseReceiptAvalViewModel()
        {
            AvalReceiptNo = "no",
            ReceiptDate = DateTimeOffset.Now,
            AvalType = "AVAL BAHAN PENOLONG",
            Remark = "jhalh",
            UnitFrom = new UnitViewModel
            {
                Name = "bName",
                Code = "bCode",
                Id = "1"
            },
            TotalAval = 10,
            Items = new List<GarmentLeftoverWarehouseReceiptAvalItemViewModel>
                {
                    new GarmentLeftoverWarehouseReceiptAvalItemViewModel
                    {
                        Product=new ProductViewModel
                        {
                            Id="1",
                            Name="product",
                            Code="product"
                        },
                        Quantity=10,
                        RONo="ro",
                        Uom=new UomViewModel
                        {
                            Unit="uom"
                        },
                        ProductRemark="apo",
                        Article="a",
                        AvalComponentNo="a",
                    }
                }
        };

        private GarmentLeftoverWarehouseReceiptAvalViewModel vmCOMPONENT = new GarmentLeftoverWarehouseReceiptAvalViewModel()
        {
            AvalReceiptNo = "no",
            ReceiptDate = DateTimeOffset.Now,
            AvalType = "AVAL KOMPONEN",
            Remark = "jhalh",
            UnitFrom = new UnitViewModel
            {
                Name = "bName",
                Code = "bCode",
                Id = "1"
            },
            TotalAval = 10,
            Items = new List<GarmentLeftoverWarehouseReceiptAvalItemViewModel>
                {
                    new GarmentLeftoverWarehouseReceiptAvalItemViewModel
                    {
                        Product=new ProductViewModel
                        {
                            Id="1",
                            Name="product",
                            Code="product"
                        },
                        Quantity=10,
                        RONo="ro",
                        Uom=new UomViewModel
                        {
                            Unit="uom"
                        },
                        ProductRemark="apo",
                        Article="a",
                        AvalComponentNo="a",
                    }
                }
        };

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseReceiptAval>())).Returns(vm);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK_BP()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseReceiptAval>())).Returns(vmBP);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK_COMPONENT()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseReceiptAval>())).Returns(vmCOMPONENT);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdf_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseReceiptAval>())).Returns(ViewModel);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
