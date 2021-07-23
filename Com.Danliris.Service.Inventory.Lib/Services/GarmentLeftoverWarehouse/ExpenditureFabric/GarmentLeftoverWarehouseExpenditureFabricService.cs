using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFabric
{
    public class GarmentLeftoverWarehouseExpenditureFabricService : IGarmentLeftoverWarehouseExpenditureFabricService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseExpenditureFabricService";

        private InventoryDbContext DbContext;
        private DbSet<GarmentLeftoverWarehouseExpenditureFabric> DbSet;
        private DbSet<GarmentLeftoverWarehouseExpenditureFabricItem> DbSetItem;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        private readonly IGarmentLeftoverWarehouseStockService StockService;

        private readonly string GarmentUnitReceiptNoteUri;
        private readonly string GarmentCoreProductUri;

        public GarmentLeftoverWarehouseExpenditureFabricService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentLeftoverWarehouseExpenditureFabric>();
            DbSetItem = DbContext.Set<GarmentLeftoverWarehouseExpenditureFabricItem>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));

            StockService = (IGarmentLeftoverWarehouseStockService)serviceProvider.GetService(typeof(IGarmentLeftoverWarehouseStockService));

            GarmentUnitReceiptNoteUri = APIEndpoint.Purchasing + "garment-unit-expenditure-notes/";
            GarmentCoreProductUri = APIEndpoint.Core + "master/garmentProducts";
        }

        public GarmentLeftoverWarehouseExpenditureFabric MapToModel(GarmentLeftoverWarehouseExpenditureFabricViewModel viewModel)
        {
            GarmentLeftoverWarehouseExpenditureFabric model = new GarmentLeftoverWarehouseExpenditureFabric();
            PropertyCopier<GarmentLeftoverWarehouseExpenditureFabricViewModel, GarmentLeftoverWarehouseExpenditureFabric>.Copy(viewModel, model);

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

            model.Items = new List<GarmentLeftoverWarehouseExpenditureFabricItem>();
            foreach (var viewModelItem in viewModel.Items)
            {
                GarmentLeftoverWarehouseExpenditureFabricItem modelItem = new GarmentLeftoverWarehouseExpenditureFabricItem();
                PropertyCopier<GarmentLeftoverWarehouseExpenditureFabricItemViewModel, GarmentLeftoverWarehouseExpenditureFabricItem>.Copy(viewModelItem, modelItem);

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

                model.Items.Add(modelItem);
            }

            return model;
        }

        public GarmentLeftoverWarehouseExpenditureFabricViewModel MapToViewModel(GarmentLeftoverWarehouseExpenditureFabric model)
        {
            GarmentLeftoverWarehouseExpenditureFabricViewModel viewModel = new GarmentLeftoverWarehouseExpenditureFabricViewModel();
            PropertyCopier<GarmentLeftoverWarehouseExpenditureFabric, GarmentLeftoverWarehouseExpenditureFabricViewModel>.Copy(model, viewModel);

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
                viewModel.Items = new List<GarmentLeftoverWarehouseExpenditureFabricItemViewModel>();
                foreach (var modelItem in model.Items)
                {
                    GarmentLeftoverWarehouseExpenditureFabricItemViewModel viewModelItem = new GarmentLeftoverWarehouseExpenditureFabricItemViewModel();
                    PropertyCopier<GarmentLeftoverWarehouseExpenditureFabricItem, GarmentLeftoverWarehouseExpenditureFabricItemViewModel>.Copy(modelItem, viewModelItem);

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

                    viewModel.Items.Add(viewModelItem);
                }
            }

            return viewModel;
        }

        public async Task<int> CreateAsync(GarmentLeftoverWarehouseExpenditureFabric model)
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

        public ReadResponse<GarmentLeftoverWarehouseExpenditureFabric> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentLeftoverWarehouseExpenditureFabric> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "ExpenditureNo", "ExpenditureDestination", "UnitExpenditureName", "BuyerName", "EtcRemark"
            };
            Query = QueryHelper<GarmentLeftoverWarehouseExpenditureFabric>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentLeftoverWarehouseExpenditureFabric>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentLeftoverWarehouseExpenditureFabric>.Order(Query, OrderDictionary);

            List<string> SelectedFields = (select != null && select.Count > 0) ? select : new List<string>()
            {
                "Id", "ExpenditureNo", "ExpenditureDate", "ExpenditureDestination", "UnitExpenditure", "Buyer", "EtcRemark"
            };

            Query = Query.Select(s => new GarmentLeftoverWarehouseExpenditureFabric
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

            Pageable<GarmentLeftoverWarehouseExpenditureFabric> pageable = new Pageable<GarmentLeftoverWarehouseExpenditureFabric>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseExpenditureFabric> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<GarmentLeftoverWarehouseExpenditureFabric>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<GarmentLeftoverWarehouseExpenditureFabric> ReadByIdAsync(int id)
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

                    GarmentLeftoverWarehouseExpenditureFabric model = await ReadByIdAsync(id);
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

        public async Task<int> UpdateAsync(int id, GarmentLeftoverWarehouseExpenditureFabric model)
        {
            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    int Updated = 0;

                    GarmentLeftoverWarehouseExpenditureFabric existingModel = await ReadByIdAsync(id);
                    if (existingModel.ExpenditureDate != model.ExpenditureDate)
                    {
                        existingModel.ExpenditureDate = model.ExpenditureDate;
                    }
                    if (existingModel.Remark != model.Remark)
                    {
                        existingModel.Remark = model.Remark;
                    }
                    if (existingModel.QtyKG != model.QtyKG)
                    {
                        existingModel.QtyKG = model.QtyKG;
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

        private string GenerateNo(GarmentLeftoverWarehouseExpenditureFabric model)
        {
            string prefix = $"BKF{model._CreatedUtc.ToString("yy")}{model._CreatedUtc.ToString("MM")}";

            var lastNo = DbSet.Where(w => w.ExpenditureNo.StartsWith(prefix))
                .OrderByDescending(o => o.ExpenditureNo)
                .Select(s => int.Parse(s.ExpenditureNo.Replace(prefix, "")))
                .FirstOrDefault();

            var curNo = $"{prefix}{(lastNo + 1).ToString("D5")}";

            return curNo;
        }

        private GarmentLeftoverWarehouseStock GenerateStock(GarmentLeftoverWarehouseExpenditureFabricItem item)
        {
            GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
            {
                ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FABRIC,
                UnitId = item.UnitId,
                UnitCode = item.UnitCode,
                UnitName = item.UnitName,
                PONo = item.PONo,
                UomId = item.UomId,
                UomUnit = item.UomUnit,
                Quantity = item.Quantity,
                BasicPrice = item.BasicPrice
            };

            return stock;
        }

        public GarmentProductViewModel GetProductFromCore(string productId)
        {
            var httpService = (IHttpService)ServiceProvider.GetService(typeof(IHttpService));
            var responseGarmentProduct = httpService.GetAsync($"{GarmentCoreProductUri}/" + productId).Result.Content.ReadAsStringAsync();

            Dictionary<string, object> resultGarmentProduct = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseGarmentProduct.Result);
            GarmentProductViewModel viewModel = JsonConvert.DeserializeObject<GarmentProductViewModel>(resultGarmentProduct.GetValueOrDefault("data").ToString());
            return viewModel;

        }

        public List<GarmentProductViewModel> getProductForPDF (GarmentLeftoverWarehouseExpenditureFabric model)
        {
            List<GarmentProductViewModel> garmentProducts = new List<GarmentProductViewModel>();
            foreach (var item in model.Items)
            {
                GarmentProductViewModel garmentProduct = new GarmentProductViewModel();
                var stock = DbContext.GarmentLeftoverWarehouseStocks.Where(a => a.PONo == item.PONo).FirstOrDefault();
                if (stock != null)
                {
                    garmentProduct = GetProductFromCore(stock.ProductId.ToString());
                    garmentProduct.PONo = item.PONo;
                    garmentProducts.Add(garmentProduct);
                }
            }
            return garmentProducts;
        }

    }
}
