using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Stock;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.ViewModels.GarmentLeftoverWarehouse.Stock
{
    public class GarmentLeftoverWarehouseStockViewModelTest
    {
        [Fact]
        public void should_succes_instantiate()
        {
            GarmentLeftoverWarehouseStockViewModel viewModel = new GarmentLeftoverWarehouseStockViewModel()
            {
                Histories = new List<GarmentLeftoverWarehouseStockHistoryViewModel>(),
                PONo = "PONo",
                Product = new Lib.ViewModels.ProductViewModel(),
                Quantity = 1,
                ReferenceType = new Lib.Enums.GarmentLeftoverWarehouseStockReferenceTypeEnum(),
                RONo = "RONo",
                Unit = new Lib.ViewModels.UnitViewModel(),
                Uom = new Lib.ViewModels.UomViewModel(),
                LeftoverComodity= new Lib.ViewModels.LeftoverComodityViewModel(),
                BasicPrice=1
            };

            Assert.NotNull(viewModel.Histories);
            Assert.Equal("PONo", viewModel.PONo);
            Assert.NotNull(viewModel.Product);
            Assert.Equal(1, viewModel.Quantity);
            var temp = viewModel.ReferenceType.ToString(); 
            Assert.Equal("FABRIC", viewModel.ReferenceType.ToString());
            Assert.Equal("RONo", viewModel.RONo);
            Assert.NotNull(viewModel.Unit);
            Assert.NotNull(viewModel.Uom);
            Assert.NotNull(viewModel.LeftoverComodity);
            Assert.Equal(1, viewModel.BasicPrice);

        }
    }
}
