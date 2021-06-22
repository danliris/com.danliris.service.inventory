using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricModels;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricViewModels;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricControllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricControllerTests
{
    public class BasicTest: BaseControllerTest<GarmentLeftoverWarehouseReceiptFabricController, GarmentLeftoverWarehouseReceiptFabric, GarmentLeftoverWarehouseReceiptFabricViewModel, IGarmentLeftoverWarehouseReceiptFabricService>
    {
        private GarmentLeftoverWarehouseReceiptFabricViewModel vm = new GarmentLeftoverWarehouseReceiptFabricViewModel()
        {
            ReceiptNoteNo = "no",
            ReceiptDate = DateTimeOffset.Now,
            UENNo = "no",
            Remark = "jhalh",
            UnitFrom = new UnitViewModel
            {
                Name = "bName",
                Code = "bCode",
                Id = "1"
            },
            Items = new List<GarmentLeftoverWarehouseReceiptFabricItemViewModel>
                {
                    new GarmentLeftoverWarehouseReceiptFabricItemViewModel
                    {
                        Product=new ProductViewModel
                        {
                            Id="1",
                            Name="product",
                            Code="product"
                        },
                        Quantity=10,
                        Uom=new UomViewModel
                        {
                            Unit="uom"
                        },
                        POSerialNumber="apo",
                        Composition="a",
                        FabricRemark="a",
                    }
                }
        };
        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK_COMPONENT()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseReceiptFabric>())).Returns(vm);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdf_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseReceiptFabric>())).Returns(ViewModel);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
