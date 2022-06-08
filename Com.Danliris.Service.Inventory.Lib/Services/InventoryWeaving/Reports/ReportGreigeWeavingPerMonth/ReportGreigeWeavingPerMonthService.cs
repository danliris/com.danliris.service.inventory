using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportGreigeWeavingPerMonth
{
    public class ReportGreigeWeavingPerMonthService : IReportGreigeWeavingPerMonthService
    {

        //private readonly InventoryDbContext dbContext;
        //public readonly IServiceProvider serviceProvider;
        //private readonly DbSet<InventoryWeavingMovement> DbSetMovement;

        //public ReportGreigeWeavingPerMonthService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        //{
        //    this.serviceProvider = serviceProvider;
        //    this.dbContext = dbContext;
        //    this.DbSetMovement = dbContext.Set<InventoryWeavingMovement>();
        //}

        private string USER_AGENT = "Service";
        private const string UserAgent = "inventory-service";
        protected DbSet<InventoryWeavingDocumentItem> DbSetItem;
        protected DbSet<InventoryWeavingMovement> DbSetMovement;
        protected DbSet<InventoryWeavingDocument> DbSetDoc;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;


        public ReportGreigeWeavingPerMonthService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSetItem = dbContext.Set<InventoryWeavingDocumentItem>();
            DbSetMovement = dbContext.Set<InventoryWeavingMovement>();
            DbSetDoc = dbContext.Set<InventoryWeavingDocument>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
            //IdentityService = serviceProvider.GetService<IIdentityService>();
        }


        public Tuple<List<ReportGreigeWeavingPerMonthViewModel>, int> GetStockReportGreige(DateTime? dateTo, int offset, int page, int size, string Order)
        {

            List<ReportGreigeWeavingPerMonthViewModel> Query =  GetStockQuery(dateTo, offset);
            //Query = Query.Where(x => (x.BeginningBalanceQty != 0) || (x.BeginningBalancePrice != 0) || (x.EndingBalancePrice > 0) || (x.EndingBalanceQty > 0) || (x.ExpendKon1APrice > 0) || (x.ExpendKon1AQty > 0) ||
            //(x.ExpendKon1BPrice > 0) || (x.ExpendKon1BQty > 0) || (x.ExpendKon2APrice > 0) || (x.ExpendKon2AQty > 0) || (x.ExpendKon2BPrice > 0) || (x.ExpendKon2BQty > 0) || (x.ExpendKon2CPrice > 0) || (x.ExpendKon2CQty > 0) ||
            //(x.ExpendProcessPrice > 0) || (x.ExpendProcessQty > 0) || (x.ExpendRestPrice > 0) || (x.ExpendRestQty > 0) || (x.ExpendReturPrice > 0) || (x.ExpendReturQty > 0) || (x.ExpendSamplePrice > 0) || (x.ExpendSampleQty > 0) ||
            //(x.ReceiptCorrectionPrice > 0) || (x.ReceiptCorrectionQty > 0) || (x.ReceiptKon1APrice > 0) || (x.ReceiptKon1AQty > 0) || (x.ReceiptKon1BPrice > 0) || (x.ReceiptKon1BQty > 0) || (x.ReceiptKon2APrice > 0) || (x.ReceiptKon2AQty > 0)
            //|| (x.ReceiptKon2BPrice > 0) || (x.ReceiptKon2BQty > 0) || (x.ReceiptKon2CPrice > 0) || (x.ReceiptKon2CQty > 0) || (x.ReceiptProcessPrice > 0) || (x.ReceiptProcessQty > 0) || (x.ReceiptPurchasePrice > 0) || (x.ReceiptPurchaseQty > 0)).ToList();

            //Query = Query.OrderBy(x => x.ProductCode).ThenBy(x => x.PlanPo).ToList();
            Pageable<ReportGreigeWeavingPerMonthViewModel> pageable = new Pageable<ReportGreigeWeavingPerMonthViewModel>(Query, page - 1, size);
            List<ReportGreigeWeavingPerMonthViewModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;
            //int TotalData = Data.Count();
            return Tuple.Create(Data, TotalData);
        }

        //public async Task<Tuple<List<ReportGreigeWeavingAllDataViewModel>, int>> GetStockReportGreige(DateTime? dateTo, int offset, int page, int size, string Order)
        //{

        //    List<ReportGreigeWeavingAllDataViewModel> Query = await GetStockQuery(dateTo, offset);
        //    //Query = Query.Where(x => (x.BeginningBalanceQty != 0) || (x.BeginningBalancePrice != 0) || (x.EndingBalancePrice > 0) || (x.EndingBalanceQty > 0) || (x.ExpendKon1APrice > 0) || (x.ExpendKon1AQty > 0) ||
        //    //(x.ExpendKon1BPrice > 0) || (x.ExpendKon1BQty > 0) || (x.ExpendKon2APrice > 0) || (x.ExpendKon2AQty > 0) || (x.ExpendKon2BPrice > 0) || (x.ExpendKon2BQty > 0) || (x.ExpendKon2CPrice > 0) || (x.ExpendKon2CQty > 0) ||
        //    //(x.ExpendProcessPrice > 0) || (x.ExpendProcessQty > 0) || (x.ExpendRestPrice > 0) || (x.ExpendRestQty > 0) || (x.ExpendReturPrice > 0) || (x.ExpendReturQty > 0) || (x.ExpendSamplePrice > 0) || (x.ExpendSampleQty > 0) ||
        //    //(x.ReceiptCorrectionPrice > 0) || (x.ReceiptCorrectionQty > 0) || (x.ReceiptKon1APrice > 0) || (x.ReceiptKon1AQty > 0) || (x.ReceiptKon1BPrice > 0) || (x.ReceiptKon1BQty > 0) || (x.ReceiptKon2APrice > 0) || (x.ReceiptKon2AQty > 0)
        //    //|| (x.ReceiptKon2BPrice > 0) || (x.ReceiptKon2BQty > 0) || (x.ReceiptKon2CPrice > 0) || (x.ReceiptKon2CQty > 0) || (x.ReceiptProcessPrice > 0) || (x.ReceiptProcessQty > 0) || (x.ReceiptPurchasePrice > 0) || (x.ReceiptPurchaseQty > 0)).ToList();

        //    //Query = Query.OrderBy(x => x.ProductCode).ThenBy(x => x.PlanPo).ToList();
        //    Pageable<ReportGreigeWeavingAllDataViewModel> pageable = new Pageable<ReportGreigeWeavingAllDataViewModel>(Query, page - 1, size);
        //    List<ReportGreigeWeavingAllDataViewModel> Data = pageable.Data.ToList();
        //    int TotalData = pageable.TotalCount;
        //    //int TotalData = Data.Count();
        //    return Tuple.Create(Data, TotalData);
        //}

        public List<ReportGreigeWeavingPerMonthViewModel> GetStockQuery( DateTime? dateto, int offset)
        {
            DateTime dateReport = dateto == null ? DateTime.Now : (DateTime)dateto;
            var startDate = new DateTime(dateReport.Year, dateReport.Month, 1);


            var QueryAll = ( from  a in DbSetMovement
                             join c in DbSetDoc on a.InventoryWeavingDocumentId equals c.Id into m
                             from b in m.DefaultIfEmpty()
                             where
                             a._IsDeleted == false
                             && b._IsDeleted == false
                             //&& a.Construction.Contains("C D    120  60  63  RF  RF  B  B")

                             select new ReportGreigeWeavingAllDataViewModel {
                                 Date = b.Date,
                                 _CreatedUtc = a._CreatedUtc,
                                 Construction = a.Construction,
                                 MaterialName = a.MaterialName,
                                 WovenType = a.WovenType,
                                 Yarn1 = a.Yarn1,
                                 Yarn2 = a.Yarn2,
                                 Width = a.Width,
                                 YarnOrigin1 = a.YarnOrigin1,
                                 YarnOrigin2 = a.YarnOrigin2,
                                 YarnType1 = a.YarnType1,
                                 YarnType2 = a.YarnType2,
                                 Area = a.DestinationArea,
                                 Quantity = a.Quantity, //!= null ? a.Quantity: 0,
                                 Type = a.Type
                                 
                                 
                                 
                             }
                             
                
                );

            var TerimaAll = QueryAll.Where(d => d.Date.AddHours(offset).Date >= startDate.Date && d.Date.AddHours(offset).Date <= dateReport.Date && d.Type == "IN").Select(x => new ReportGreigeWeavingPerMonthViewModel
            {
                Construction = x.Construction,
                MaterialName = x.MaterialName,
                WovenType = x.WovenType,
                Yarn1 = x.Yarn1,
                Yarn2 = x.Yarn2,
                Width = x.Width,
                YarnOrigin1 = x.YarnOrigin1,
                YarnOrigin2 = x.YarnOrigin2,
                YarnType1 = x.YarnType1,
                YarnType2 = x.YarnType2,
                ReceiptProduction = x.Area == "PRODUKSI" && x.Type == "IN" ? x.Quantity : 0,
                ReceiptFinishing = x.Area == "FINISHING" && x.Type == "IN" ? x.Quantity : 0,
                ReceiptRecheking = x.Area == "RECHEKING" && x.Type == "IN" ? x.Quantity : 0,
                ReceiptPacking = x.Area == "PACKING" && x.Type == "IN" ? x.Quantity : 0,
                ReceiptPrinting = x.Area == "PRINTING" && x.Type == "IN" ? x.Quantity : 0,
                ReceiptOther = x.Area == "LAIN-LAIN" && x.Type == "IN" ? x.Quantity : 0,
                ExpendInspecting =0,
                ExpendFinishing =  0,
                ExpendPacking =  0,
                ExpendPrinting = 0,
                ExpendDirty =  0,
                ExpendOther =  0,
                BeginingBalance = 0,
                EndingBalance = 0,
            });


            var Terima = TerimaAll.GroupBy(s => new {
                s.MaterialName,
                s.WovenType,
                s.Yarn1,
                s.Yarn2,
                s.Width,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2

            }).Select(x => new ReportGreigeWeavingPerMonthViewModel
            {
                Construction = x.FirstOrDefault().Construction,
                MaterialName = x.Key.MaterialName,
                WovenType = x.Key.WovenType,
                Yarn1 = x.Key.Yarn1,
                Yarn2 = x.Key.Yarn2,
                Width = x.Key.Width,
                YarnOrigin1 = x.Key.YarnOrigin1,
                YarnOrigin2 = x.Key.YarnOrigin2,
                YarnType1 = x.Key.YarnType1,
                YarnType2 = x.Key.YarnType2,
                ReceiptProduction = x.Sum( f => f.ReceiptProduction),
                ReceiptFinishing = x.Sum(f => f.ReceiptFinishing),
                ReceiptRecheking = x.Sum(f => f.ReceiptRecheking),
                ReceiptPacking = x.Sum(f => f.ReceiptPacking),
                ReceiptPrinting = x.Sum(f => f.ReceiptPrinting),
                ReceiptOther = x.Sum(f => f.ReceiptOther),
                ExpendInspecting = 0,
                ExpendFinishing =  0,
                ExpendPacking =  0,
                ExpendPrinting =  0,
                ExpendDirty =  0,
                ExpendOther =0,
                BeginingBalance = 0,
                EndingBalance = 0,
            });

            var KeluarAll = QueryAll.Where(d => d.Date.AddHours(offset).Date >= startDate.Date && d.Date.AddHours(offset).Date <= dateReport.Date && d.Type == "OUT").Select(x => new ReportGreigeWeavingPerMonthViewModel
            {
                Construction = x.Construction,
                MaterialName = x.MaterialName,
                WovenType = x.WovenType,
                Yarn1 = x.Yarn1,
                Yarn2 = x.Yarn2,
                Width = x.Width,
                YarnOrigin1 = x.YarnOrigin1,
                YarnOrigin2 = x.YarnOrigin2,
                YarnType1 = x.YarnType1,
                YarnType2 = x.YarnType2,
                ReceiptProduction = 0,
                ReceiptFinishing = 0,
                ReceiptRecheking = 0,
                ReceiptPacking = 0,
                ReceiptPrinting = 0,
                ReceiptOther = 0,
                ExpendInspecting = x.Area == "INSPECTING WEAVING" && x.Type == "OUT" ? x.Quantity : 0,
                ExpendFinishing = x.Area == "FINISHING" && x.Type == "OUT" ? x.Quantity : 0,
                ExpendPacking = x.Area == "PACKING" && x.Type == "OUT" ? x.Quantity : 0,
                ExpendPrinting = x.Area == "PRINTING" && x.Type == "OUT" ? x.Quantity : 0,
                ExpendDirty = x.Area == "KOTOR" && x.Type == "OUT" ? x.Quantity : 0,
                ExpendOther = x.Area == "LAIN-LAIN" && x.Type == "OUT" ? x.Quantity : 0,
                BeginingBalance = 0,
                EndingBalance = 0,
            });



            var Keluar = KeluarAll.GroupBy(s => new {
                s.MaterialName,
                s.WovenType,
                s.Yarn1,
                s.Yarn2,
                s.Width,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2


            }).Select(x => new ReportGreigeWeavingPerMonthViewModel
            {
                Construction = x.FirstOrDefault().Construction,
                MaterialName = x.Key.MaterialName,
                WovenType = x.Key.WovenType,
                Yarn1 = x.Key.Yarn1,
                Yarn2 = x.Key.Yarn2,
                Width = x.Key.Width,
                YarnOrigin1 = x.Key.YarnOrigin1,
                YarnOrigin2 = x.Key.YarnOrigin2,
                YarnType1 = x.Key.YarnType1,
                YarnType2 = x.Key.YarnType2,
                ReceiptProduction = 0,
                ReceiptFinishing =  0,
                ReceiptRecheking =  0,
                ReceiptPacking =  0,
                ReceiptPrinting =  0,
                ReceiptOther =  0,
                ExpendInspecting =  x.Sum( f => f.ExpendInspecting),
                ExpendFinishing = x.Sum(f => f.ExpendFinishing),
                ExpendPacking = x.Sum(f => f.ExpendPacking),
                ExpendPrinting = x.Sum(f => f.ExpendPrinting),
                ExpendDirty = x.Sum(f => f.ExpendDirty),
                ExpendOther = x.Sum(f => f.ExpendOther),
                BeginingBalance = 0,
                EndingBalance = 0,
            });


            var SaldoAllAwal = QueryAll.Where(d => d.Date.AddHours(offset).Date < startDate.Date).GroupBy(s => new
            {
                s.MaterialName,
                s.WovenType,
                s.Yarn1,
                s.Yarn2,
                s.Width,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2,
                s.Type

            }).Select(x => new ReportGreigeWeavingAllDataViewModel
            {
                _CreatedUtc = x.FirstOrDefault()._CreatedUtc,
                Construction = x.FirstOrDefault().Construction,
                MaterialName = x.Key.MaterialName,
                WovenType = x.Key.WovenType,
                Yarn1 = x.Key.Yarn1,
                Yarn2 = x.Key.Yarn2,
                Width = x.Key.Width,
                YarnOrigin1 = x.Key.YarnOrigin1,
                YarnOrigin2 = x.Key.YarnOrigin2,
                YarnType1 = x.Key.YarnType1,
                YarnType2 = x.Key.YarnType2,
                Area = x.FirstOrDefault().Area,
                Quantity = x.Sum(d => d.Quantity),
                Type = x.Key.Type


            });

            var SaldoAllAkhir = QueryAll.Where(d => d.Date.AddHours(offset).Date <= dateReport.Date).GroupBy(s => new
            {
                s.MaterialName,
                s.WovenType,
                s.Yarn1,
                s.Yarn2,
                s.Width,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2,
                s.Type

            }).Select(x => new ReportGreigeWeavingAllDataViewModel
            {
                _CreatedUtc = x.FirstOrDefault()._CreatedUtc,
                Construction = x.FirstOrDefault().Construction,
                MaterialName = x.Key.MaterialName,
                WovenType = x.Key.WovenType,
                Yarn1 = x.Key.Yarn1,
                Yarn2 = x.Key.Yarn2,
                Width = x.Key.Width,
                YarnOrigin1 = x.Key.YarnOrigin1,
                YarnOrigin2 = x.Key.YarnOrigin2,
                YarnType1 = x.Key.YarnType1,
                YarnType2 = x.Key.YarnType2,
                Area = x.FirstOrDefault().Area,
                Quantity = x.Sum(d => d.Quantity),
                Type = x.Key.Type


            });

            var SaldoAwal = SaldoAllAwal.GroupBy(s => new {
                s.MaterialName,
                s.WovenType,
                s.Yarn1,
                s.Yarn2,
                s.Width,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2

            }).Select(x => new ReportGreigeWeavingPerMonthViewModel {
                Construction = x.FirstOrDefault().Construction,
                MaterialName = x.Key.MaterialName,
                WovenType = x.Key.WovenType,
                Yarn1 = x.Key.Yarn1,
                Yarn2 = x.Key.Yarn2,
                Width = x.Key.Width,
                YarnOrigin1 = x.Key.YarnOrigin1,
                YarnOrigin2 = x.Key.YarnOrigin2,
                YarnType1 = x.Key.YarnType1,
                YarnType2 = x.Key.YarnType2,
                ReceiptProduction = 0,
                ReceiptFinishing = 0,
                ReceiptRecheking = 0,
                ReceiptPacking = 0,
                ReceiptPrinting = 0,
                ReceiptOther = 0,
                ExpendInspecting = 0,
                ExpendFinishing = 0,
                ExpendPacking = 0,
                ExpendPrinting = 0,
                ExpendDirty = 0,
                ExpendOther = 0,
                BeginingBalance = x.FirstOrDefault(d => d.Type == "OUT") != null ? x.FirstOrDefault(d => d.Type == "IN").Quantity - x.FirstOrDefault(d => d.Type == "OUT").Quantity :
                                 x.FirstOrDefault(d => d.Type == "OUT") == null ? x.FirstOrDefault(d => d.Type == "IN").Quantity : 0,
                EndingBalance = 0,
            });



           var SaldoAkhir = SaldoAllAkhir.GroupBy(s => new {
               s.MaterialName,
               s.WovenType,
               s.Yarn1,
               s.Yarn2,
               s.Width,
               s.YarnOrigin1,
               s.YarnOrigin2,
               s.YarnType1,
               s.YarnType2

           }).Select(x => new ReportGreigeWeavingPerMonthViewModel
           {
               Construction = x.FirstOrDefault().Construction,
               MaterialName = x.Key.MaterialName,
               WovenType = x.Key.WovenType,
               Yarn1 = x.Key.Yarn1,
               Yarn2 = x.Key.Yarn2,
               Width = x.Key.Width,
               YarnOrigin1 = x.Key.YarnOrigin1,
               YarnOrigin2 = x.Key.YarnOrigin2,
               YarnType1 = x.Key.YarnType1,
               YarnType2 = x.Key.YarnType2,
               ReceiptRecheking = 0,
               ReceiptFinishing = 0,
               ReceiptProduction = 0,
               ReceiptPacking = 0,
               ReceiptPrinting = 0,
               ReceiptOther = 0,
               ExpendInspecting = 0,
               ExpendFinishing = 0,
               ExpendPacking = 0,
               ExpendPrinting = 0,
               ExpendDirty = 0,
               ExpendOther = 0,
               BeginingBalance = 0,
               EndingBalance = x.FirstOrDefault(d => d.Type == "OUT") != null ? x.FirstOrDefault(d => d.Type == "IN").Quantity - x.FirstOrDefault(d => d.Type == "OUT").Quantity :
                                x.FirstOrDefault(d => d.Type == "OUT") == null ? x.FirstOrDefault(d => d.Type == "IN").Quantity : 0,
           });


           // var Stock = Terima.Concat(Keluar).Concat(SaldoAwal).Concat(SaldoAkhir);

            var cTerima = Terima.Count();
            var cKeluar = Keluar.Count();
            var cSaldoAwal = SaldoAwal.Count();
            var cSaldoAkhir = SaldoAkhir.Count();

            var Stock = SaldoAwal.Concat(SaldoAkhir).Concat(Terima).Concat(Keluar).AsEnumerable();

            var DataStock = Stock.GroupBy(s => new
            {
                s.MaterialName,
                s.WovenType,
                s.Yarn1,
                s.Yarn2,
                s.Width,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2
            }, (key, data) => new ReportGreigeWeavingPerMonthViewModel
            {
                Construction = data.FirstOrDefault().Construction,
                MaterialName = key.MaterialName,
                WovenType = key.WovenType,
                Yarn1 = key.Yarn1,
                Yarn2 = key.Yarn2,
                Width = key.Width,
                YarnOrigin1 = key.YarnOrigin1,
                YarnOrigin2 = key.YarnOrigin2,
                YarnType1 = key.YarnType1,
                YarnType2 = key.YarnType2,
                ReceiptRecheking = data.Sum(d => d.ReceiptRecheking),
                ReceiptFinishing = data.Sum(d => d.ReceiptFinishing),
                ReceiptProduction = data.Sum(d => d.ReceiptProduction),
                ReceiptPacking = data.Sum(d => d.ReceiptPacking),
                ReceiptPrinting = data.Sum(d => d.ReceiptPrinting),
                ReceiptOther = data.Sum(d => d.ReceiptOther),
                ExpendInspecting = data.Sum(d => d.ExpendInspecting),
                ExpendFinishing = data.Sum(d => d.ExpendFinishing),
                ExpendPacking = data.Sum(d => d.ExpendPacking),
                ExpendPrinting = data.Sum(d => d.ExpendPrinting),
                ExpendDirty = data.Sum(d => d.ExpendDirty),
                ExpendOther = data.Sum(d => d.ExpendOther),
                BeginingBalance = data.Sum(d => d.BeginingBalance),
                EndingBalance = data.Sum(d => d.EndingBalance)
            });


            List<ReportGreigeWeavingPerMonthViewModel> stockReportViewModels = new List<ReportGreigeWeavingPerMonthViewModel>();
            int index = 1;
            foreach ( var i in DataStock)
            {
                
                stockReportViewModels.Add(new ReportGreigeWeavingPerMonthViewModel
                {
                    Nomor = index++,
                    Construction = i.Construction,
                    MaterialName = i.MaterialName,
                    WovenType = i.WovenType,
                    Yarn1 = i.Yarn1,
                    Yarn2 = i.Yarn2,
                    Width = i.Width,
                    YarnOrigin1 = i.YarnOrigin1,
                    YarnOrigin2 = i.YarnOrigin2,
                    YarnType1 = i.YarnType1,
                    YarnType2 = i.YarnType2,
                    ReceiptRecheking = i.ReceiptRecheking,
                    ReceiptFinishing = i.ReceiptFinishing,
                    ReceiptProduction = i.ReceiptProduction,
                    ReceiptPacking = i.ReceiptPacking,
                    ReceiptPrinting = i.ReceiptPrinting,
                    ReceiptOther = i.ReceiptOther,
                    ExpendInspecting = i.ExpendInspecting,
                    ExpendFinishing = i.ExpendFinishing,
                    ExpendPacking = i.ExpendPacking,
                    ExpendDirty =i.ExpendDirty,
                    ExpendOther = i.ExpendOther,
                    ExpendPrinting = i.ExpendPrinting,
                    BeginingBalance = i.BeginingBalance,
                    EndingBalance = i.EndingBalance

                });

                

            }

            var total = new ReportGreigeWeavingPerMonthViewModel
            {
                Construction = "TOTAL",
                MaterialName = "-",
                WovenType = "-",
                Yarn1 = "-",
                Yarn2 = "-",
                Width = "-",
                YarnOrigin1 = "-",
                YarnOrigin2 = "-",
                YarnType1 = "-",
                YarnType2 = "-",
                ReceiptPacking = Math.Round(DataStock.Sum(X => X.ReceiptPacking), 2),
                ReceiptFinishing = Math.Round(DataStock.Sum(X => X.ReceiptFinishing), 2),
                ReceiptPrinting = Math.Round(DataStock.Sum(X => X.ReceiptPrinting), 2),
                ReceiptProduction = Math.Round(DataStock.Sum(X => X.ReceiptProduction), 2),
                ReceiptRecheking = Math.Round(DataStock.Sum(X => X.ReceiptRecheking), 2),
                ReceiptOther = Math.Round(DataStock.Sum(X => X.ReceiptOther), 2),
                ExpendPrinting = Math.Round(DataStock.Sum(X => X.ExpendPrinting), 2),
                ExpendFinishing = Math.Round(DataStock.Sum(X => X.ExpendFinishing), 2),
                ExpendInspecting = Math.Round(DataStock.Sum(X => X.ExpendInspecting), 2),
                ExpendPacking = Math.Round(DataStock.Sum(X => X.ExpendPacking), 2),
                ExpendOther = Math.Round(DataStock.Sum(X => X.ExpendOther), 2),
                ExpendDirty = Math.Round(DataStock.Sum(X => X.ExpendDirty), 2),
                BeginingBalance = Math.Round(DataStock.Sum(X => X.BeginingBalance), 2),
                EndingBalance = Math.Round(DataStock.Sum(X => X.EndingBalance), 2),

            };


            stockReportViewModels.Add(total);

          return stockReportViewModels;


        }

        public MemoryStream GenerateExcel(DateTime? dateTo, int offset)
        {
            var Query = GetStockQuery(dateTo, offset);
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            string Bulan = DateTo.ToString("MMM yyyy", new CultureInfo("id-ID"));
            string Tanggal = DateTo.ToString("dd MMM yyyy", new CultureInfo("id-ID"));

            DataTable result = new DataTable();
            var headers = new string[] { "No", "Konstruksi", "Saldo Awal", "Pemasukan", "Pemasukan1", "Pemasukan2", "Pemasukan3", "Pemasukan4", "Pemasukan5", "Pengeluaran", "Pengeluaran1", "Pengeluaran2", "Pengeluaran3", "Pengeluaran4", "Pengeluaran5", "Saldo Akhir" };
            var subheaders = new string[] { "Produksi", "Recheking", "Ret. Finish", "Ret. Print", "Ret. Pack", "Lain-lain", "Packing", "Finishing", "Printing", "Inspecting", "Kotor", "Lain-lain" };
            

            result.Columns.Add(new DataColumn() { ColumnName = headers[0], DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[1], DataType = typeof(string) });

            for (int i = 2; i < 16; i++)
            {
                result.Columns.Add(new DataColumn() { ColumnName = headers[i], DataType = typeof(Double) });
            }

            Query.RemoveAt(Query.Count() - 1);

            double TotBeginingBalance = 0;
            double TotReceiptProduction = 0;
            double TotReceiptRecheking = 0;
            double TotReceiptFinishing = 0;
            double TotReceiptPrinting = 0;
            double TotReceiptPacking = 0;
            double TotReceiptOther = 0;
            double TotExpendPacking = 0;
            double TotExpendFinishing = 0;
            double TotExpendPrinting = 0;
            double TotExpendInspecting = 0;
            double TotExpendDirty = 0;
            double TotExpendOther = 0;
            double TotEndingBalance = 0;



            foreach (var item in Query)
            {
                TotBeginingBalance += item.BeginingBalance;
                TotReceiptProduction += item.ReceiptProduction;
                TotReceiptRecheking += item.ReceiptRecheking;
                TotReceiptFinishing += item.ReceiptFinishing;
                TotReceiptPrinting += item.ReceiptPrinting;
                TotReceiptPacking += item.ReceiptPacking;
                TotReceiptOther += item.ReceiptOther;
                TotExpendPacking += item.ExpendPacking;
                TotExpendFinishing += item.ExpendFinishing;
                TotExpendPrinting += item.ExpendPrinting;
                TotExpendInspecting += item.ExpendInspecting;
                TotExpendDirty += item.ExpendDirty;
                TotExpendOther += item.ExpendOther;
                TotEndingBalance += item.EndingBalance;


                //result.Rows.Add(index++, item.ProductCode, item.RO, item.PlanPo, item.NoArticle, item.ProductName, item.Information, item.Buyer,

                //    item.BeginningBalanceQty, item.BeginningBalanceUom, item.ReceiptQty, item.ReceiptCorrectionQty, item.ReceiptUom,
                //    NumberFormat(item.ExpendQty),
                //    item.ExpandUom, item.EndingBalanceQty, item.EndingUom, item.From);


                result.Rows.Add(item.Nomor, item.Construction, item.BeginingBalance, item.ReceiptProduction, item.ReceiptRecheking, item.ReceiptFinishing, item.ReceiptPrinting, item.ReceiptPacking, item.ReceiptOther,
                                item.ExpendPacking, item.ExpendFinishing, item.ExpendPrinting, item.ExpendInspecting, item.ExpendDirty, item.ExpendOther, item.EndingBalance
                                );

            }
            

            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Data");
            

            var col = (char)('A' + (result.Columns.Count - 1));
            //string tglawal = new DateTimeOffset(dateFrom.Value.Date).ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            //string tglakhir = new DateTimeOffset(dateTo.Value.Date).ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            sheet.Cells[$"A1:{col}1"].Value = string.Format("LAPORAN KAIN GREY WEAVING III");
            sheet.Cells[$"A1:{col}1"].Merge = true;
            sheet.Cells[$"A1:{col}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A1:{col}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A1:{col}1"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Value = string.Format("PER : {0}", Bulan);
            sheet.Cells[$"A2:{col}2"].Merge = true;
            sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A3:{col}3"].Value = string.Format("Tanggal : {0}", Tanggal);
            sheet.Cells[$"A3:{col}3"].Merge = true;
            sheet.Cells[$"A3:{col}3"].Style.Font.Bold = true;
            sheet.Cells[$"A3:{col}3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A3:{col}3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


            sheet.Cells["A7"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light16);
            sheet.Cells["D5"].Value = headers[3];
            sheet.Cells["D5:I5"].Merge = true;
            sheet.Cells["J5"].Value = headers[9];
            sheet.Cells["J5:O5"].Merge = true;



            foreach (var i in Enumerable.Range(0, 3))
            {
                col = (char)('A' + i);
                sheet.Cells[$"{col}5"].Value = headers[i];
                sheet.Cells[$"{col}5:{col}6"].Merge = true;
            }

            for (var i = 0; i < 12; i++)
            {
                col = (char)('D' + i);
                sheet.Cells[$"{col}6"].Value = subheaders[i];

            }

            foreach (var i in Enumerable.Range(0, 1))
            {
                col = (char)('P' + i);
                sheet.Cells[$"{col}5"].Value = headers[i + 15];
                sheet.Cells[$"{col}5:{col}6"].Merge = true;
            }

            sheet.Cells["A5:P6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells["A5:P6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells["A5:P6"].Style.Font.Bold = true;
            var widths = new int[] { 10, 30, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 15 };
            foreach (var i in Enumerable.Range(0, headers.Length))
            {
                sheet.Column(i + 1).Width = widths[i];
            }

            sheet.Column(5).Hidden = true;

            var a = Query.Count();
            var date = a + 2;
            var dateNow = DateTime.Now.Date;
            

            //sheet.Cells[$"A{5}:J{5 + a + 3}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            sheet.Cells[$"A{4}:P{5 + a + 2}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"A{5}:Q{5 + a + 2}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;


            sheet.Cells[$"A{7 + a}"].Value = "T O T A L  . . . . . . . . . . . . . . .";
            sheet.Cells[$"A{7 + a}:B{7 + a}"].Merge = true;
            sheet.Cells[$"A{7 + a}:P{7 + a}"].Style.Font.Bold = true;
            sheet.Cells[$"A{7 + a}:B{7 + a}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A{7 + a}:B{7 + a}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"C{7 + a}"].Value = TotBeginingBalance;
            sheet.Cells[$"D{7 + a}"].Value = TotReceiptProduction;
            sheet.Cells[$"E{7 + a}"].Value = TotReceiptRecheking;
            sheet.Cells[$"F{7 + a}"].Value = TotReceiptFinishing;
            sheet.Cells[$"G{7 + a}"].Value = TotReceiptPrinting;
            sheet.Cells[$"H{7 + a}"].Value = TotReceiptPacking;
            sheet.Cells[$"I{7 + a}"].Value = TotReceiptOther;
            sheet.Cells[$"J{7 + a}"].Value = TotExpendPacking;
            sheet.Cells[$"K{7 + a}"].Value = TotExpendFinishing;
            sheet.Cells[$"L{7 + a}"].Value = TotExpendPrinting;
            sheet.Cells[$"M{7 + a}"].Value = TotExpendInspecting;
            sheet.Cells[$"N{7 + a}"].Value = TotExpendDirty;
            sheet.Cells[$"O{7 + a}"].Value = TotExpendOther;
            sheet.Cells[$"P{7 + a}"].Value = TotEndingBalance;




            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;


        }
    }
}
