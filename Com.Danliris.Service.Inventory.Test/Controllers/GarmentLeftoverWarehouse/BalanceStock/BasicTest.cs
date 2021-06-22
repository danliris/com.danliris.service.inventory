using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.BalanceStock;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Test.Controllers.GarmentLeftoverWarehouse.BalanceStock
{
    public class BasicTest : BaseControllerTest<GarmentLeftoverWarehouseBalanceStockController, GarmentLeftoverWarehouseBalanceStock, GarmentLeftoverWarehouseBalanceStockViewModel, IGarmentLeftoverWarehouseBalanceStockService>
    {
    }
}
