using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.BalanceStock;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.BalanceStock
{
    public class GarmentLeftoverWarehouseBalanceStockService : IGarmentLeftoverWarehouseBalanceStockService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseBalanceStockService";

        private InventoryDbContext DbContext;
        private DbSet<GarmentLeftoverWarehouseBalanceStock> DbSet;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        private readonly IGarmentLeftoverWarehouseStockService StockService;

        public GarmentLeftoverWarehouseBalanceStockService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentLeftoverWarehouseBalanceStock>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));

            StockService = (IGarmentLeftoverWarehouseStockService)serviceProvider.GetService(typeof(IGarmentLeftoverWarehouseStockService));
        }

        public async Task<int> CreateAsync(GarmentLeftoverWarehouseBalanceStock model)
        {
            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    int Created = 0;

                    model.FlagForCreate(IdentityService.Username, UserAgent);
                    model.FlagForUpdate(IdentityService.Username, UserAgent);

                    foreach (var item in model.Items)
                    {
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                    }
                    DbSet.Add(model);
                    Created = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        GarmentLeftoverWarehouseStock stock = GenerateStock(model.TypeOfGoods, item);
                        await StockService.StockIn(stock, "BALANCE STOCK", model.Id, item.Id);
                    }

                    transaction.Commit();

                    return Created;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public GarmentLeftoverWarehouseBalanceStock MapToModel(GarmentLeftoverWarehouseBalanceStockViewModel viewModel)
        {
            GarmentLeftoverWarehouseBalanceStock model = new GarmentLeftoverWarehouseBalanceStock();
            PropertyCopier<GarmentLeftoverWarehouseBalanceStockViewModel, GarmentLeftoverWarehouseBalanceStock>.Copy(viewModel, model);


            model.Items = new List<GarmentLeftoverWarehouseBalanceStockItem>();
            foreach (var viewModelItem in viewModel.Items)
            {
                GarmentLeftoverWarehouseBalanceStockItem modelItem = new GarmentLeftoverWarehouseBalanceStockItem();
                PropertyCopier<GarmentLeftoverWarehouseBalanceStockItemViewModel, GarmentLeftoverWarehouseBalanceStockItem>.Copy(viewModelItem, modelItem);

                if (viewModelItem.Unit != null)
                {
                    modelItem.UnitId = long.Parse(viewModelItem.Unit.Id);
                    modelItem.UnitCode = viewModelItem.Unit.Code;
                    modelItem.UnitName = viewModelItem.Unit.Name;
                }

                if (viewModelItem.Product != null)
                {
                    modelItem.ProductId = long.Parse(viewModelItem.Product.Id);
                    modelItem.ProductName = viewModelItem.Product.Name;
                    modelItem.ProductCode = viewModelItem.Product.Code;
                }

                if (viewModelItem.Uom != null)
                {
                    modelItem.UomId = long.Parse(viewModelItem.Uom.Id);
                    modelItem.UomUnit = viewModelItem.Uom.Unit;
                }

                if (viewModelItem.LeftoverComodity != null)
                {
                    modelItem.LeftoverComodityCode = viewModelItem.LeftoverComodity.Code;
                    modelItem.LeftoverComodityId = viewModelItem.LeftoverComodity.Id;
                    modelItem.LeftoverComodityName = viewModelItem.LeftoverComodity.Name;
                }
                model.Items.Add(modelItem);
            }

            return model;
        }

        public GarmentLeftoverWarehouseBalanceStockViewModel MapToViewModel(GarmentLeftoverWarehouseBalanceStock model)
        {
            GarmentLeftoverWarehouseBalanceStockViewModel viewModel = new GarmentLeftoverWarehouseBalanceStockViewModel();
            PropertyCopier<GarmentLeftoverWarehouseBalanceStock, GarmentLeftoverWarehouseBalanceStockViewModel>.Copy(model, viewModel);

            if (model.Items != null)
            {
                viewModel.Items = new List<GarmentLeftoverWarehouseBalanceStockItemViewModel>();
                foreach (var modelItem in model.Items)
                {
                    GarmentLeftoverWarehouseBalanceStockItemViewModel viewModelItem = new GarmentLeftoverWarehouseBalanceStockItemViewModel();
                    PropertyCopier<GarmentLeftoverWarehouseBalanceStockItem, GarmentLeftoverWarehouseBalanceStockItemViewModel>.Copy(modelItem, viewModelItem);

                    viewModelItem.Unit = new UnitViewModel
                    {
                        Id = modelItem.UnitId.ToString(),
                        Code = modelItem.UnitCode,
                        Name = modelItem.UnitName
                    };

                    viewModelItem.Uom = new UomViewModel
                    {
                        Id = modelItem.UomId.ToString(),
                        Unit = modelItem.UomUnit
                    };

                    viewModelItem.LeftoverComodity = new LeftoverComodityViewModel
                    {
                        Id = modelItem.LeftoverComodityId,
                        Code = modelItem.LeftoverComodityCode,
                        Name = modelItem.LeftoverComodityName
                    };

                    viewModelItem.Product = new ProductViewModel
                    {
                        Id = modelItem.ProductId.ToString(),
                        Code = modelItem.ProductCode,
                        Name = modelItem.ProductName
                    };

                    viewModel.Items.Add(viewModelItem);
                }
            }

            return viewModel;
        }

        public ReadResponse<GarmentLeftoverWarehouseBalanceStock> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentLeftoverWarehouseBalanceStock> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "TypeOfGoods", "_CreatedBy"
            };
            Query = QueryHelper<GarmentLeftoverWarehouseBalanceStock>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentLeftoverWarehouseBalanceStock>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentLeftoverWarehouseBalanceStock>.Order(Query, OrderDictionary);

            List<string> SelectedFields = (select != null && select.Count > 0) ? select : new List<string>()
            {
                "Id", "TypeOfGoods", "BalanceStockDate", "_CreatedBy"
            };

            Query = Query.Select(s => new GarmentLeftoverWarehouseBalanceStock
            {
                Id = s.Id,
                TypeOfGoods = s.TypeOfGoods,
                BalanceStockDate = s.BalanceStockDate,
                _CreatedBy = s._CreatedBy,
            });

            Pageable<GarmentLeftoverWarehouseBalanceStock> pageable = new Pageable<GarmentLeftoverWarehouseBalanceStock>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseBalanceStock> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<GarmentLeftoverWarehouseBalanceStock>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<GarmentLeftoverWarehouseBalanceStock> ReadByIdAsync(int id)
        {
            return await DbSet
                .Where(w => w.Id == id)
                .Include(i => i.Items)
                .FirstOrDefaultAsync();
        }

        public Task<int> UpdateAsync(int id, GarmentLeftoverWarehouseBalanceStock model)
        {
            throw new NotImplementedException();
        }

        private GarmentLeftoverWarehouseStock GenerateStock(string typeOfGoods, GarmentLeftoverWarehouseBalanceStockItem item)
        {
            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock();
            var referenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FABRIC;

            if (typeOfGoods == "ACCESSORIES")
            {
                referenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.ACCESSORIES;
            } else if (typeOfGoods == "BARANG JADI")
            {
                referenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FINISHED_GOOD;
            }

            if(typeOfGoods!="BARANG JADI")
            {
                stock = new GarmentLeftoverWarehouseStock
                {
                    ReferenceType = referenceType,
                    UnitId = item.UnitId,
                    UnitCode = item.UnitCode,
                    UnitName = item.UnitName,
                    PONo = item.PONo,
                    UomId = item.UomId,
                    UomUnit = item.UomUnit,
                    Quantity = item.Quantity,
                    RONo = item.RONo,
                    LeftoverComodityCode = item.LeftoverComodityCode,
                    LeftoverComodityId = item.LeftoverComodityId,
                    LeftoverComodityName = item.LeftoverComodityName,
                    ProductCode = item.ProductCode,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    BasicPrice = item.BasicPrice
                };
            }
            else
            {
                stock = new GarmentLeftoverWarehouseStock
                {
                    ReferenceType = referenceType,
                    UnitId = item.UnitId,
                    UnitCode = item.UnitCode,
                    UnitName = item.UnitName,
                    PONo = item.PONo,
                    Quantity = item.Quantity,
                    RONo = item.RONo,
                    LeftoverComodityCode = item.LeftoverComodityCode,
                    LeftoverComodityId = item.LeftoverComodityId,
                    LeftoverComodityName = item.LeftoverComodityName,
                    BasicPrice = item.BasicPrice
                };
            }

            return stock;
        }
    }
}
