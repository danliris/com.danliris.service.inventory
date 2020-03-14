using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricViewModels;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricServices
{
    public class GarmentLeftoverWarehouseReceiptFabricService : IGarmentLeftoverWarehouseReceiptFabricService
    {
        public Task<int> CreateAsync(GarmentLeftoverWarehouseReceiptFabric model)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public GarmentLeftoverWarehouseReceiptFabric MapToModel(GarmentLeftoverWarehouseReceiptFabricViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        public GarmentLeftoverWarehouseReceiptFabricViewModel MapToViewModel(GarmentLeftoverWarehouseReceiptFabric model)
        {
            throw new NotImplementedException();
        }

        public ReadResponse<GarmentLeftoverWarehouseReceiptFabric> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            throw new NotImplementedException();
        }

        public Task<GarmentLeftoverWarehouseReceiptFabric> ReadByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(int id, GarmentLeftoverWarehouseReceiptFabric model)
        {
            throw new NotImplementedException();
        }
    }
}
