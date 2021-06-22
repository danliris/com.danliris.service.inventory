using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.ReceiptAccessories;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.GarmentLeftoverWarehouse.ReceiptAccessories
{
    public class BasicTest: BaseControllerTest<GarmentLeftoverWarehouseReceiptAccessoriesController, GarmentLeftoverWarehouseReceiptAccessory, GarmentLeftoverWarehouseReceiptAccessoriesViewModel, IGarmentLeftoverWarehouseReceiptAccessoriesService>
    {
        private GarmentLeftoverWarehouseReceiptAccessoriesViewModel vm = new GarmentLeftoverWarehouseReceiptAccessoriesViewModel()
        {
            InvoiceNoReceive = "no",
            ExpenditureDate = DateTimeOffset.Now,
            UENNo = "afafa",
            Remark = "jhalh",
            RequestUnit = new UnitViewModel
            {
                Name = "bName",
                Code = "bCode",
                Id = "1"
            },
            Items = new List<GarmentLeftoverWarehouseReceiptAccessoriesItemViewModel>
                {
                    new GarmentLeftoverWarehouseReceiptAccessoriesItemViewModel
                    {
                        Product=new ProductViewModel
                        {
                            Id="1",
                            Name="product",
                            Code="product"
                        },
                        Quantity=10,
                        ROJob="ro",
                        Uom=new UomViewModel
                        {
                            Unit="uom"
                        },
                        POSerialNumber="apo"
                    }
                }
        };

        [Fact]
        public async void GetPdfTest_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseReceiptAccessory>())).Returns(vm);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.NotNull(response);
        }

        [Fact]
        public async void GetPdf_WithException_InternalServer()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            mocks.Service.Setup(f => f.MapToViewModel(It.IsAny<GarmentLeftoverWarehouseReceiptAccessory>())).Returns(ViewModel);

            var response = await GetController(mocks).GetPdfById(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
