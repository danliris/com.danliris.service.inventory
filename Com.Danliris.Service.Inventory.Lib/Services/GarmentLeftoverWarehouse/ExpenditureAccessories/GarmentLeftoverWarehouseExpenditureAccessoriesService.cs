using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAccessories
{
    public class GarmentLeftoverWarehouseExpenditureAccessoriesService : IGarmentLeftoverWarehouseExpenditureAccessoriesService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseExpenditureAccessoriesService";

        private InventoryDbContext DbContext;
        private DbSet<GarmentLeftoverWarehouseExpenditureAccessories> DbSet;
        private DbSet<GarmentLeftoverWarehouseExpenditureAccessoriesItem> DbSetItem;

        private readonly IGarmentLeftoverWarehouseStockService StockService;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        public GarmentLeftoverWarehouseExpenditureAccessoriesService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentLeftoverWarehouseExpenditureAccessories>();
            DbSetItem = DbContext.Set<GarmentLeftoverWarehouseExpenditureAccessoriesItem>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));

            StockService = (IGarmentLeftoverWarehouseStockService)serviceProvider.GetService(typeof(IGarmentLeftoverWarehouseStockService));
        }

        public GarmentLeftoverWarehouseExpenditureAccessories MapToModel(GarmentLeftoverWarehouseExpenditureAccessoriesViewModel viewModel)
        {
            GarmentLeftoverWarehouseExpenditureAccessories model = new GarmentLeftoverWarehouseExpenditureAccessories();
            PropertyCopier<GarmentLeftoverWarehouseExpenditureAccessoriesViewModel, GarmentLeftoverWarehouseExpenditureAccessories>.Copy(viewModel, model);

            if (viewModel.UnitExpenditure != null)
            {
                model.UnitExpenditureId = long.Parse(viewModel.UnitExpenditure.Id);
                model.UnitExpenditureCode = viewModel.UnitExpenditure.Code;
                model.UnitExpenditureName = viewModel.UnitExpenditure.Name;
            }

            if (viewModel.Buyer != null)
            {
                model.BuyerId = viewModel.Buyer.Id;
                model.BuyerCode = viewModel.Buyer.Code;
                model.BuyerName = viewModel.Buyer.Name;
            }

            model.Items = new List<GarmentLeftoverWarehouseExpenditureAccessoriesItem>();
            foreach (var viewModelItem in viewModel.Items)
            {
                GarmentLeftoverWarehouseExpenditureAccessoriesItem modelItem = new GarmentLeftoverWarehouseExpenditureAccessoriesItem();
                PropertyCopier<GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel, GarmentLeftoverWarehouseExpenditureAccessoriesItem>.Copy(viewModelItem, modelItem);

                if (viewModelItem.Unit != null)
                {
                    modelItem.UnitId = long.Parse(viewModelItem.Unit.Id);
                    modelItem.UnitCode = viewModelItem.Unit.Code;
                    modelItem.UnitName = viewModelItem.Unit.Name;
                }

                if (viewModelItem.Uom != null)
                {
                    modelItem.UomId = long.Parse(viewModelItem.Uom.Id);
                    modelItem.UomUnit = viewModelItem.Uom.Unit;
                }

                if (viewModelItem.Product != null)
                {
                    modelItem.ProductId = long.Parse(viewModelItem.Product.Id);
                    modelItem.ProductCode = viewModelItem.Product.Code;
                    modelItem.ProductName = viewModelItem.Product.Name;
                }

                model.Items.Add(modelItem);
            }

            return model;
        }

        public GarmentLeftoverWarehouseExpenditureAccessoriesViewModel MapToViewModel(GarmentLeftoverWarehouseExpenditureAccessories model)
        {
            GarmentLeftoverWarehouseExpenditureAccessoriesViewModel viewModel = new GarmentLeftoverWarehouseExpenditureAccessoriesViewModel();
            PropertyCopier<GarmentLeftoverWarehouseExpenditureAccessories, GarmentLeftoverWarehouseExpenditureAccessoriesViewModel>.Copy(model, viewModel);

            viewModel.UnitExpenditure = new UnitViewModel
            {
                Id = model.UnitExpenditureId.ToString(),
                Code = model.UnitExpenditureCode,
                Name = model.UnitExpenditureName
            };

            viewModel.Buyer = new BuyerViewModel
            {
                Id = model.BuyerId,
                Code = model.BuyerCode,
                Name = model.BuyerName
            };

            if (model.Items != null)
            {
                viewModel.Items = new List<GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel>();
                foreach (var modelItem in model.Items)
                {
                    GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel viewModelItem = new GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel();
                    PropertyCopier<GarmentLeftoverWarehouseExpenditureAccessoriesItem, GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel>.Copy(modelItem, viewModelItem);

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

        public async Task<int> CreateAsync(GarmentLeftoverWarehouseExpenditureAccessories model)
        {
            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    int Created = 0;

                    model.FlagForCreate(IdentityService.Username, UserAgent);
                    model.FlagForUpdate(IdentityService.Username, UserAgent);

                    model.ExpenditureNo = GenerateNo(model);

                    foreach (var item in model.Items)
                    {
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                    }
                    DbSet.Add(model);
                    Created = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        GarmentLeftoverWarehouseStock stock = GenerateStock(item);
                        await StockService.StockOut(stock, model.ExpenditureNo, model.Id, item.Id);
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

        public ReadResponse<GarmentLeftoverWarehouseExpenditureAccessories> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentLeftoverWarehouseExpenditureAccessories> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "ExpenditureNo", "ExpenditureDestination", "UnitExpenditureName", "BuyerName", "EtcRemark"
            };
            Query = QueryHelper<GarmentLeftoverWarehouseExpenditureAccessories>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentLeftoverWarehouseExpenditureAccessories>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentLeftoverWarehouseExpenditureAccessories>.Order(Query, OrderDictionary);

            List<string> SelectedFields = (select != null && select.Count > 0) ? select : new List<string>()
            {
                "Id", "ExpenditureNo", "ExpenditureDate", "ExpenditureDestination", "UnitExpenditure", "Buyer", "EtcRemark"
            };

            Query = Query.Select(s => new GarmentLeftoverWarehouseExpenditureAccessories
            {
                Id = s.Id,
                ExpenditureNo = s.ExpenditureNo,
                ExpenditureDate = s.ExpenditureDate,
                ExpenditureDestination = s.ExpenditureDestination,
                UnitExpenditureId = s.UnitExpenditureId,
                UnitExpenditureCode = s.UnitExpenditureCode,
                UnitExpenditureName = s.UnitExpenditureName,
                BuyerId = s.BuyerId,
                BuyerCode = s.BuyerCode,
                BuyerName = s.BuyerName,
                EtcRemark = s.EtcRemark
            });

            Pageable<GarmentLeftoverWarehouseExpenditureAccessories> pageable = new Pageable<GarmentLeftoverWarehouseExpenditureAccessories>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseExpenditureAccessories> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<GarmentLeftoverWarehouseExpenditureAccessories>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<GarmentLeftoverWarehouseExpenditureAccessories> ReadByIdAsync(int id)
        {
            return await DbSet
                .Where(w => w.Id == id)
                .Include(i => i.Items)
                .FirstOrDefaultAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    int Deleted = 0;

                    GarmentLeftoverWarehouseExpenditureAccessories model = await ReadByIdAsync(id);
                    model.FlagForDelete(IdentityService.Username, UserAgent);
                    foreach (var item in model.Items)
                    {
                        item.FlagForDelete(IdentityService.Username, UserAgent);
                    }

                    Deleted = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        GarmentLeftoverWarehouseStock stock = GenerateStock(item);
                        await StockService.StockIn(stock, model.ExpenditureNo, model.Id, item.Id);
                    }

                    transaction.Commit();

                    return Deleted;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public async Task<int> UpdateAsync(int id, GarmentLeftoverWarehouseExpenditureAccessories model)
        {
            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    int Updated = 0;

                    GarmentLeftoverWarehouseExpenditureAccessories existingModel = await ReadByIdAsync(id);
                    if (existingModel.ExpenditureDate != model.ExpenditureDate)
                    {
                        existingModel.ExpenditureDate = model.ExpenditureDate;
                    }
                    if (existingModel.Remark != model.Remark)
                    {
                        existingModel.Remark = model.Remark;
                    }
                    if (existingModel.IsUsed != model.IsUsed)
                    {
                        existingModel.IsUsed = model.IsUsed;
                    }
                    existingModel.FlagForUpdate(IdentityService.Username, UserAgent);

                    foreach (var existingItem in existingModel.Items)
                    {
                        GarmentLeftoverWarehouseStock stockIn = GenerateStock(existingItem);
                        await StockService.StockIn(stockIn, model.ExpenditureNo, model.Id, existingItem.Id);
                    }

                    foreach (var existingItem in existingModel.Items)
                    {
                        var item = model.Items.FirstOrDefault(i => i.Id == existingItem.Id);
                        if (item == null)
                        {
                            existingItem.FlagForDelete(IdentityService.Username, UserAgent);
                        }
                        else
                        {
                            if (existingItem.Quantity != item.Quantity)
                            {
                                existingItem.Quantity = item.Quantity;
                            }
                            existingItem.FlagForUpdate(IdentityService.Username, UserAgent);
                        }
                    }

                    foreach (var item in model.Items.Where(i => i.Id == 0))
                    {
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                        existingModel.Items.Add(item);
                    }

                    Updated = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        GarmentLeftoverWarehouseStock stock = GenerateStock(item);
                        await StockService.StockOut(stock, model.ExpenditureNo, model.Id, item.Id);
                    }

                    transaction.Commit();

                    return Updated;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public double CheckStockQuantity(int Id, int StockId)
        {
            var stock = StockService.ReadById(StockId);
            var stockQuantity = stock.Quantity;

            if (Id > 0)
            {
                var existingQuantity = DbSetItem.Where(i => i.ExpenditureId == Id && i.StockId == StockId).Select(s => s.Quantity).FirstOrDefault();
                stockQuantity += existingQuantity;
            }

            return stockQuantity;
        }

        private string GenerateNo(GarmentLeftoverWarehouseExpenditureAccessories model)
        {
            string prefix = $"BKACC{model._CreatedUtc.ToString("yy")}{model._CreatedUtc.ToString("MM")}";

            var lastNo = DbSet.Where(w => w.ExpenditureNo.StartsWith(prefix))
                .OrderByDescending(o => o.ExpenditureNo)
                .Select(s => int.Parse(s.ExpenditureNo.Replace(prefix, "")))
                .FirstOrDefault();

            var curNo = $"{prefix}{(lastNo + 1).ToString("D5")}";

            return curNo;
        }

        private GarmentLeftoverWarehouseStock GenerateStock(GarmentLeftoverWarehouseExpenditureAccessoriesItem item)
        {
            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
            {
                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.ACCESSORIES,
                UnitId = item.UnitId,
                UnitCode = item.UnitCode,
                UnitName = item.UnitName,
                PONo = item.PONo,
                UomId = item.UomId,
                UomUnit = item.UomUnit,
                Quantity = item.Quantity,
                ProductCode = item.ProductCode,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                BasicPrice = item.BasicPrice
            };

            return stock;
        }

        public List<GarmentLeftoverWarehouseReceiptAccessoryItem> getProductForPDF(GarmentLeftoverWarehouseExpenditureAccessories model)
        {
            List<GarmentLeftoverWarehouseReceiptAccessoryItem> garmentProducts = new List<GarmentLeftoverWarehouseReceiptAccessoryItem>();
            foreach (var item in model.Items)
            {
                var stock = DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems.Where(a => a.POSerialNumber==item.PONo && a.ProductId == item.ProductId).FirstOrDefault();
                if (stock != null)
                {
                    garmentProducts.Add(stock);
                }
                else
                {
                    var balance= DbContext.GarmentLeftoverWarehouseBalanceStocksItems.Where(a => a.PONo == item.PONo && a.ProductId==item.ProductId).FirstOrDefault();
                    if (balance != null)
                    {
                        GarmentLeftoverWarehouseReceiptAccessoryItem garmentLeftoverWarehouseReceiptAccessoryItem = new GarmentLeftoverWarehouseReceiptAccessoryItem
                        {
                            ProductRemark = balance.ProductRemark,
                            ProductId = balance.ProductId
                        };
                        garmentProducts.Add(garmentLeftoverWarehouseReceiptAccessoryItem);
                    }
                }
            }
            return garmentProducts;
        }
    }
}
