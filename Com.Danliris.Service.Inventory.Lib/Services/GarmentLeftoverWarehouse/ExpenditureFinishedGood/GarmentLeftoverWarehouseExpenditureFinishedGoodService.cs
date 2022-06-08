using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Stock;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFinishedGood
{
    public class GarmentLeftoverWarehouseExpenditureFinishedGoodService : IGarmentLeftoverWarehouseExpenditureFinishedGoodService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseReceiptFinishedGoodService";

        private InventoryDbContext DbContext;
        private DbSet<GarmentLeftoverWarehouseExpenditureFinishedGood> DbSet;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        private readonly IGarmentLeftoverWarehouseStockService StockService;

        private readonly string GarmentExpenditureGoodUri;

        public GarmentLeftoverWarehouseExpenditureFinishedGoodService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentLeftoverWarehouseExpenditureFinishedGood>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));

            StockService = (IGarmentLeftoverWarehouseStockService)serviceProvider.GetService(typeof(IGarmentLeftoverWarehouseStockService));

            GarmentExpenditureGoodUri = APIEndpoint.GarmentProduction + "expenditure-goods/byRO";

        }
        public async Task<int> CreateAsync(GarmentLeftoverWarehouseExpenditureFinishedGood model)
        {
            int Created = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    model.FlagForCreate(IdentityService.Username, UserAgent);
                    model.FlagForUpdate(IdentityService.Username, UserAgent);

                    model.FinishedGoodExpenditureNo = GenerateNo(model);

                    foreach (var item in model.Items)
                    {
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                    }
                    DbSet.Add(model);

                    Created = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FINISHED_GOOD,
                            UnitId = item.UnitId,
                            UnitCode = item.UnitCode,
                            UnitName = item.UnitName,
                            RONo = item.RONo,
                            Quantity = item.ExpenditureQuantity,
                            LeftoverComodityCode = item.LeftoverComodityCode,
                            LeftoverComodityId = item.LeftoverComodityId,
                            LeftoverComodityName = item.LeftoverComodityName
                        };
                        await StockService.StockOut(stock, model.FinishedGoodExpenditureNo, model.Id, item.Id);
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return Created;
        }

        public async Task<int> DeleteAsync(int id)
        {
            int Deleted = 0;

            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    GarmentLeftoverWarehouseExpenditureFinishedGood model = await ReadByIdAsync(id);
                    model.FlagForDelete(IdentityService.Username, UserAgent);
                    foreach (var item in model.Items)
                    {
                        item.FlagForDelete(IdentityService.Username, UserAgent);
                    }

                    Deleted = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FINISHED_GOOD,
                            UnitId = item.UnitId,
                            UnitCode = item.UnitCode,
                            UnitName = item.UnitName,
                            RONo = item.RONo,
                            Quantity = item.ExpenditureQuantity,
                            LeftoverComodityCode = item.LeftoverComodityCode,
                            LeftoverComodityId = item.LeftoverComodityId,
                            LeftoverComodityName = item.LeftoverComodityName
                        };

                        await StockService.StockIn(stock, model.FinishedGoodExpenditureNo, model.Id, item.Id);
                    }


                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return Deleted;
        }

        public GarmentLeftoverWarehouseExpenditureFinishedGood MapToModel(GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel viewModel)
        {
            GarmentLeftoverWarehouseExpenditureFinishedGood model = new GarmentLeftoverWarehouseExpenditureFinishedGood();
            PropertyCopier<GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel, GarmentLeftoverWarehouseExpenditureFinishedGood>.Copy(viewModel, model);

            if (viewModel.Buyer != null)
            {
                model.BuyerId = viewModel.Buyer.Id;
                model.BuyerCode = viewModel.Buyer.Code;
                model.BuyerName = viewModel.Buyer.Name;
            }


            model.Items = new List<GarmentLeftoverWarehouseExpenditureFinishedGoodItem>();
            foreach (var viewModelItem in viewModel.Items)
            {
                GarmentLeftoverWarehouseExpenditureFinishedGoodItem modelItem = new GarmentLeftoverWarehouseExpenditureFinishedGoodItem();
                PropertyCopier<GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel, GarmentLeftoverWarehouseExpenditureFinishedGoodItem>.Copy(viewModelItem, modelItem);

                if (viewModelItem.Unit != null)
                {
                    modelItem.UnitId = long.Parse(viewModelItem.Unit.Id);
                    modelItem.UnitCode = viewModelItem.Unit.Code;
                    modelItem.UnitName = viewModelItem.Unit.Name;
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

        public GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel MapToViewModel(GarmentLeftoverWarehouseExpenditureFinishedGood model)
        {
            GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel viewModel = new GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel();
            PropertyCopier<GarmentLeftoverWarehouseExpenditureFinishedGood, GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel>.Copy(model, viewModel);

            viewModel.Buyer = new BuyerViewModel
            {
                Id = model.BuyerId,
                Code = model.BuyerCode,
                Name = model.BuyerName
            };

            if (model.Items != null)
            {
                viewModel.Items = new List<GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel>();
                foreach (var modelItem in model.Items)
                {
                    GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel viewModelItem = new GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel();
                    PropertyCopier<GarmentLeftoverWarehouseExpenditureFinishedGoodItem, GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel>.Copy(modelItem, viewModelItem);


                    viewModelItem.Unit = new UnitViewModel
                    {
                        Id = modelItem.UnitId.ToString(),
                        Code = modelItem.UnitCode,
                        Name = modelItem.UnitName
                    };

                    viewModelItem.LeftoverComodity = new LeftoverComodityViewModel
                    {
                        Id = modelItem.LeftoverComodityId,
                        Name = modelItem.LeftoverComodityName,
                        Code = modelItem.LeftoverComodityCode
                    };

                    viewModel.Items.Add(viewModelItem);
                }
            }

            return viewModel;
        }

        public ReadResponse<GarmentLeftoverWarehouseExpenditureFinishedGood> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentLeftoverWarehouseExpenditureFinishedGood> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "FinishedGoodExpenditureNo", "BuyerName","BuyerCode", "ExpenditureTo", "OtherDescription"
            };
            Query = QueryHelper<GarmentLeftoverWarehouseExpenditureFinishedGood>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentLeftoverWarehouseExpenditureFinishedGood>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentLeftoverWarehouseExpenditureFinishedGood>.Order(Query, OrderDictionary);

            List<string> SelectedFields = (select != null && select.Count > 0) ? select : new List<string>()
            {
                "Id", "FinishedGoodExpenditureNo", "Buyer", "OtherDescription", "ExpenditureTo","ExpenditureDate"
            };

            Query = Query.Select(s => new GarmentLeftoverWarehouseExpenditureFinishedGood
            {
                Id = s.Id,
                FinishedGoodExpenditureNo=s.FinishedGoodExpenditureNo,
                ExpenditureDate=s.ExpenditureDate,
                ExpenditureTo=s.ExpenditureTo,
                BuyerCode=s.BuyerCode,
                BuyerId=s.BuyerId,
                BuyerName=s.BuyerName,
                OtherDescription=s.OtherDescription
            });

            Pageable<GarmentLeftoverWarehouseExpenditureFinishedGood> pageable = new Pageable<GarmentLeftoverWarehouseExpenditureFinishedGood>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseExpenditureFinishedGood> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<GarmentLeftoverWarehouseExpenditureFinishedGood>(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<GarmentLeftoverWarehouseExpenditureFinishedGood> ReadByIdAsync(int id)
        {
            return await DbSet
                .Where(w => w.Id == id)
                .Include(i => i.Items)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAsync(int id, GarmentLeftoverWarehouseExpenditureFinishedGood model)
        {
            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {

                    int Updated = 0;

                    GarmentLeftoverWarehouseExpenditureFinishedGood existingModel = await ReadByIdAsync(id);
                    if (existingModel.ExpenditureDate != model.ExpenditureDate)
                    {
                        existingModel.ExpenditureDate = model.ExpenditureDate;
                    }
                    if (existingModel.Consignment != model.Consignment)
                    {
                        existingModel.Consignment = model.Consignment;
                    }
                    if (existingModel.Description != model.Description)
                    {
                        existingModel.Description = model.Description;
                    }
                    existingModel.FlagForUpdate(IdentityService.Username, UserAgent);

                    foreach (var existingItem in existingModel.Items)
                    {
                        GarmentLeftoverWarehouseStock stockIn = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FINISHED_GOOD,
                            UnitId = existingItem.UnitId,
                            UnitCode = existingItem.UnitCode,
                            UnitName = existingItem.UnitName,
                            RONo = existingItem.RONo,
                            Quantity = existingItem.ExpenditureQuantity,
                            LeftoverComodityCode= existingItem.LeftoverComodityCode,
                            LeftoverComodityId= existingItem.LeftoverComodityId,
                            LeftoverComodityName= existingItem.LeftoverComodityName
                        };

                        await StockService.StockIn(stockIn, model.FinishedGoodExpenditureNo, model.Id, existingItem.Id);
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
                            if (existingItem.ExpenditureQuantity != item.ExpenditureQuantity)
                            {
                                existingItem.ExpenditureQuantity = item.ExpenditureQuantity;
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
                        GarmentLeftoverWarehouseStock stock = new GarmentLeftoverWarehouseStock
                        {
                            ReferenceType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FINISHED_GOOD,
                            UnitId = item.UnitId,
                            UnitCode = item.UnitCode,
                            UnitName = item.UnitName,
                            RONo = item.RONo,
                            Quantity = item.ExpenditureQuantity,
                            LeftoverComodityCode = item.LeftoverComodityCode,
                            LeftoverComodityId = item.LeftoverComodityId,
                            LeftoverComodityName = item.LeftoverComodityName
                        };

                        await StockService.StockOut(stock, model.FinishedGoodExpenditureNo, model.Id, item.Id);
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

        private string GenerateNo(GarmentLeftoverWarehouseExpenditureFinishedGood model)
        {
            string prefix = "BKB" + model._CreatedUtc.ToString("yy") + model._CreatedUtc.ToString("MM");

            var lastNo = DbSet.Where(w => w.FinishedGoodExpenditureNo.StartsWith(prefix))
                .OrderByDescending(o => o.FinishedGoodExpenditureNo)
                .Select(s => int.Parse(s.FinishedGoodExpenditureNo.Replace(prefix, "")))
                .FirstOrDefault();

            var curNo = $"{prefix}{(lastNo + 1).ToString("D4")}";

            return curNo;
        }

        
        public IQueryable<ExpenditureFInishedGoodReportViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo,int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? new DateTime(1970, 1, 1) : (DateTime)dateTo;
            int index = 0;
            var Query = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoods
                                   where data._IsDeleted == false
                                   && data.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date
                                   && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                                   select new { data.BuyerName,data.OtherDescription,data.Consignment,data.Id, data.FinishedGoodExpenditureNo, data.ExpenditureDate, data.ExpenditureTo, data.LocalSalesNoteNo })
                        join b in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoodItems on a.Id equals b.FinishedGoodExpenditureId
                        select new ExpenditureFInishedGoodReportViewModel {
                           FinishedGoodExpenditureNo= a.FinishedGoodExpenditureNo,
                           ExpenditureDate= a.ExpenditureDate,
                           LocalSalesNoteNo=a.LocalSalesNoteNo,
                           Consignment = a.Consignment == true? "YA": "TIDAK",
                           ExpenditureTo=a.ExpenditureTo,
                           ExpenditureDestinationDesc= a.ExpenditureTo =="JUAL LOKAL"? a.BuyerName : a.OtherDescription,
                            UnitFrom =new UnitViewModel {

                                        Code= b.UnitCode,
                                        Name=b.UnitName },
                            RONo = b.RONo,
                            LeftoverComodityCode = b.LeftoverComodityCode,
                            LeftoverComodityName = b.LeftoverComodityName,
                            ExpenditureQuantity = b.ExpenditureQuantity,
                            Uom ="PCS",
                            Price = Math.Round(b.BasicPrice * b.ExpenditureQuantity, 2)
                        };
            return Query;
        }

        public Tuple<List<ExpenditureFInishedGoodReportViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string order, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.ExpenditureDate);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];
                Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<ExpenditureFInishedGoodReportViewModel> pageable = new Pageable<ExpenditureFInishedGoodReportViewModel>(Query, page - 1, size);
            List<ExpenditureFInishedGoodReportViewModel> Data = pageable.Data.ToList<ExpenditureFInishedGoodReportViewModel>();
            
            int TotalData = pageable.TotalCount;
            int index = 0;
            Data.ForEach(c => {
                var comodity = GetComodityFromProduction(c.RONo);
                c.UnitComodityCode = (comodity.Count() > 0) ? comodity.FirstOrDefault().Comodity.Code : "-";
                index += 1;
                c.index = index;

            });

            if (TotalData == page * size && TotalData != 0)
            {
                var QtyTotal = Query.Sum(x => x.ExpenditureQuantity);
                var PriceTotal = Math.Round(Query.Sum(x => x.Price),2);
                //var WeightTotal = Query.Sum(x => x.Weight);
                ExpenditureFInishedGoodReportViewModel vm = new ExpenditureFInishedGoodReportViewModel();

                vm.FinishedGoodExpenditureNo = "T O T A L";
                vm.ExpenditureDate = DateTimeOffset.MinValue;
                vm.LocalSalesNoteNo = "";
                vm.Consignment = "";
                vm.ExpenditureTo = "";
                vm.ExpenditureDestinationDesc = "";
                vm.UnitFrom = new UnitViewModel();
                vm.RONo = "";
                vm.LeftoverComodityCode = "";
                vm.LeftoverComodityName = "";
                vm.ExpenditureQuantity = QtyTotal;
                vm.Uom = "";
                vm.Price = PriceTotal;

                Data.Add(vm);

            }

            return Tuple.Create(Data, TotalData);
        }
        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(b => b.ExpenditureDate);

            var QtyTotal = Query.Sum(x => x.ExpenditureQuantity);
            var PriceTotal = Math.Round(Query.Sum(x => x.Price),2);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Bon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tujuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Tujuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit Asal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Komoditi Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Komoditi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Komoditi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Titip Jual", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Nota Penjualan", DataType = typeof(String) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", 0, "", 0, "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    var comodity = GetComodityFromProduction(item.RONo);
                    //c.UnitComodityCode = comodity != null ? comodity.Comodity.Code : "-";

                    index++;
                    //DateTimeOffset date = item.date ?? new DateTime(1970, 1, 1);
                    //string dateString = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.FinishedGoodExpenditureNo, item.ExpenditureDate.AddHours(offset).ToString("dd MMM yyyy", new CultureInfo("id-ID")), item.ExpenditureTo, item.ExpenditureDestinationDesc, item.UnitFrom.Code, item.RONo, (comodity.Count() > 0 ? comodity.FirstOrDefault().Comodity.Code : "-"), item.LeftoverComodityCode, item.LeftoverComodityName, item.ExpenditureQuantity, item.Uom, item.Price, item.Consignment, item.LocalSalesNoteNo);
                }

                result.Rows.Add("", "T O T A L ..........", "", "", "", "", "", "", "", "", QtyTotal, "", PriceTotal, "", "");
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Report Pengeluaran Gudang Sisa Barang Jadi") }, true);

        }

        private List<ExpendGoodViewModel> GetComodityFromProduction(string RONo)
        {
            var httpService = (IHttpService)ServiceProvider.GetService(typeof(IHttpService));

            //Dictionary<string, object> filterLocalCoverLetter = new Dictionary<string, object> { { "POSerialNumber", POSerialNumber } };
            //var filter = JsonConvert.SerializeObject(filterLocalCoverLetter);
            var responseLocalCoverLetter = httpService.GetAsync($"{GarmentExpenditureGoodUri}?RONo=" + RONo).Result;

            if (responseLocalCoverLetter.IsSuccessStatusCode)
            {
                var content = responseLocalCoverLetter.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> resultGarmentProduct = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                var jsonLocalCoverLetter = resultGarmentProduct.Single(p => p.Key.Equals("data")).Value;
                //if (a.Count > 0)
                //{
                var a = JsonConvert.DeserializeObject<List<ExpendGoodViewModel>>(jsonLocalCoverLetter.ToString());
                return a;
            }

            else
            {
                return new List<ExpendGoodViewModel>();
            }
        }

    }
}
