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
            return new GarmentLeftoverWarehouseReceiptFabric();
        }

        public async Task<GarmentLeftoverWarehouseReceiptFabric> GetTestData()
        {
            GarmentLeftoverWarehouseReceiptFabric data = GetNewData();

            await Service.CreateAsync(data);

            return data;
        }
    }
}
