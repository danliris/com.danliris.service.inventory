using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.BalanceStock;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.Stock
{
    public class GarmentLeftoverWarehouseStockDataUtil
    {
        private readonly GarmentLeftoverWarehouseBalanceStockService Service;

        public GarmentLeftoverWarehouseStockDataUtil(GarmentLeftoverWarehouseBalanceStockService service)
        {
            Service = service;
        }

        public GarmentLeftoverWarehouseBalanceStock GetNewData(string type)
        {
            return new GarmentLeftoverWarehouseBalanceStock
            {
                BalanceStockDate = DateTimeOffset.Now,
                TypeOfGoods = type,
                
                Items = new List<GarmentLeftoverWarehouseBalanceStockItem>
                {
                    new GarmentLeftoverWarehouseBalanceStockItem
                    {
                        
                        UnitId = 1,
                        UnitCode = "Unit",
                        UnitName = "Unit",
                        PONo = "PONo",
                        Quantity = 1,
                        UomId = 1,
                        UomUnit = "Uom"
                    }
                }
            };
        }

        public async Task<GarmentLeftoverWarehouseBalanceStock> GetTestData(string type)
        {
            GarmentLeftoverWarehouseBalanceStock data = GetNewData(type);

            await Service.CreateAsync(data);

            return data;
        }

    }
}
