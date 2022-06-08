using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalModels;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalDataUtils
{
    public class GarmentLeftoverWarehouseReceiptAvalDataUtil
    {
        private readonly GarmentLeftoverWarehouseReceiptAvalService Service;

        public GarmentLeftoverWarehouseReceiptAvalDataUtil(GarmentLeftoverWarehouseReceiptAvalService service)
        {
            Service = service;
        }

        public GarmentLeftoverWarehouseReceiptAval GetNewData()
        {
            return new GarmentLeftoverWarehouseReceiptAval
            {
                UnitFromId = 1,
                UnitFromCode = "Unit",
                UnitFromName = "Unit",
                AvalType="AVAL FABRIC",
                ReceiptDate = DateTimeOffset.Now,
                Remark = "Remark",
                TotalAval=10,
                Items = new List<GarmentLeftoverWarehouseReceiptAvalItem>
                {
                    new GarmentLeftoverWarehouseReceiptAvalItem
                    {
                        RONo = "ro",
                        ProductId = 1,
                        ProductCode = "Product",
                        ProductName = "Product",
                        ProductRemark = "Remark",
                        Quantity = 1,
                        UomId = 1,
                        UomUnit = "Uom"
                    },
                    new GarmentLeftoverWarehouseReceiptAvalItem
                    {
                        RONo = "ro1",
                        ProductId = 2,
                        ProductCode = "Product1",
                        ProductName = "Product1",
                        ProductRemark = "Remark",
                        Quantity = 2,
                        UomId = 1,
                        UomUnit = "Uom"
                    },
                    new GarmentLeftoverWarehouseReceiptAvalItem
                    {
                        RONo = "ro2",
                        ProductId = 3,
                        ProductCode = "Product2",
                        ProductName = "Product2",
                        ProductRemark = "Remark",
                        Quantity = 3,
                        UomId = 1,
                        UomUnit = "Uom"
                    }
                }
            };
        }

        public async Task<GarmentLeftoverWarehouseReceiptAval> GetTestData()
        {
            GarmentLeftoverWarehouseReceiptAval data = GetNewData();

            await Service.CreateAsync(data);

            return data;
        }

        public GarmentLeftoverWarehouseReceiptAval GetNewData1()
        {
            return new GarmentLeftoverWarehouseReceiptAval
            {
                UnitFromId = 1,
                UnitFromCode = "Unit",
                UnitFromName = "Unit",
                AvalType = "AVAL KOMPONEN",
                ReceiptDate = DateTimeOffset.Now,
                Remark = "Remark",
                TotalAval = 10,
                Items = new List<GarmentLeftoverWarehouseReceiptAvalItem>
                {
                    new GarmentLeftoverWarehouseReceiptAvalItem
                    {
                        RONo = "ro",
                        ProductId = 1,
                        ProductCode = "Product",
                        ProductName = "Product",
                        ProductRemark = "Remark",
                        Quantity = 1,
                        UomId = 1,
                        UomUnit = "Uom"
                    },
                    new GarmentLeftoverWarehouseReceiptAvalItem
                    {
                        RONo = "ro1",
                        ProductId = 2,
                        ProductCode = "Product1",
                        ProductName = "Product1",
                        ProductRemark = "Remark",
                        Quantity = 2,
                        UomId = 1,
                        UomUnit = "Uom"
                    },
                    new GarmentLeftoverWarehouseReceiptAvalItem
                    {
                        RONo = "ro2",
                        ProductId = 3,
                        ProductCode = "Product2",
                        ProductName = "Product2",
                        ProductRemark = "Remark",
                        Quantity = 3,
                        UomId = 1,
                        UomUnit = "Uom"
                    },
                }
            };
        }

        public async Task<GarmentLeftoverWarehouseReceiptAval> GetTestData1()
        {
            GarmentLeftoverWarehouseReceiptAval data = GetNewData1();

            await Service.CreateAsync(data);

            return data;
        }

        public async Task<GarmentLeftoverWarehouseReceiptAval> GetTestDataACC()
        {
            GarmentLeftoverWarehouseReceiptAval data = GetNewData();
            data.AvalType = "AVAL ACCESSORIES";
            await Service.CreateAsync(data);

            return data;
        }

        public async Task<GarmentLeftoverWarehouseReceiptAval> GetTestDataComponent()
        {
            GarmentLeftoverWarehouseReceiptAval data = GetNewData();
            data.AvalType = "AVAL KOMPONEN";
            await Service.CreateAsync(data);

            return data;
        }

        public async Task<GarmentLeftoverWarehouseReceiptAval> GetTestDataBP()
        {
            GarmentLeftoverWarehouseReceiptAval data = GetNewData();
            data.AvalType = "AVAL BAHAN PENOLONG";
            await Service.CreateAsync(data);

            return data;
        }

        public GarmentLeftoverWarehouseReceiptAval CopyData(GarmentLeftoverWarehouseReceiptAval oldData)
        {
            GarmentLeftoverWarehouseReceiptAval newData = new GarmentLeftoverWarehouseReceiptAval();

            PropertyCopier<GarmentLeftoverWarehouseReceiptAval, GarmentLeftoverWarehouseReceiptAval>.Copy(oldData, newData);

            newData.Items = new List<GarmentLeftoverWarehouseReceiptAvalItem>();
            foreach (var oldItem in oldData.Items)
            {
                GarmentLeftoverWarehouseReceiptAvalItem newItem = new GarmentLeftoverWarehouseReceiptAvalItem();

                PropertyCopier<GarmentLeftoverWarehouseReceiptAvalItem, GarmentLeftoverWarehouseReceiptAvalItem>.Copy(oldItem, newItem);

                newData.Items.Add(newItem);
            }

            return newData;
        }
    }
}
