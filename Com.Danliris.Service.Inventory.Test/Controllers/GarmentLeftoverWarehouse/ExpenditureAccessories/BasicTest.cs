using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.GarmentLeftoverWarehouse.ExpenditureAccessories
{
    public class BasicTest : BaseControllerTest<GarmentLeftoverWarehouseExpenditureAccessoriesController, GarmentLeftoverWarehouseExpenditureAccessories, GarmentLeftoverWarehouseExpenditureAccessoriesViewModel, IGarmentLeftoverWarehouseExpenditureAccessoriesService>
    {
        private GarmentLeftoverWarehouseExpenditureAccessoriesViewModel vm = new GarmentLeftoverWarehouseExpenditureAccessoriesViewModel()
        {
            ExpenditureNo = "no",
            Buyer = new BuyerViewModel
            {
                Name = "bName",
                Code = "bCode",
                Id = 1
            },
            ExpenditureDate = DateTimeOffset.Now,
            EtcRemark = "afafa",
            ExpenditureDestination = "UNIT",
            LocalSalesNoteNo = "LSNNo",
            Remark = "jhalh",
            UnitExpenditure = new UnitViewModel
            {
                Name = "bName",
                Code = "bCode",
                Id = "1"
            },
            Items = new List<GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel
                    {
                        Product=new ProductViewModel
                        {
                            Id="1",
                            Name="product",
                            Code="product"
                        },
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
                        },
                        PONo="apo"
                    }
                }
        };

        private GarmentLeftoverWarehouseExpenditureAccessoriesViewModel vmSAMPLE = new GarmentLeftoverWarehouseExpenditureAccessoriesViewModel()
        {
            ExpenditureNo = "no",
            Buyer = new BuyerViewModel
            {
                Name = "bName",
                Code = "bCode",
                Id = 1
            },
            ExpenditureDate = DateTimeOffset.Now,
            EtcRemark = "afafa",
            ExpenditureDestination = "SAMPLE",
            LocalSalesNoteNo = "LSNNo",
            Remark = "jhalh",
            UnitExpenditure = new UnitViewModel
            {
                Name = "bName",
                Code = "bCode",
                Id = "1"
            },
            Items = new List<GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel
                    {
                        Product=new ProductViewModel
                        {
                            Id="1",
                            Name="product",
                            Code="product"
                        },
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
                        },
                        PONo="apo"
                    }
                }
        };

        private GarmentLeftoverWarehouseExpenditureAccessoriesViewModel vmLOCAL = new GarmentLeftoverWarehouseExpenditureAccessoriesViewModel()
        {
            ExpenditureNo = "no",
            Buyer = new BuyerViewModel
            {
                Name = "bName",
                Code = "bCode",
                Id = 1
            },
            ExpenditureDate = DateTimeOffset.Now,
            EtcRemark = "afafa",
            ExpenditureDestination = "JUAL LOKAL",
            LocalSalesNoteNo = "LSNNo",
            Remark = "jhalh",
            UnitExpenditure = new UnitViewModel
            {
                Name = "bName",
                Code = "bCode",
                Id = "1"
            },
            Items = new List<GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel
                    {
                        Product=new ProductViewModel
                        {
                            Id="1",
                            Name="product",
                            Code="product"
                        },
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
                        },
                        PONo="apo"
                    }
                }
        };

        private GarmentLeftoverWarehouseExpenditureAccessoriesViewModel vmETC = new GarmentLeftoverWarehouseExpenditureAccessoriesViewModel()
        {
            ExpenditureNo = "no",
            Buyer = new BuyerViewModel
            {
                Name = "bName",
                Code = "bCode",
                Id = 1
            },
            ExpenditureDate = DateTimeOffset.Now,
            EtcRemark = "afafa",
            ExpenditureDestination = "DLL",
            LocalSalesNoteNo = "LSNNo",
            Remark = "jhalh",
            UnitExpenditure = new UnitViewModel
            {
                Name = "bName",
                Code = "bCode",
                Id = "1"
            },
            Items = new List<GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel
                    {
                        Product=new ProductViewModel
                        {
                            Id="1",
                            Name="product",
                            Code="product"
                        },
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
                        },
                        PONo="apo"
                    }
                }
        };

        private List<GarmentLeftoverWarehouseReceiptAccessoryItem> product = new List<GarmentLeftoverWarehouseReceiptAccessoryItem>()
        {
            new GarmentLeftoverWarehouseReceiptAccessoryItem
            {
                ProductId=1,
                POSerialNumber="apo",
                ProductRemark="remark"
            }
            
        };

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK_UNIT()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Service.Setup(s => s.getProductForPDF(It.IsAny<GarmentLeftoverWarehouseExpenditureAccessories>())).Returns(product);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureAccessories>())).Returns(vm);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK_SAMPLE()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Service.Setup(s => s.getProductForPDF(It.IsAny<GarmentLeftoverWarehouseExpenditureAccessories>())).Returns(product);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureAccessories>())).Returns(vmSAMPLE);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK_LOCAL()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Service.Setup(s => s.getProductForPDF(It.IsAny<GarmentLeftoverWarehouseExpenditureAccessories>())).Returns(product);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureAccessories>())).Returns(vmLOCAL);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK_ETC()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Service.Setup(s => s.getProductForPDF(It.IsAny<GarmentLeftoverWarehouseExpenditureAccessories>())).Returns(product);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureAccessories>())).Returns(vmETC);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdf_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureAccessories>())).Returns(ViewModel);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
