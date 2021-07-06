using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ReceiptAccessories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.Helpers;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ReceiptAccessories
{
    public class GarmentLeftoverWarehouseReceiptAccessoriesDataUtil
    {
        private readonly GarmentLeftoverWarehouseReceiptAccessoriesService Service;

        public GarmentLeftoverWarehouseReceiptAccessoriesDataUtil(GarmentLeftoverWarehouseReceiptAccessoriesService service)
        {
            Service = service;
        }

        public GarmentLeftoverWarehouseReceiptAccessory GetNewData()
        {
            return new GarmentLeftoverWarehouseReceiptAccessory
            {
                RequestUnitId = 1,
                RequestUnitCode = "Unit",
                RequestUnitName = "Unit",
                StorageReceiveDate = DateTimeOffset.Now,
                ExpenditureDate = DateTimeOffset.Now,
                Remark = "Remark",
                UENid = 1,
                UENNo = "UON No",
                InvoiceNoReceive = "InvoiceNo",
                StorageFromCode = "StorageFromno",
                StorageFromId = 1,
                StorageFromName = "StorageFromName",
                Items = new List<GarmentLeftoverWarehouseReceiptAccessoryItem>
                {
                    new GarmentLeftoverWarehouseReceiptAccessoryItem
                    {
                        POSerialNumber = "PO",
                        UomUnitId = 1,
                        ProductId = 1,
                        ProductCode = "Product",
                        ProductName = "Product",
                        ProductRemark = "Remark",
                        Quantity = 1,
                        UomUnit = "Uom"
                    }
                }
            };
        }
        public async Task<GarmentLeftoverWarehouseReceiptAccessory> GetTestData()
        {
            GarmentLeftoverWarehouseReceiptAccessory data = GetNewData();

            await Service.CreateAsync(data);

            return data;
        }
        public async Task<GarmentLeftoverWarehouseReceiptAccessory> GetTestDataACC()
        {
            GarmentLeftoverWarehouseReceiptAccessory data = GetNewData();
            await Service.CreateAsync(data);

            return data;
        }

        public GarmentLeftoverWarehouseReceiptAccessory CopyData(GarmentLeftoverWarehouseReceiptAccessory oldData)
        {
            GarmentLeftoverWarehouseReceiptAccessory newData = new GarmentLeftoverWarehouseReceiptAccessory();

            PropertyCopier<GarmentLeftoverWarehouseReceiptAccessory, GarmentLeftoverWarehouseReceiptAccessory>.Copy(oldData, newData);

            newData.Items = new List<GarmentLeftoverWarehouseReceiptAccessoryItem>();
            foreach (var oldItem in oldData.Items)
            {
                GarmentLeftoverWarehouseReceiptAccessoryItem newItem = new GarmentLeftoverWarehouseReceiptAccessoryItem();

                PropertyCopier<GarmentLeftoverWarehouseReceiptAccessoryItem, GarmentLeftoverWarehouseReceiptAccessoryItem>.Copy(oldItem, newItem);

                newData.Items.Add(newItem);
            }

            return newData;
        }
    }
}
