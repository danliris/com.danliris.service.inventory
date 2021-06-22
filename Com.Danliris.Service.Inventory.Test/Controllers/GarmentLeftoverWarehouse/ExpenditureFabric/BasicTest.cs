using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.ExpenditureFabric;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.GarmentLeftoverWarehouse.ExpenditureFabric
{
    public class BasicTest : BaseControllerTest<GarmentLeftoverWarehouseExpenditureFabricController, GarmentLeftoverWarehouseExpenditureFabric, GarmentLeftoverWarehouseExpenditureFabricViewModel, IGarmentLeftoverWarehouseExpenditureFabricService>
    {
        private GarmentLeftoverWarehouseExpenditureFabricViewModel vm = new GarmentLeftoverWarehouseExpenditureFabricViewModel()
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
            Items = new List<GarmentLeftoverWarehouseExpenditureFabricItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureFabricItemViewModel
                    {
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
                        PONo="apo",
                        BasicPrice=10
                    }
                }
        };

        private GarmentLeftoverWarehouseExpenditureFabricViewModel vmSAMPLE = new GarmentLeftoverWarehouseExpenditureFabricViewModel()
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
            Items = new List<GarmentLeftoverWarehouseExpenditureFabricItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureFabricItemViewModel
                    {
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

        private GarmentLeftoverWarehouseExpenditureFabricViewModel vmLOCAL = new GarmentLeftoverWarehouseExpenditureFabricViewModel()
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
            Items = new List<GarmentLeftoverWarehouseExpenditureFabricItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureFabricItemViewModel
                    {
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

        private GarmentLeftoverWarehouseExpenditureFabricViewModel vmETC = new GarmentLeftoverWarehouseExpenditureFabricViewModel()
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
            Items = new List<GarmentLeftoverWarehouseExpenditureFabricItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureFabricItemViewModel
                    {
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

        private List<GarmentProductViewModel> product = new List<GarmentProductViewModel>()
        {
            new GarmentProductViewModel
            {
                Code="code",
                Name="name",
                PONo="apo",
                Composition="comp",
                Const="const",
                Width="w",
                Yarn="y"
            }
        };

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Service.Setup(s => s.getProductForPDF(It.IsAny<GarmentLeftoverWarehouseExpenditureFabric>())).Returns(product);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureFabric>())).Returns(vm);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK_SAMPLE()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Service.Setup(s => s.getProductForPDF(It.IsAny<GarmentLeftoverWarehouseExpenditureFabric>())).Returns(product);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureFabric>())).Returns(vmSAMPLE);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK_LOCAL()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Service.Setup(s => s.getProductForPDF(It.IsAny<GarmentLeftoverWarehouseExpenditureFabric>())).Returns(product);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureFabric>())).Returns(vmLOCAL);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK_ETC()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Service.Setup(s => s.getProductForPDF(It.IsAny<GarmentLeftoverWarehouseExpenditureFabric>())).Returns(product);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureFabric>())).Returns(vmETC);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdf_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseExpenditureFabric>())).Returns(ViewModel);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
