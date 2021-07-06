using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.BalanceStock;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.BalanceStock
{
    public class GarmentLeftoverWarehouseBalanceStockDataUtil
    {
        private readonly GarmentLeftoverWarehouseBalanceStockService Service;

        public GarmentLeftoverWarehouseBalanceStockDataUtil(GarmentLeftoverWarehouseBalanceStockService service)
        {
            Service = service;
        }

        public GarmentLeftoverWarehouseBalanceStock GetNewData_FINISHEDGOOD()
        {
            return new GarmentLeftoverWarehouseBalanceStock
            {
                BalanceStockDate = DateTimeOffset.Now,
                TypeOfGoods = "BARANG JADI",
                Items = new List<GarmentLeftoverWarehouseBalanceStockItem>
                {
                    new GarmentLeftoverWarehouseBalanceStockItem
                    {
                        BasicPrice = 1,
                        UnitId = 1,
                        UnitCode = "Unit",
                        UnitName = "Unit",
                        PONo = "PONo",
                        Quantity = 1,
                        UomId = 1,
                        UomUnit = "Uom",
                        LeftoverComodityCode="Como",
                        LeftoverComodityId=1,
                        LeftoverComodityName="ComoName",
                        ProductCode="product",
                        ProductId=1,
                        ProductName="prName",
                        RONo="ro",
                        Composition="asa",
                        Construction="asf",
                        ProductRemark="asfa",

                    }
                }
            };
        }

        public GarmentLeftoverWarehouseBalanceStock GetNewData_FABRIC()
        {
            return new GarmentLeftoverWarehouseBalanceStock
            {
                BalanceStockDate = DateTimeOffset.Now,
                TypeOfGoods = "FABRIC",
                Items = new List<GarmentLeftoverWarehouseBalanceStockItem>
                {
                    new GarmentLeftoverWarehouseBalanceStockItem
                    {
                        BasicPrice = 1,
                        UnitId = 1,
                        UnitCode = "Unit",
                        UnitName = "Unit",
                        PONo = "PONo",
                        Quantity = 1,
                        UomId = 1,
                        UomUnit = "Uom",
                        LeftoverComodityCode="Como",
                        LeftoverComodityId=1,
                        LeftoverComodityName="ComoName",
                        ProductCode="product",
                        ProductId=1,
                        ProductName="prName",
                        RONo="ro",
                        Composition="asa",
                        Construction="asf",
                        ProductRemark="asfa",

                    }
                }
            };
        }
        public GarmentLeftoverWarehouseBalanceStock GetNewData_ACC()
        {
            return new GarmentLeftoverWarehouseBalanceStock
            {
                BalanceStockDate = DateTimeOffset.Now,
                TypeOfGoods = "ACCESSORIES",
                Items = new List<GarmentLeftoverWarehouseBalanceStockItem>
                {
                    new GarmentLeftoverWarehouseBalanceStockItem
                    {
                        BasicPrice = 1,
                        UnitId = 1,
                        UnitCode = "Unit",
                        UnitName = "Unit",
                        PONo = "PONo",
                        Quantity = 1,
                        UomId = 1,
                        UomUnit = "Uom",
                        LeftoverComodityCode="Como",
                        LeftoverComodityId=1,
                        LeftoverComodityName="ComoName",
                        ProductCode="product",
                        ProductId=1,
                        ProductName="prName",
                        RONo="ro",
                        Composition="asa",
                        Construction="asf",
                        ProductRemark="asfa",

                    }
                }
            };
        }

        public async Task<GarmentLeftoverWarehouseBalanceStock> GetTestData_FINISHEDGOOD()
        {
            GarmentLeftoverWarehouseBalanceStock data = GetNewData_FINISHEDGOOD();

            await Service.CreateAsync(data);

            return data;
        }

        public async Task<GarmentLeftoverWarehouseBalanceStock> GetTestData_FABRIC()
        {
            GarmentLeftoverWarehouseBalanceStock data = GetNewData_FABRIC();

            await Service.CreateAsync(data);

            return data;
        }

        public async Task<GarmentLeftoverWarehouseBalanceStock> GetTestData_ACC()
        {
            GarmentLeftoverWarehouseBalanceStock data = GetNewData_ACC();

            await Service.CreateAsync(data);

            return data;
        }
        public GarmentLeftoverWarehouseBalanceStock CopyData(GarmentLeftoverWarehouseBalanceStock oldData)
        {
            GarmentLeftoverWarehouseBalanceStock newData = new GarmentLeftoverWarehouseBalanceStock();

            PropertyCopier<GarmentLeftoverWarehouseBalanceStock, GarmentLeftoverWarehouseBalanceStock>.Copy(oldData, newData);

            newData.Items = new List<GarmentLeftoverWarehouseBalanceStockItem>();
            foreach (var oldItem in oldData.Items)
            {
                GarmentLeftoverWarehouseBalanceStockItem newItem = new GarmentLeftoverWarehouseBalanceStockItem();

                PropertyCopier<GarmentLeftoverWarehouseBalanceStockItem, GarmentLeftoverWarehouseBalanceStockItem>.Copy(oldItem, newItem);

                newData.Items.Add(newItem);
            }

            return newData;
        }
    }
}
