using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricModels;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricDataUtils
{
    public class GarmentLeftoverWarehouseReceiptFabricDataUtil
    {
        private readonly GarmentLeftoverWarehouseReceiptFabricService Service;

        public GarmentLeftoverWarehouseReceiptFabricDataUtil(GarmentLeftoverWarehouseReceiptFabricService service)
        {
            Service = service;
        }

        public GarmentLeftoverWarehouseReceiptFabric GetNewData()
        {
            return new GarmentLeftoverWarehouseReceiptFabric {
                UnitFromId = 1,
                UnitFromCode = "Unit",
                UnitFromName = "Unit",
                UENId = 1,
                UENNo = "UENNo",
                StorageFromId = 1,
                StorageFromCode = "Storage",
                StorageFromName = "Storage",
                ExpenditureDate = DateTimeOffset.Now,
                ReceiptDate = DateTimeOffset.Now,
                Remark = "Remark",
                Items = new List<GarmentLeftoverWarehouseReceiptFabricItem>
                {
                    new GarmentLeftoverWarehouseReceiptFabricItem
                    {
                        UENItemId = 1,
                        ProductId = 1, 
                        POSerialNumber="PONo",
                        ProductCode = "Product",
                        ProductName = "Product",
                        ProductRemark = "Remark",
                        FabricRemark = "FabricRemark",
                        Composition = "Composition",
                        BasicPrice = 1,
                        Quantity = 1,
                        UomId = 1,
                        UomUnit = "Uom"
                    }
                }
            };
        }

        public async Task<GarmentLeftoverWarehouseReceiptFabric> GetTestData()
        {
            GarmentLeftoverWarehouseReceiptFabric data = GetNewData();

            await Service.CreateAsync(data);

            return data;
        }

        public GarmentLeftoverWarehouseReceiptFabric CopyData(GarmentLeftoverWarehouseReceiptFabric oldData)
        {
            GarmentLeftoverWarehouseReceiptFabric newData = new GarmentLeftoverWarehouseReceiptFabric();

            PropertyCopier<GarmentLeftoverWarehouseReceiptFabric, GarmentLeftoverWarehouseReceiptFabric>.Copy(oldData, newData);

            newData.Items = new List<GarmentLeftoverWarehouseReceiptFabricItem>();
            foreach (var oldItem in oldData.Items)
            {
                GarmentLeftoverWarehouseReceiptFabricItem newItem = new GarmentLeftoverWarehouseReceiptFabricItem();

                PropertyCopier<GarmentLeftoverWarehouseReceiptFabricItem, GarmentLeftoverWarehouseReceiptFabricItem>.Copy(oldItem, newItem);

                newData.Items.Add(newItem);
            }

            return newData;
        }
    }
}
