using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.ExpenditureAval;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.GarmentLeftoverWarehouse.ExpenditureAval
{
    public class BasicTest : BaseControllerTest<GarmentLeftoverWarehouseExpenditureAvalController, GarmentLeftoverWarehouseExpenditureAval, GarmentLeftoverWarehouseExpenditureAvalViewModel, IGarmentLeftoverWarehouseExpenditureAvalService>
    {
        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK_FAB()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            GarmentLeftoverWarehouseExpenditureAvalViewModel vm = new GarmentLeftoverWarehouseExpenditureAvalViewModel()
            {
                AvalExpenditureNo="no",
                Buyer=new BuyerViewModel
                {
                    Name="bName",
                    Code="bCode",
                    Id=1
                },
                AvalType="AVAL FABRIC",
                ExpenditureDate=DateTimeOffset.Now,
                Description="afafa",
                ExpenditureTo="UNIT",
                LocalSalesNoteNo="LSNNo",
                Items= new List<GarmentLeftoverWarehouseExpenditureAvalItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureAvalItemViewModel
                    {
                        Product=new ProductViewModel
                        {
                            Id="1",
                            Name="product",
                            Code="product"
                        },
                        ActualQuantity=9,
                        AvalReceiptNo="avalNo",
                        Quantity=10,
                        Unit=new UnitViewModel
                        {
                            Id="1",
                            Code="code",
                            Name="name"
                        },
                        Uom=new UomViewModel
                        {
                            Unit="uom"
                        }
                    }
                }
            };
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureAval>())).Returns(vm);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK_PENOLONG()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            GarmentLeftoverWarehouseExpenditureAvalViewModel vm = new GarmentLeftoverWarehouseExpenditureAvalViewModel()
            {
                AvalExpenditureNo = "no",
                Buyer = new BuyerViewModel
                {
                    Name = "bName",
                    Code = "bCode",
                    Id = 1
                },
                AvalType = "AVAL BAHAN PENOLONG",
                ExpenditureDate = DateTimeOffset.Now,
                Description = "afafa",
                ExpenditureTo = "JUAL LOKAL",
                LocalSalesNoteNo = "LSNNo",
                Items = new List<GarmentLeftoverWarehouseExpenditureAvalItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureAvalItemViewModel
                    {
                        Product=new ProductViewModel
                        {
                            Id="1",
                            Name="product",
                            Code="product"
                        },
                        ActualQuantity=9,
                        AvalReceiptNo="avalNo",
                        Quantity=10,
                        Unit=new UnitViewModel
                        {
                            Id="1",
                            Code="code",
                            Name="name"
                        },
                        Uom=new UomViewModel
                        {
                            Unit="uom"
                        }
                    }
                }
            };
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureAval>())).Returns(vm);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK_COMPONENT()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            GarmentLeftoverWarehouseExpenditureAvalViewModel vm = new GarmentLeftoverWarehouseExpenditureAvalViewModel()
            {
                AvalExpenditureNo = "no",
                Buyer = new BuyerViewModel
                {
                    Name = "bName",
                    Code = "bCode",
                    Id = 1
                },
                AvalType = "AVAL KOMPONEN",
                ExpenditureDate = DateTimeOffset.Now,
                Description = "afafa",
                ExpenditureTo = "JUAL LOKAL",
                LocalSalesNoteNo = "LSNNo",
                Items = new List<GarmentLeftoverWarehouseExpenditureAvalItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureAvalItemViewModel
                    {
                        Product=new ProductViewModel
                        {
                            Id="1",
                            Name="product",
                            Code="product"
                        },
                        ActualQuantity=9,
                        AvalReceiptNo="avalNo",
                        Quantity=10,
                        Unit=new UnitViewModel
                        {
                            Id="1",
                            Code="code",
                            Name="name"
                        },
                        Uom=new UomViewModel
                        {
                            Unit="uom"
                        }
                    }
                }
            };
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureAval>())).Returns(vm);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdf_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureAval>())).Returns(ViewModel);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
