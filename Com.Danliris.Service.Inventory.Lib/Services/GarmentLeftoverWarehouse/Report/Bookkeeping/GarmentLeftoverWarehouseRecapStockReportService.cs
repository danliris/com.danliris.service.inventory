using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Bookkeeping;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Bookkeeping
{
    public class GarmentLeftoverWarehouseRecapStockReportService : IGarmentLeftoverWarehouseRecapStockReportService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseRecapStockReportService";

        private InventoryDbContext DbContext;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        public GarmentLeftoverWarehouseRecapStockReportService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));
        }

        public IQueryable<GarmentLeftoverWarehouseRecapStockReportViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, int offset)
        {

            DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
            DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;


            List<GarmentLeftoverWarehouseRecapStockReportViewModel> GarmentLeftoverWarehouseRecapStockReportViewModel = new List<GarmentLeftoverWarehouseRecapStockReportViewModel>();
            #region Saldo
            var QueryBalanceFABRIC = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                                where data._IsDeleted == false && data.TypeOfGoods.ToString() == "FABRIC"
                                                select new { data._CreatedUtc, data.Id })
                                     join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
                                     select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                     {
                                         Description = "SALDO",
                                         FabricPrice = b.Quantity * b.BasicPrice,
                                         FabricQty = b.Quantity,
                                         FabricUom = b.UomUnit,
                                         AccPrice = 0,
                                         FinishedGoodQty = 0,
                                         FinishedGoodPrice = 0,
                                         FinishedGoodUom = "PCS",
                                         Unit = b.UnitName
                                     };
            var QueryBalanceReceiptFABRIC = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFabrics
                                                       where data._IsDeleted == false
                                                  && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                                                       select new { data.UnitFromName, data.ReceiptDate, data.Id })
                                            join b in DbContext.GarmentLeftoverWarehouseReceiptFabricItems on a.Id equals b.GarmentLeftoverWarehouseReceiptFabricId
                                            select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                            {
                                                Description = "SALDO",
                                                FabricPrice = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date ? b.Quantity * b.BasicPrice : 0,
                                                FabricQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date ? b.Quantity : 0,
                                                FabricUom = b.UomUnit,
                                                AccPrice = 0,
                                                FinishedGoodQty = 0,
                                                FinishedGoodPrice = 0,
                                                FinishedGoodUom = "PCS",
                                                Unit = a.UnitFromName
                                            };
            var QueryBalanceExpenditureFABRIC = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFabrics
                                                           where data._IsDeleted == false
                                                               && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                                                           select new { data.UnitExpenditureCode, data.ExpenditureDate, data.Id, data.ExpenditureDestination })
                                                join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFabricItems
                                                           select new { expend.BasicPrice, expend.UomUnit, expend.Quantity, expend.UnitName, expend.ExpenditureId }) on a.Id equals b.ExpenditureId
                                                select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                                {
                                                    Description = "SALDO",
                                                    FabricPrice = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date ? -b.Quantity * b.BasicPrice : 0,
                                                    FabricQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date ? -b.Quantity : 0,
                                                    FabricUom = b.UomUnit,
                                                    AccPrice = 0,
                                                    FinishedGoodQty = 0,
                                                    FinishedGoodPrice = 0,
                                                    FinishedGoodUom = "PCS",

                                                    Unit = b.UnitName
                                                };
            var QueryBalanceACC = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                             where data._IsDeleted == false && data.TypeOfGoods.ToString() == "ACCESSORIES"
                                             select new { data._CreatedUtc, data.Id })
                                  join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
                                  select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                  {
                                      Description = "SALDO",
                                      FabricPrice = 0,
                                      FabricQty = 0,
                                      AccPrice = b.Quantity * b.BasicPrice,
                                      FinishedGoodQty = 0,
                                      FinishedGoodPrice = 0,
                                      FinishedGoodUom = "PCS",
                                      FabricUom = "MT",
                                      Unit = b.UnitName
                                  };
            var QueryBalanceReceiptACC = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptAccessories
                                                    where data._IsDeleted == false
                                               && data.StorageReceiveDate.AddHours(offset).Date <= DateTo.Date
                                                    select new { data.RequestUnitName, data.StorageReceiveDate, data.Id })
                                         join b in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems on a.Id equals b.GarmentLeftOverWarehouseReceiptAccessoriesId
                                         select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                         {
                                             Description = "SALDO",
                                             FabricPrice = 0,
                                             FabricQty = 0,
                                             AccPrice = a.StorageReceiveDate.AddHours(offset).Date < DateFrom.Date ? b.Quantity * b.BasicPrice : 0,
                                             FabricUom = "MT",
                                             FinishedGoodQty = 0,
                                             FinishedGoodPrice = 0,
                                             FinishedGoodUom = "PCS",
                                             Unit = a.RequestUnitName
                                         };
            var QueryBalanceExpenditureACC = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureAccessories
                                                        where data._IsDeleted == false
                                                            && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                                                        select new { data.UnitExpenditureCode, data.ExpenditureDate, data.Id, data.ExpenditureDestination })
                                             join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureAccessoriesItems
                                                        select new { expend.BasicPrice, expend.UomUnit, expend.Quantity, expend.UnitName, expend.ExpenditureId }) on a.Id equals b.ExpenditureId
                                             select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                             {
                                                 Description = "SALDO",
                                                 FabricPrice = 0,
                                                 FabricQty = 0,
                                                 AccPrice = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date ? -b.Quantity * b.BasicPrice : 0,
                                                 FabricUom = "MT",
                                                 FinishedGoodQty = 0,
                                                 FinishedGoodPrice = 0,
                                                 FinishedGoodUom = "PCS",
                                                 Unit = b.UnitName
                                             };
            var QueryBalanceFinishedGood = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                                      where data._IsDeleted == false && data.TypeOfGoods.ToString() == "BARANG JADI"
                                                      select new { data._CreatedUtc, data.Id })
                                           join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
                                           select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                           {
                                               Description = "SALDO",
                                               FabricPrice = 0,
                                               FabricQty = 0,
                                               AccPrice = 0,
                                               FinishedGoodQty = b.Quantity,
                                               FinishedGoodPrice = b.Quantity * b.BasicPrice,
                                               FinishedGoodUom = "PCS",
                                               FabricUom = "MT",
                                               Unit = b.UnitName
                                           };
            var QueryBalanceReceiptFinishedGood = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoods
                                                             where data._IsDeleted == false
                                                        && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                                                             select new { data.UnitFromName, data.ReceiptDate, data.Id })
                                                  join b in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoodItems on a.Id equals b.FinishedGoodReceiptId
                                                  select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                                  {
                                                      Description = "SALDO",
                                                      FabricPrice = 0,
                                                      FabricQty = 0,
                                                      AccPrice = 0,
                                                      FabricUom = "MT",
                                                      FinishedGoodQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date ? b.Quantity : 0,
                                                      FinishedGoodPrice = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date ? b.Quantity * b.BasicPrice : 0,
                                                      FinishedGoodUom = "PCS",
                                                      Unit = a.UnitFromName
                                                  };
            var QueryBalanceExpenditureFinishedGood = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoods
                                                                 where data._IsDeleted == false
                                                                     && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                                                                 select new { data.ExpenditureDate, data.Id })
                                                      join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoodItems
                                                                 select new { expend.BasicPrice, expend.ExpenditureQuantity, expend.UnitName, expend.FinishedGoodExpenditureId }) on a.Id equals b.FinishedGoodExpenditureId
                                                      select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                                      {
                                                          Description = "SALDO",
                                                          FabricPrice = 0,
                                                          FabricQty = 0,
                                                          AccPrice = 0,
                                                          FabricUom = "MT",
                                                          FinishedGoodQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date ? -b.ExpenditureQuantity : 0,
                                                          FinishedGoodPrice = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date ? -b.ExpenditureQuantity * b.BasicPrice : 0,
                                                          FinishedGoodUom = "PCS",
                                                          Unit = b.UnitName
                                                      };

            #endregion

            #region receipt
            var QueryReceiptHeader = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFabrics
                                                where data._IsDeleted == false
                                            && data.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                                                select new { data.UnitFromName, data.ReceiptDate, data.Id })
                                     join b in DbContext.GarmentLeftoverWarehouseReceiptFabricItems on a.Id equals b.GarmentLeftoverWarehouseReceiptFabricId
                                     select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                     {
                                         Description = "PENERIMAAN"

                                     };
            var QueryReceiptFABRICNow = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFabrics
                                                   where data._IsDeleted == false
                                               && data.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                                                   select new { data.UnitFromName, data.ReceiptDate, data.Id })
                                        join b in DbContext.GarmentLeftoverWarehouseReceiptFabricItems on a.Id equals b.GarmentLeftoverWarehouseReceiptFabricId
                                        select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                        {
                                            Description = "D/" + a.UnitFromName,
                                            FabricPrice = b.Quantity * b.BasicPrice,
                                            FabricQty = b.Quantity,
                                            FabricUom = "MT",
                                            AccPrice = 0,
                                            FinishedGoodQty = 0,
                                            FinishedGoodPrice = 0,
                                            FinishedGoodUom = "PCS",
                                            Unit = a.UnitFromName
                                        };
            var QueryReceiptNow = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptAccessories
                                             where data._IsDeleted == false
                                        && data.StorageReceiveDate.AddHours(offset).Date >= DateFrom.Date && data.StorageReceiveDate.AddHours(offset).Date <= DateTo.Date
                                             select new { data.RequestUnitName, data.StorageReceiveDate, data.Id })
                                  join b in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems on a.Id equals b.GarmentLeftOverWarehouseReceiptAccessoriesId
                                  select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                  {
                                      Description = "D/" + a.RequestUnitName,
                                      FabricPrice = 0,
                                      FabricQty = 0,
                                      AccPrice = b.Quantity * b.BasicPrice,
                                      FabricUom = "MT",
                                      FinishedGoodQty = 0,
                                      FinishedGoodPrice = 0,
                                      FinishedGoodUom = "PCS",
                                      Unit = a.RequestUnitName
                                  };
            var QueryReceiptFinishedGoodNow = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoods
                                                         where data._IsDeleted == false
                                                        && data.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                                                         select new { data.UnitFromName, data.ReceiptDate, data.Id })
                                              join b in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoodItems on a.Id equals b.FinishedGoodReceiptId
                                              select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                              {
                                                  Description = "D/" + a.UnitFromName,
                                                  FabricPrice = 0,
                                                  FabricQty = 0,
                                                  AccPrice = 0,
                                                  FabricUom = "MT",
                                                  FinishedGoodQty = b.Quantity,
                                                  FinishedGoodPrice = b.Quantity * b.BasicPrice,
                                                  FinishedGoodUom = "PCS",
                                                  Unit = a.UnitFromName
                                              };

            #endregion
            #region expenditure
            var QueryExpenditureFABRICNow = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFabrics
                                                       where data._IsDeleted == false
                                                               && data.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                                                       select new { data.UnitExpenditureCode, data.ExpenditureDate, data.Id, data.ExpenditureDestination })
                                            join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFabricItems
                                                       select new { expend.BasicPrice, expend.UomUnit, expend.Quantity, expend.UnitName, expend.ExpenditureId }) on a.Id equals b.ExpenditureId
                                            select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                            {
                                                Description = "U/" + a.ExpenditureDestination,
                                                FabricPrice = b.Quantity * b.BasicPrice,
                                                FabricQty = b.Quantity,
                                                FabricUom = b.UomUnit,
                                                AccPrice = 0,
                                                FinishedGoodQty = 0,
                                                FinishedGoodPrice = 0,
                                                FinishedGoodUom = "PCS",

                                                Unit = b.UnitName
                                            };
            var QueryExpenditureACCNow = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureAccessories
                                                    where data._IsDeleted == false
                                                        && data.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                                                    select new { data.UnitExpenditureCode, data.ExpenditureDate, data.Id, data.ExpenditureDestination })
                                         join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureAccessoriesItems
                                                    select new { expend.BasicPrice, expend.UomUnit, expend.Quantity, expend.UnitName, expend.ExpenditureId }) on a.Id equals b.ExpenditureId
                                         select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                         {
                                             Description = "U/" + a.ExpenditureDestination,
                                             FabricPrice = 0,
                                             FabricQty = 0,
                                             AccPrice = b.Quantity * b.BasicPrice,
                                             FabricUom = "MT",
                                             FinishedGoodQty = 0,
                                             FinishedGoodPrice = 0,
                                             FinishedGoodUom = "PCS",
                                             Unit = b.UnitName
                                         };
            var QueryExpenditureFinishedGoodNow = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoods
                                                             where data._IsDeleted == false
                                                                 && data.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                                                             select new { data.ExpenditureDate, data.Id,data.ExpenditureTo })
                                                  join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoodItems
                                                             select new { expend.BasicPrice, expend.ExpenditureQuantity, expend.UnitName, expend.FinishedGoodExpenditureId }) on a.Id equals b.FinishedGoodExpenditureId
                                                  select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                                  {
                                                      Description = "U/" + a.ExpenditureTo,
                                                      FabricPrice = 0,
                                                      FabricQty = 0,
                                                      AccPrice = 0,
                                                      FabricUom = "MT",
                                                      FinishedGoodQty = b.ExpenditureQuantity,
                                                      FinishedGoodPrice = b.ExpenditureQuantity * b.BasicPrice,
                                                      FinishedGoodUom = "PCS",
                                                      Unit = b.UnitName
                                                  };

            #endregion
            var QuerySaldo = QueryBalanceACC.Union(QueryBalanceExpenditureACC).Union(QueryBalanceReceiptACC)
                            .Union(QueryBalanceFABRIC).Union(QueryBalanceExpenditureFABRIC).Union(QueryBalanceReceiptFABRIC)
                            .Union(QueryBalanceFinishedGood).Union(QueryBalanceExpenditureFinishedGood).Union(QueryBalanceReceiptFinishedGood)
                            .Union(QueryReceiptHeader)
                            .Union(QueryReceiptFABRICNow).Union(QueryReceiptNow).Union(QueryReceiptFinishedGoodNow)
                            ;

            var queryReceipt = QueryReceiptFABRICNow.Union(QueryReceiptNow).Union(QueryReceiptFinishedGoodNow);
            var querySumReceipt = queryReceipt.ToList()
                 .GroupBy(x => new { x.FabricUom, x.FinishedGoodUom }, (key, group) => new
                 {
                     Description = "TOTAL",

                     FabricUom = key.FabricUom,
                     FinishedGoodUom = key.FinishedGoodUom,
                     FabricQty = group.Sum(s => s.FabricQty),
                     FabricPrice = group.Sum(s => s.FabricPrice),
                     AccPrice = group.Sum(s => s.AccPrice),
                     FinishedGoodQty = group.Sum(s => s.FinishedGoodQty),
                     FinishedGoodPrice = group.Sum(s => s.FinishedGoodPrice)
                 });
            var querySum = QuerySaldo.ToList()
                  .GroupBy(x => new { x.Description, x.FabricUom, x.FinishedGoodUom }, (key, group) => new
                  {
                      Description = key.Description,

                      FabricUom = key.FabricUom,
                      FinishedGoodUom = key.FinishedGoodUom,
                      FabricQty = group.Sum(s => s.FabricQty),
                      FabricPrice = group.Sum(s => s.FabricPrice),
                      AccPrice = group.Sum(s => s.AccPrice),
                      FinishedGoodQty = group.Sum(s => s.FinishedGoodQty),
                      FinishedGoodPrice = group.Sum(s => s.FinishedGoodPrice)
                  });
            foreach (var data in querySum)
            {

                GarmentLeftoverWarehouseRecapStockReportViewModel garmentLeftover = new GarmentLeftoverWarehouseRecapStockReportViewModel
                {
                    Description = data.Description,
                    FabricQty = data.FabricQty,
                    FabricUom = data.FabricUom,
                    FabricPrice = data.FabricPrice,
                    AccPrice = data.AccPrice,
                    FinishedGoodQty = data.FinishedGoodQty,
                    FinishedGoodUom = data.FinishedGoodUom,
                    FinishedGoodPrice = data.FinishedGoodPrice
                };
                GarmentLeftoverWarehouseRecapStockReportViewModel.Add(garmentLeftover);
            }
            foreach (var item in querySumReceipt)
            {
                GarmentLeftoverWarehouseRecapStockReportViewModel garmentLeftover = new GarmentLeftoverWarehouseRecapStockReportViewModel
                {
                    Description = item.Description,
                    FabricQty = item.FabricQty,
                    FabricUom = item.FabricUom,
                    FabricPrice = item.FabricPrice,
                    AccPrice = item.AccPrice,
                    FinishedGoodQty = item.FinishedGoodQty,
                    FinishedGoodUom = item.FinishedGoodUom,
                    FinishedGoodPrice = item.FinishedGoodPrice
                };
                GarmentLeftoverWarehouseRecapStockReportViewModel.Add(garmentLeftover);
            }
            var QueryExpendHeader = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFabrics
                                               where data._IsDeleted == false
                                           && data.ReceiptDate.AddHours(offset).Date >= DateFrom.Date && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                                               select new { data.UnitFromName, data.ReceiptDate, data.Id })
                                    join b in DbContext.GarmentLeftoverWarehouseReceiptFabricItems on a.Id equals b.GarmentLeftoverWarehouseReceiptFabricId
                                    select new GarmentLeftoverWarehouseRecapStockReportViewModel
                                    {
                                        Description = "PENGELUARAN"

                                    };
            var queryAddExpenditure = GarmentLeftoverWarehouseRecapStockReportViewModel.Union(QueryExpendHeader).Union(QueryExpenditureFABRICNow).Union(QueryExpenditureACCNow).Union(QueryExpenditureFinishedGoodNow);
            var queryExpend = QueryExpenditureFABRICNow.Union(QueryExpenditureACCNow).Union(QueryExpenditureFinishedGoodNow);
            var queryTotalExpend = queryExpend.ToList()
                 .GroupBy(x => new { x.FabricUom, x.FinishedGoodUom }, (key, group) => new
                 {
                     Description = "TOTAL",

                     FabricUom = key.FabricUom,
                     FinishedGoodUom = key.FinishedGoodUom,
                     FabricQty = group.Sum(s => s.FabricQty),
                     FabricPrice = group.Sum(s => s.FabricPrice),
                     AccPrice = group.Sum(s => s.AccPrice),
                     FinishedGoodQty = group.Sum(s => s.FinishedGoodQty),
                     FinishedGoodPrice = group.Sum(s => s.FinishedGoodPrice)
                 });

            var querySumEND = queryAddExpenditure.ToList()
                 .GroupBy(x => new { x.Description, x.FabricUom, x.FinishedGoodUom }, (key, group) => new
                 {
                     Description = key.Description,

                     FabricUom = key.FabricUom,
                     FinishedGoodUom = key.FinishedGoodUom,
                     FabricQty = group.Sum(s => s.FabricQty),
                     FabricPrice = group.Sum(s => s.FabricPrice),
                     AccPrice = group.Sum(s => s.AccPrice),
                     FinishedGoodQty = group.Sum(s => s.FinishedGoodQty),
                     FinishedGoodPrice = group.Sum(s => s.FinishedGoodPrice)
                 });
            GarmentLeftoverWarehouseRecapStockReportViewModel = new List<GarmentLeftoverWarehouseRecapStockReportViewModel>();
            foreach (var data in querySumEND)
            {

                GarmentLeftoverWarehouseRecapStockReportViewModel garmentLeftover = new GarmentLeftoverWarehouseRecapStockReportViewModel
                {
                    Description = data.Description,
                    FabricQty = data.FabricQty,
                    FabricUom = data.FabricUom,
                    FabricPrice = data.FabricPrice,
                    AccPrice = data.AccPrice,
                    FinishedGoodQty = data.FinishedGoodQty,
                    FinishedGoodUom = data.FinishedGoodUom,
                    FinishedGoodPrice = data.FinishedGoodPrice
                };
                GarmentLeftoverWarehouseRecapStockReportViewModel.Add(garmentLeftover);
            }
            foreach (var item in queryTotalExpend)
            {
                GarmentLeftoverWarehouseRecapStockReportViewModel garmentLeftover = new GarmentLeftoverWarehouseRecapStockReportViewModel
                {
                    Description = item.Description,
                    FabricQty = item.FabricQty,
                    FabricUom = item.FabricUom,
                    FabricPrice = item.FabricPrice,
                    AccPrice = item.AccPrice,
                    FinishedGoodQty = item.FinishedGoodQty,
                    FinishedGoodUom = item.FinishedGoodUom,
                    FinishedGoodPrice = item.FinishedGoodPrice
                };
                GarmentLeftoverWarehouseRecapStockReportViewModel.Add(garmentLeftover);
            }
            List<GarmentLeftoverWarehouseRecapStockReportViewModel> recapStockReportViewModels = new List<GarmentLeftoverWarehouseRecapStockReportViewModel>();

            foreach (var item in GarmentLeftoverWarehouseRecapStockReportViewModel.Where(s=>s.Description !="TOTAL"))
            {
                if(item.Description=="SALDO" || item.Description.Contains("D/"))
                {
                    GarmentLeftoverWarehouseRecapStockReportViewModel garmentLeftover = new GarmentLeftoverWarehouseRecapStockReportViewModel
                    {
                        Description = "SALDO AKHIR",
                        FabricQty = item.FabricQty,
                        FabricUom = item.FabricUom,
                        FabricPrice = item.FabricPrice,
                        AccPrice = item.AccPrice,
                        FinishedGoodQty = item.FinishedGoodQty,
                        FinishedGoodUom = item.FinishedGoodUom,
                        FinishedGoodPrice = item.FinishedGoodPrice
                    };
                    recapStockReportViewModels.Add(garmentLeftover);

                }else
                {
                    GarmentLeftoverWarehouseRecapStockReportViewModel garmentLeftover = new GarmentLeftoverWarehouseRecapStockReportViewModel
                    {
                        Description = "SALDO AKHIR",
                        FabricQty = -item.FabricQty,
                        FabricUom = item.FabricUom,
                        FabricPrice = -item.FabricPrice,
                        AccPrice = -item.AccPrice,
                        FinishedGoodQty = -item.FinishedGoodQty,
                        FinishedGoodUom = item.FinishedGoodUom,
                        FinishedGoodPrice = -item.FinishedGoodPrice
                    };
                    recapStockReportViewModels.Add(garmentLeftover);
                }

            }
            var querySaldoAkhir = recapStockReportViewModels.ToList()
                 .GroupBy(x => new { x.FabricUom, x.FinishedGoodUom,x.Description }, (key, group) => new
                 {
                     Description = key.Description,

                     FabricUom = key.FabricUom,
                     FinishedGoodUom = key.FinishedGoodUom,
                     FabricQty = group.Sum(s =>  s.FabricQty),
                     FabricPrice = group.Sum(s => s.FabricPrice),
                     AccPrice = group.Sum(s => s.AccPrice),
                     FinishedGoodQty = group.Sum(s => s.FinishedGoodQty),
                     FinishedGoodPrice = group.Sum(s => s.FinishedGoodPrice)
                 });

            foreach(var item in querySaldoAkhir.Where(s=> s.FabricUom !=null))
            {
                GarmentLeftoverWarehouseRecapStockReportViewModel garmentLeftover = new GarmentLeftoverWarehouseRecapStockReportViewModel
                {
                    Description = item.Description,
                    FabricQty = item.FabricQty,
                    FabricUom = item.FabricUom,
                    FabricPrice = item.FabricPrice,
                    AccPrice = item.AccPrice,
                    FinishedGoodQty = item.FinishedGoodQty,
                    FinishedGoodUom = item.FinishedGoodUom,
                    FinishedGoodPrice = item.FinishedGoodPrice
                };
                GarmentLeftoverWarehouseRecapStockReportViewModel.Add(garmentLeftover);
            }
            return GarmentLeftoverWarehouseRecapStockReportViewModel.AsQueryable();
        }

        public Tuple<List<GarmentLeftoverWarehouseRecapStockReportViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.Unit);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];
                Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<GarmentLeftoverWarehouseRecapStockReportViewModel> pageable = new Pageable<GarmentLeftoverWarehouseRecapStockReportViewModel>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseRecapStockReportViewModel> Data = pageable.Data.ToList<GarmentLeftoverWarehouseRecapStockReportViewModel>();

            int TotalData = pageable.TotalCount;
            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, offset);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "KETERANGAN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "BARANG JADI", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "BARANG JADI2", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "BARANG JADI3", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "BAHAN BAKU", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "BAHAN BAKU2", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "BAHAN BAKU3", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "ACCESSORIES", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "JUMLAH", DataType = typeof(String) });
            var stream = new MemoryStream();

            int counter = 0;
            result.Rows.Add("",
                    "Qty", "Sat", "Jumlah RP", "Qty", "Sat", "Jumlah RP", "Jumlah RP", "JUMLAH");
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", 0, 0, 0, 0, 0, 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {

                foreach (var item in Query)
                {
                    counter++;
                    //DateTimeOffset date = item.date ?? new DateTime(1970, 1, 1);
                    //string dateString = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(item.Description, item.FinishedGoodQty, item.FinishedGoodUom, item.FinishedGoodPrice, item.FabricQty, item.FabricUom, item.FabricPrice, item.AccPrice, item.AccPrice + item.FabricPrice + item.FinishedGoodPrice);
                }
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet 1");
                    worksheet.Cells["A1"].LoadFromDataTable(result, true);
                    worksheet.Cells["B" + 1 + ":D" + 1 + ""].Merge = true;
                    worksheet.Cells["E" + 1 + ":G" + 1 + ""].Merge = true;

                    worksheet.Cells["A" + 1 + ":I" + 2 + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + 1 + ":I" + 3 + ""].Style.Font.Bold = true;
                    worksheet.Cells["A" + 1 + ":I" + (counter + 2) + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + 1 + ":I" + (counter + 2) + ""].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + 1 + ":I" + (counter + 2) + ""].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + 1 + ":I" + (counter + 2) + ""].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    for (int i = 1; i < counter + 3; i++)
                    {
                        if (worksheet.Cells["A" + i].Value != null)
                        {
                            string _val = worksheet.Cells["A" + i].Value.ToString();

                            if (_val.Contains("TOTAL") || _val.Contains("SALDO AKHIR"))
                            {

                                worksheet.Cells["A" + i + ":R" + i + ""].Style.Font.Bold = true;

                            }
                            if (_val.Contains("PENERIMAAN") || _val.Contains("PENGELUARAN") )
                            {
                                worksheet.Cells["A" + i + ":R" + i + ""].Merge = true;
                                worksheet.Cells["A" + i + ":R" + i + ""].Style.Font.Bold = true;
                            }
                        }
                    }


                    foreach (var cell in worksheet.Cells["B" + 3 + ":B" + (counter + 2) + ""])
                    {
                        cell.Value = Convert.ToDecimal(cell.Value);
                        cell.Style.Numberformat.Format = "#,##0.00";
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    foreach (var cell in worksheet.Cells["D" + 3 + ":D" + (counter + 2) + ""])
                    {
                        cell.Value = Convert.ToDecimal(cell.Value);
                        cell.Style.Numberformat.Format = "#,##0.00";
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    foreach (var cell in worksheet.Cells["E" + 3 + ":E" + (counter + 2) + ""])
                    {
                        cell.Value = Convert.ToDecimal(cell.Value);
                        cell.Style.Numberformat.Format = "#,##0.00";
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    foreach (var cell in worksheet.Cells["G" + 3 + ":I" + (counter + 2) + ""])
                    {
                        cell.Value = Convert.ToDecimal(cell.Value);
                        cell.Style.Numberformat.Format = "#,##0.00";
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    worksheet.Cells["A" + 1 + ":I" + (counter + 2) + ""].AutoFitColumns();

                    package.SaveAs(stream);

                }
              
            }

            return stream;
        }
    }
}