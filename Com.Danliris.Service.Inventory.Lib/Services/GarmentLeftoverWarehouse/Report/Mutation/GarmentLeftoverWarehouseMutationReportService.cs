using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Mutation;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Mutation
{
    public class GarmentLeftoverWarehouseMutationReportService : IGarmentLeftoverWarehouseMutationReportService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseMutationReportService";

        private InventoryDbContext DbContext;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        public GarmentLeftoverWarehouseMutationReportService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));
        }

        public Tuple<List<GarmentLeftoverWarehouseMutationReportViewModel>, int> GetMutation(DateTime? dateFrom, DateTime? dateTo, int page, int size)
        {
            var Query = GetReportQuery(dateFrom, dateTo);

            //Pageable<GarmentLeftoverWarehouseMutationReportViewModel> pageable = new Pageable<GarmentLeftoverWarehouseMutationReportViewModel>(Query, page - 1, size);
            //List<GarmentLeftoverWarehouseMutationReportViewModel> Data = pageable.Data.ToList<GarmentLeftoverWarehouseMutationReportViewModel>();

            int TotalData = Query.Count();

            return Tuple.Create(Query, TotalData);
        }

        public List<GarmentLeftoverWarehouseMutationReportViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo)
        {
            DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
            DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;

            var BalanceDate = DbContext.GarmentLeftoverWarehouseBalanceStocks.OrderByDescending(x => x.BalanceStockDate).Select(x => x.BalanceStockDate).FirstOrDefault();

            var BalanceStock = (from a in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
                                where a._IsDeleted == false && b._IsDeleted == false && a.TypeOfGoods == "BARANG JADI"
                                select new GarmentLeftoverWarehouseMutationReportViewModel
                                {
                                    ClassificationCode = "RJ001",
                                    ClassificationName = "Reject",
                                    SaldoAwal = b.Quantity,
                                    Pemasukan = 0,
                                    Pengeluaran = 0,
                                    Penyesuaian = 0,
                                    Selisih = 0,
                                    SaldoAkhir = 0,
                                    StockOpname = 0,
                                    UnitQtyName = b.UomUnit
                                }).GroupBy(x => new { x.ClassificationCode, x.ClassificationName, x.UnitQtyName }, (key, group) => new GarmentLeftoverWarehouseMutationReportViewModel
                                {
                                    ClassificationCode = key.ClassificationCode,
                                    ClassificationName = key.ClassificationName,
                                    SaldoAwal = group.Sum(x => x.SaldoAwal),
                                    Pemasukan = group.Sum(x => x.Pemasukan),
                                    Pengeluaran = group.Sum(x => x.Pengeluaran),
                                    Penyesuaian = group.Sum(x => x.Penyesuaian),
                                    Selisih = group.Sum(x => x.Selisih),
                                    SaldoAkhir = group.Sum(x => x.SaldoAkhir),
                                    StockOpname = group.Sum(x => x.StockOpname),
                                    UnitQtyName = key.UnitQtyName

                                });

            var SAReceiptBarangJadi = (from a in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoods
                                       join b in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoodItems on a.Id equals b.FinishedGoodReceiptId
                                       where a._IsDeleted == false && b._IsDeleted == false
                                       && a._CreatedUtc > BalanceDate
                                       && a._CreatedUtc < DateFrom
                                       select new GarmentLeftoverWarehouseMutationReportViewModel
                                       {
                                           ClassificationCode = "RJ001",
                                           ClassificationName = "Reject",
                                           SaldoAwal = b.Quantity,
                                           Pemasukan = 0,
                                           Pengeluaran = 0,
                                           Penyesuaian = 0,
                                           Selisih = 0,
                                           SaldoAkhir = 0,
                                           StockOpname = 0,
                                           UnitQtyName = b.UomUnit
                                       }).GroupBy(x => new { x.ClassificationCode, x.ClassificationName, x.UnitQtyName }, (key, group) => new GarmentLeftoverWarehouseMutationReportViewModel
                                       {
                                           ClassificationCode = key.ClassificationCode,
                                           ClassificationName = key.ClassificationName,
                                           SaldoAwal = group.Sum(x => x.SaldoAwal),
                                           Pemasukan = group.Sum(x => x.Pemasukan),
                                           Pengeluaran = group.Sum(x => x.Pengeluaran),
                                           Penyesuaian = group.Sum(x => x.Penyesuaian),
                                           Selisih = group.Sum(x => x.Selisih),
                                           SaldoAkhir = group.Sum(x => x.SaldoAkhir),
                                           StockOpname = group.Sum(x => x.StockOpname),
                                           UnitQtyName = key.UnitQtyName

                                       });

            var SAReceiptAval = (from a in DbContext.GarmentLeftoverWarehouseReceiptAvals
                                 join b in DbContext.GarmentLeftoverWarehouseReceiptAvalItems on a.Id equals b.AvalReceiptId
                                 where a._IsDeleted == false && b._IsDeleted == false
                                 && a._CreatedUtc > BalanceDate
                                 && a._CreatedUtc < DateFrom
                                 select new GarmentLeftoverWarehouseMutationReportViewModel
                                 {
                                     ClassificationCode = a.AvalType == "AVAL FABRIC" ? "AV001" : a.AvalType == "AVAL BAHAN PENOLONG" ? "AV004" : "AV002",
                                     ClassificationName = a.AvalType == "AVAL FABRIC" ? "Aval Besar" : a.AvalType == "AVAL BAHAN PENOLONG" ? "Aval Bahan Penolong" : "Aval Komponen",
                                     SaldoAwal = b.Quantity,
                                     Pemasukan = 0,
                                     Pengeluaran = 0,
                                     Penyesuaian = 0,
                                     Selisih = 0,
                                     SaldoAkhir = 0,
                                     StockOpname = 0,
                                     UnitQtyName = b.UomUnit
                                 }).GroupBy(x => new { x.ClassificationCode, x.ClassificationName, x.UnitQtyName }, (key, group) => new GarmentLeftoverWarehouseMutationReportViewModel
                                 {
                                     ClassificationCode = key.ClassificationCode,
                                     ClassificationName = key.ClassificationName,
                                     SaldoAwal = group.Sum(x => x.SaldoAwal),
                                     Pemasukan = group.Sum(x => x.Pemasukan),
                                     Pengeluaran = group.Sum(x => x.Pengeluaran),
                                     Penyesuaian = group.Sum(x => x.Penyesuaian),
                                     Selisih = group.Sum(x => x.Selisih),
                                     SaldoAkhir = group.Sum(x => x.SaldoAkhir),
                                     StockOpname = group.Sum(x => x.StockOpname),
                                     UnitQtyName = key.UnitQtyName

                                 });

            var SAExpendBarangJadi = (from a in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoods
                                      join b in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoodItems on a.Id equals b.FinishedGoodExpenditureId
                                      where a._IsDeleted == false && b._IsDeleted == false
                                      && a._CreatedUtc > BalanceDate
                                      && a._CreatedUtc < DateFrom
                                      select new GarmentLeftoverWarehouseMutationReportViewModel
                                      {
                                          ClassificationCode = "RJ001",
                                          ClassificationName = "Reject",
                                          SaldoAwal = b.ExpenditureQuantity * (-1),
                                          Pemasukan = 0,
                                          Pengeluaran = 0,
                                          Penyesuaian = 0,
                                          Selisih = 0,
                                          SaldoAkhir = 0,
                                          StockOpname = 0,
                                          UnitQtyName = "PCS"
                                      }).GroupBy(x => new { x.ClassificationCode, x.ClassificationName, x.UnitQtyName }, (key, group) => new GarmentLeftoverWarehouseMutationReportViewModel
                                      {
                                          ClassificationCode = key.ClassificationCode,
                                          ClassificationName = key.ClassificationName,
                                          SaldoAwal = group.Sum(x => x.SaldoAwal),
                                          Pemasukan = group.Sum(x => x.Pemasukan),
                                          Pengeluaran = group.Sum(x => x.Pengeluaran),
                                          Penyesuaian = group.Sum(x => x.Penyesuaian),
                                          Selisih = group.Sum(x => x.Selisih),
                                          SaldoAkhir = group.Sum(x => x.SaldoAkhir),
                                          StockOpname = group.Sum(x => x.StockOpname),
                                          UnitQtyName = key.UnitQtyName

                                      });

            var SAExpendAval = (from a in DbContext.GarmentLeftoverWarehouseExpenditureAvals
                                join b in DbContext.GarmentLeftoverWarehouseExpenditureAvalItems on a.Id equals b.AvalReceiptId
                                where a._IsDeleted == false && b._IsDeleted == false
                                && a._CreatedUtc > BalanceDate
                                && a._CreatedUtc < DateFrom
                                select new GarmentLeftoverWarehouseMutationReportViewModel
                                {
                                    ClassificationCode = a.AvalType == "AVAL FABRIC" ? "AV001" : a.AvalType == "AVAL BAHAN PENOLONG" ? "AV004" : "AV002",
                                    ClassificationName = a.AvalType == "AVAL FABRIC" ? "Aval Besar" : a.AvalType == "AVAL BAHAN PENOLONG" ? "Aval Bahan Penolong" : "Aval Komponen",
                                    SaldoAwal = b.Quantity * (-1),
                                    Pemasukan = 0,
                                    Pengeluaran = 0,
                                    Penyesuaian = 0,
                                    Selisih = 0,
                                    SaldoAkhir = 0,
                                    StockOpname = 0,
                                    UnitQtyName = b.UomUnit
                                }).GroupBy(x => new { x.ClassificationCode, x.ClassificationName, x.UnitQtyName }, (key, group) => new GarmentLeftoverWarehouseMutationReportViewModel
                                {
                                    ClassificationCode = key.ClassificationCode,
                                    ClassificationName = key.ClassificationName,
                                    SaldoAwal = group.Sum(x => x.SaldoAwal),
                                    Pemasukan = group.Sum(x => x.Pemasukan),
                                    Pengeluaran = group.Sum(x => x.Pengeluaran),
                                    Penyesuaian = group.Sum(x => x.Penyesuaian),
                                    Selisih = group.Sum(x => x.Selisih),
                                    SaldoAkhir = group.Sum(x => x.SaldoAkhir),
                                    StockOpname = group.Sum(x => x.StockOpname),
                                    UnitQtyName = key.UnitQtyName

                                });

            var SAwal = BalanceStock.Concat(SAReceiptBarangJadi).Concat(SAReceiptAval).Concat(SAExpendAval).Concat(SAExpendBarangJadi).AsEnumerable();
            var SaldoAwal = SAwal.GroupBy(x => new { x.ClassificationCode, x.ClassificationName, x.UnitQtyName }, (key, group) => new GarmentLeftoverWarehouseMutationReportViewModel
            {
                ClassificationCode = key.ClassificationCode,
                ClassificationName = key.ClassificationName,
                SaldoAwal = group.Sum(x => x.SaldoAwal),
                Pemasukan = group.Sum(x => x.Pemasukan),
                Pengeluaran = group.Sum(x => x.Pengeluaran),
                Penyesuaian = group.Sum(x => x.Penyesuaian),
                Selisih = group.Sum(x => x.Selisih),
                SaldoAkhir = group.Sum(x => x.SaldoAkhir),
                StockOpname = group.Sum(x => x.StockOpname),
                UnitQtyName = key.UnitQtyName

            }).ToList();

            var FilteredReceiptBarangJadi = (from a in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoods
                                             join b in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoodItems on a.Id equals b.FinishedGoodReceiptId
                                             where a._IsDeleted == false && b._IsDeleted == false
                                             && a._CreatedUtc >= DateFrom
                                             && a._CreatedUtc <= DateTo
                                             select new GarmentLeftoverWarehouseMutationReportViewModel
                                             {
                                                 ClassificationCode = "RJ001",
                                                 ClassificationName = "Reject",
                                                 SaldoAwal = 0,
                                                 Pemasukan = b.Quantity,
                                                 Pengeluaran = 0,
                                                 Penyesuaian = 0,
                                                 Selisih = 0,
                                                 SaldoAkhir = 0,
                                                 StockOpname = 0,
                                                 UnitQtyName = "PCS"
                                             }).GroupBy(x => new { x.ClassificationCode, x.ClassificationName, x.UnitQtyName }, (key, group) => new GarmentLeftoverWarehouseMutationReportViewModel
                                             {
                                                 ClassificationCode = key.ClassificationCode,
                                                 ClassificationName = key.ClassificationName,
                                                 SaldoAwal = group.Sum(x => x.SaldoAwal),
                                                 Pemasukan = group.Sum(x => x.Pemasukan),
                                                 Pengeluaran = group.Sum(x => x.Pengeluaran),
                                                 Penyesuaian = group.Sum(x => x.Penyesuaian),
                                                 Selisih = group.Sum(x => x.Selisih),
                                                 SaldoAkhir = group.Sum(x => x.SaldoAkhir),
                                                 StockOpname = group.Sum(x => x.StockOpname),
                                                 UnitQtyName = key.UnitQtyName

                                             });

            var FilteredReceiptAval = (from a in DbContext.GarmentLeftoverWarehouseReceiptAvals
                                       join b in DbContext.GarmentLeftoverWarehouseReceiptAvalItems on a.Id equals b.AvalReceiptId
                                       where a._IsDeleted == false && b._IsDeleted == false
                                       && a._CreatedUtc >= DateFrom
                                       && a._CreatedUtc <= DateTo
                                       select new GarmentLeftoverWarehouseMutationReportViewModel
                                       {
                                           ClassificationCode = a.AvalType == "AVAL FABRIC" ? "AV001" : a.AvalType == "AVAL BAHAN PENOLONG" ? "AV004" : "AV002",
                                           ClassificationName = a.AvalType == "AVAL FABRIC" ? "Aval Besar" : a.AvalType == "AVAL BAHAN PENOLONG" ? "Aval Bahan Penolong" : "Aval Komponen",
                                           SaldoAwal = 0,
                                           Pemasukan = b.Quantity,
                                           Pengeluaran = 0,
                                           Penyesuaian = 0,
                                           Selisih = 0,
                                           SaldoAkhir = 0,
                                           StockOpname = 0,
                                           UnitQtyName = b.UomUnit
                                       }).GroupBy(x => new { x.ClassificationCode, x.ClassificationName, x.UnitQtyName }, (key, group) => new GarmentLeftoverWarehouseMutationReportViewModel
                                       {
                                           ClassificationCode = key.ClassificationCode,
                                           ClassificationName = key.ClassificationName,
                                           SaldoAwal = group.Sum(x => x.SaldoAwal),
                                           Pemasukan = group.Sum(x => x.Pemasukan),
                                           Pengeluaran = group.Sum(x => x.Pengeluaran),
                                           Penyesuaian = group.Sum(x => x.Penyesuaian),
                                           Selisih = group.Sum(x => x.Selisih),
                                           SaldoAkhir = group.Sum(x => x.SaldoAkhir),
                                           StockOpname = group.Sum(x => x.StockOpname),
                                           UnitQtyName = key.UnitQtyName

                                       });

            var FilteredExpendBarangJadi = (from a in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoods
                                            join b in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoodItems on a.Id equals b.FinishedGoodExpenditureId
                                            where a._IsDeleted == false && b._IsDeleted == false
                                            && a._CreatedUtc >= DateFrom
                                            && a._CreatedUtc <= DateTo
                                            select new GarmentLeftoverWarehouseMutationReportViewModel
                                            {
                                                ClassificationCode = "RJ001",
                                                ClassificationName = "Reject",
                                                SaldoAwal = 0,
                                                Pemasukan = 0,
                                                Pengeluaran = b.ExpenditureQuantity,
                                                Penyesuaian = 0,
                                                Selisih = 0,
                                                SaldoAkhir = 0,
                                                StockOpname = 0,
                                                UnitQtyName = "PCS"
                                            }).GroupBy(x => new { x.ClassificationCode, x.ClassificationName, x.UnitQtyName }, (key, group) => new GarmentLeftoverWarehouseMutationReportViewModel
                                            {
                                                ClassificationCode = key.ClassificationCode,
                                                ClassificationName = key.ClassificationName,
                                                SaldoAwal = group.Sum(x => x.SaldoAwal),
                                                Pemasukan = group.Sum(x => x.Pemasukan),
                                                Pengeluaran = group.Sum(x => x.Pengeluaran),
                                                Penyesuaian = group.Sum(x => x.Penyesuaian),
                                                Selisih = group.Sum(x => x.Selisih),
                                                SaldoAkhir = group.Sum(x => x.SaldoAkhir),
                                                StockOpname = group.Sum(x => x.StockOpname),
                                                UnitQtyName = key.UnitQtyName

                                            });

            var FilteredExpendAval = (from a in DbContext.GarmentLeftoverWarehouseExpenditureAvals
                                      join b in DbContext.GarmentLeftoverWarehouseExpenditureAvalItems on a.Id equals b.AvalReceiptId
                                      where a._IsDeleted == false && b._IsDeleted == false
                                      && a._CreatedUtc >= DateFrom
                                      && a._CreatedUtc <= DateTo
                                      select new GarmentLeftoverWarehouseMutationReportViewModel
                                      {
                                          ClassificationCode = a.AvalType == "AVAL FABRIC" ? "AV001" : a.AvalType == "AVAL BAHAN PENOLONG" ? "AV004" : "AV002",
                                          ClassificationName = a.AvalType == "AVAL FABRIC" ? "Aval Besar" : a.AvalType == "AVAL BAHAN PENOLONG" ? "Aval Bahan Penolong" : "Aval Komponen",
                                          SaldoAwal = 0,
                                          Pemasukan = 0,
                                          Pengeluaran = b.Quantity,
                                          Penyesuaian = 0,
                                          Selisih = 0,
                                          SaldoAkhir = 0,
                                          StockOpname = 0,
                                          UnitQtyName = b.UomUnit
                                      }).GroupBy(x => new { x.ClassificationCode, x.ClassificationName, x.UnitQtyName }, (key, group) => new GarmentLeftoverWarehouseMutationReportViewModel
                                      {
                                          ClassificationCode = key.ClassificationCode,
                                          ClassificationName = key.ClassificationName,
                                          SaldoAwal = group.Sum(x => x.SaldoAwal),
                                          Pemasukan = group.Sum(x => x.Pemasukan),
                                          Pengeluaran = group.Sum(x => x.Pengeluaran),
                                          Penyesuaian = group.Sum(x => x.Penyesuaian),
                                          Selisih = group.Sum(x => x.Selisih),
                                          SaldoAkhir = group.Sum(x => x.SaldoAkhir),
                                          StockOpname = group.Sum(x => x.StockOpname),
                                          UnitQtyName = key.UnitQtyName

                                      });
            var SAkhir = SaldoAwal.Concat(FilteredReceiptAval).Concat(FilteredReceiptBarangJadi).Concat(FilteredExpendAval).Concat(FilteredExpendBarangJadi).AsEnumerable();
            var SaldoAkhir = SAkhir.GroupBy(x => new { x.ClassificationCode, x.ClassificationName, x.UnitQtyName }, (key, group) => new GarmentLeftoverWarehouseMutationReportViewModel
            {
                ClassificationCode = key.ClassificationCode,
                ClassificationName = key.ClassificationName,
                SaldoAwal = group.Sum(x => x.SaldoAwal),
                Pemasukan = group.Sum(x => x.Pemasukan),
                Pengeluaran = group.Sum(x => x.Pengeluaran),
                Penyesuaian = group.Sum(x => x.Penyesuaian),
                Selisih = group.Sum(x => x.Selisih),
                SaldoAkhir = group.Sum(x => x.SaldoAwal) + group.Sum(x => x.Pemasukan) - group.Sum(x => x.Pengeluaran),
                StockOpname = group.Sum(x => x.StockOpname),
                UnitQtyName = key.UnitQtyName

            }).ToList();

            var mutationScrap = GetScrap(DateFrom.Date, DateTo.Date);

            if (mutationScrap.Count == 0)
            {
                mutationScrap.Add(new GarmentLeftoverWarehouseMutationReportViewModel
                {
                    ClassificationCode = "AVP01",
                    ClassificationName = "Aval Tc Kecil",
                    SaldoAwal = 0,
                    Pemasukan = 0,
                    Pengeluaran = 0,
                    Penyesuaian = 0,
                    Selisih = 0,
                    SaldoAkhir = 0,
                    StockOpname = 0,
                    UnitQtyName = "KG"
                });
            }

            if (SaldoAkhir.FirstOrDefault(x => x.ClassificationName == "Aval Komponen") == null) {
                SaldoAkhir.Add(new GarmentLeftoverWarehouseMutationReportViewModel
                {
                    ClassificationCode = "AV002",
                    ClassificationName = "Aval Komponen",
                    SaldoAwal = 0,
                    Pemasukan = 0,
                    Pengeluaran = 0,
                    Penyesuaian = 0,
                    Selisih = 0,
                    SaldoAkhir = 0,
                    StockOpname = 0,
                    UnitQtyName = "KG"
                });
            }
            if (SaldoAkhir.FirstOrDefault(x => x.ClassificationName == "Aval Bahan Penolong") == null)
            {
                SaldoAkhir.Add(new GarmentLeftoverWarehouseMutationReportViewModel
                {
                    ClassificationCode = "AV004",
                    ClassificationName = "Aval Bahan Penolong",
                    SaldoAwal = 0,
                    Pemasukan = 0,
                    Pengeluaran = 0,
                    Penyesuaian = 0,
                    Selisih = 0,
                    SaldoAkhir = 0,
                    StockOpname = 0,
                    UnitQtyName = "KG"
                });
            }
            if (SaldoAkhir.FirstOrDefault(x => x.ClassificationName == "Aval Besar") == null)
            {
                SaldoAkhir.Add(new GarmentLeftoverWarehouseMutationReportViewModel
                {
                    ClassificationCode = "AV001",
                    ClassificationName = "Aval Besar",
                    SaldoAwal = 0,
                    Pemasukan = 0,
                    Pengeluaran = 0,
                    Penyesuaian = 0,
                    Selisih = 0,
                    SaldoAkhir = 0,
                    StockOpname = 0,
                    UnitQtyName = "KG"
                });
            }
            if (SaldoAkhir.FirstOrDefault(x => x.ClassificationName == "Reject") == null)
            {
                SaldoAkhir.Add(new GarmentLeftoverWarehouseMutationReportViewModel
                {
                    ClassificationCode = "RJ001",
                    ClassificationName = "Reject",
                    SaldoAwal = 0,
                    Pemasukan = 0,
                    Pengeluaran = 0,
                    Penyesuaian = 0,
                    Selisih = 0,
                    SaldoAkhir = 0,
                    StockOpname = 0,
                    UnitQtyName = "KG"
                });
            };

            var mutation = SaldoAkhir.Concat(mutationScrap).ToList();

            return mutation.OrderBy(x => x.ClassificationCode).ToList();


        }


        private List<GarmentLeftoverWarehouseMutationReportViewModel> GetScrap(DateTime datefrom, DateTime dateTo)
        {
            IHttpService httpClient = (IHttpService)this.ServiceProvider.GetService(typeof(IHttpService));

            var garmentProductionUri = APIEndpoint.GarmentProduction + $"scrap-transactions/mutation";
            string queryUri = "?dateFrom=" + datefrom + "&dateTo=" + dateTo;
            string uri = garmentProductionUri + queryUri;
            var httpResponse = httpClient.GetAsync($"{uri}").Result;

            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<GarmentLeftoverWarehouseMutationReportViewModel> viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = null;
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<List<GarmentLeftoverWarehouseMutationReportViewModel>>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                List<GarmentLeftoverWarehouseMutationReportViewModel> viewModel = new List<GarmentLeftoverWarehouseMutationReportViewModel>();
                return viewModel;
            }
        }

        public MemoryStream GenerateExcelMutation(DateTime dateFrom, DateTime dateTo)
        {
            var Query = GetReportQuery(dateFrom, dateTo);
            DataTable Result = new DataTable();
            Result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(Double) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Pemasukan", DataType = typeof(Double) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Pengeluaran", DataType = typeof(Double) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Penyesuaian", DataType = typeof(Double) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Saldo Akhir", DataType = typeof(Double) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Stock Opname", DataType = typeof(Double) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Selisih", DataType = typeof(Double) });
            if (Query.ToArray().Count() == 0)
                Result.Rows.Add("", "", "", 0, 0, 0, 0, 0, 0, 0);
            else
                foreach (var item in Query)
                {
                    Result.Rows.Add(item.ClassificationCode, item.ClassificationName, item.UnitQtyName, item.SaldoAwal, item.Pemasukan, item.Pengeluaran, item.Penyesuaian, item.SaldoAkhir, item.StockOpname, item.Selisih);
                }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(Result, "ScrapReject") }, true);
        }

    }
}
