using Com.Danliris.Service.Inventory.Lib.Facades.InventoryFacades;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.InventoryDataUtils
{
    public class InventorySummaryDataUtil
    {
        private readonly InventorySummaryFacade facade;

        public InventorySummaryDataUtil(InventorySummaryFacade facade)
        {
            this.facade = facade;
        }

        public InventorySummary GetNewData()
        {
            return new InventorySummary
            {
                No = "No1",
                StorageCode = "test01",
                StorageId = 2,
                StorageName = "Test",
                ProductCode = "ProductCode1",
                ProductName = "ProductName1",
                UomUnit = "UomUnit1",
                StockPlanning = 1,
                Quantity = 1,
            };
        }

        public InventorySummaryViewModel GetNewDataViewModel()
        {
            return new InventorySummaryViewModel
            {
                code = "No1",
                storageCode = "test01",
                storageId = "2",
                storageName = "Test",
                productCode = "ProductCode1",
                productName = "ProductName1",
                quantity = 1,
                uom = "UomUnit1",
            };
        }

        public async Task<InventorySummary> GetTestData(string user)
        {
            InventorySummary invSum = GetNewData();

            await facade.Create(invSum, user);

            return invSum;
        }

        //public async Task<InventorySummary> GetTestDataOUT(string user)
        //{
        //    InventorySummary invMov = GetNewData();

        //    await facade.Create(invMov, user);

        //    return invMov;
        //}
    }
}
