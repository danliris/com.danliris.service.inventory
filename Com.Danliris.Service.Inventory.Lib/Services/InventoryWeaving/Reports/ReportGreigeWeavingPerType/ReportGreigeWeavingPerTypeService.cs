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

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportGreigeWeavingPerType
{
    public class ReportGreigeWeavingPerTypeService : IReportGreigeWeavingPerTypeService
    {
        private string USER_AGENT = "Service";
        private const string UserAgent = "inventory-service";
        protected DbSet<InventoryWeavingDocumentItem> DbSetItem;
        protected DbSet<InventoryWeavingMovement> DbSetMovement;
        protected DbSet<InventoryWeavingDocument> DbSetDoc;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;


        public ReportGreigeWeavingPerTypeService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSetItem = dbContext.Set<InventoryWeavingDocumentItem>();
            DbSetMovement = dbContext.Set<InventoryWeavingMovement>();
            DbSetDoc = dbContext.Set<InventoryWeavingDocument>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
            //IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<Tuple<List<ReportGreigeWeavingPerTypeViewModel>, int>> GetStockReport(DateTime? dateTo, int offset, int page, int size, string Order)
        {

            List<ReportGreigeWeavingPerTypeViewModel> Query = await GetStockQuery(dateTo, offset);
            Pageable<ReportGreigeWeavingPerTypeViewModel> pageable = new Pageable<ReportGreigeWeavingPerTypeViewModel>(Query, page - 1, size);

            List<ReportGreigeWeavingPerTypeViewModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;
            return Tuple.Create(Data, TotalData);
        }

        public async Task<List<ReportGreigeWeavingPerTypeViewModel>> GetStockQuery(DateTime? dateto, int offset)
        {
            DateTime dateReport = dateto == null ? DateTime.Now : (DateTime)dateto;
            var startDate = new DateTime(dateReport.Year, dateReport.Month, 1);


            var QueryAll = (from a in DbSetMovement
                            join c in DbSetDoc on a.InventoryWeavingDocumentId equals c.Id into m
                            from b in m.DefaultIfEmpty()
                            where
                            a._IsDeleted == false
                            && b._IsDeleted == false

                            select new ReportGreigeWeavingDataViewModel
                            {
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
                                QuantityPiece = a.QuantityPiece,
                                Quantity = a.Quantity,
                                Type = a.Type


                            }
                );

            var MasukAll = QueryAll.Where(d => d.Date.AddHours(offset).Date >= startDate.Date && d.Date.AddHours(offset).Date <= dateReport.Date && d.Type == "IN").Select(x => new ReportGreigeWeavingPerTypeViewModel
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
                InQuantityPiece = x.Type == "IN" ? x.QuantityPiece : 0,
                InQuantity = x.Type == "IN" ? x.Quantity : 0,
                OutQuantityPiece = 0,
                OutQuantity = 0,
                BeginQuantityPiece = 0,
                BeginQuantity = 0,
                EndingQuantityPiece = 0,
                EndingQuantity = 0

            });


            var Masuk = MasukAll.GroupBy(s => new
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
            }).Select(x => new ReportGreigeWeavingPerTypeViewModel
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
                InQuantityPiece = x.Sum(f => f.InQuantityPiece),
                InQuantity = x.Sum(f => f.InQuantity),
                OutQuantityPiece = 0,
                OutQuantity = 0,
                BeginQuantityPiece = 0,
                BeginQuantity = 0,
                EndingQuantityPiece = 0,
                EndingQuantity = 0

            });

            var KeluarAll = QueryAll.Where(d => d.Date.AddHours(offset).Date >= startDate.Date && d.Date.AddHours(offset).Date <= dateReport.Date && d.Type == "OUT").Select(x => new ReportGreigeWeavingPerTypeViewModel
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
                InQuantityPiece = 0,
                InQuantity = 0,
                OutQuantityPiece = x.Type == "OUT" ? x.QuantityPiece : 0,
                OutQuantity = x.Type == "OUT" ? x.Quantity : 0,
                BeginQuantityPiece = 0,
                BeginQuantity = 0,
                EndingQuantityPiece = 0,
                EndingQuantity = 0

            });


            var Keluar = KeluarAll.GroupBy(s => new
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


            }).Select(x => new ReportGreigeWeavingPerTypeViewModel
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
                InQuantityPiece = 0,
                InQuantity = 0,
                OutQuantityPiece = x.Sum(f => f.OutQuantityPiece),
                OutQuantity = x.Sum(f => f.OutQuantity),
                BeginQuantityPiece = 0,
                BeginQuantity = 0,
                EndingQuantityPiece = 0,
                EndingQuantity = 0

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

            }).Select(x => new ReportGreigeWeavingDataViewModel
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
                QuantityPiece = x.Sum(d => d.QuantityPiece),
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

            }).Select(x => new ReportGreigeWeavingDataViewModel
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
                QuantityPiece = x.Sum(d => d.QuantityPiece),
                Quantity = x.Sum(d => d.Quantity),
                Type = x.Key.Type


            });

            var SaldoAwal = SaldoAllAwal.GroupBy(s => new
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

            }).Select(x => new ReportGreigeWeavingPerTypeViewModel
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
                InQuantityPiece = 0,
                InQuantity = 0,
                OutQuantityPiece = 0,
                OutQuantity = 0,
                BeginQuantityPiece = x.FirstOrDefault(d => d.Type == "OUT") != null ? x.FirstOrDefault(d => d.Type == "IN").QuantityPiece - x.FirstOrDefault(d => d.Type == "OUT").QuantityPiece :
                                 x.FirstOrDefault(d => d.Type == "OUT") == null ? x.FirstOrDefault(d => d.Type == "IN").QuantityPiece : 0,
                BeginQuantity = x.FirstOrDefault(d => d.Type == "OUT") != null ? x.FirstOrDefault(d => d.Type == "IN").Quantity - x.FirstOrDefault(d => d.Type == "OUT").Quantity :
                                 x.FirstOrDefault(d => d.Type == "OUT") == null ? x.FirstOrDefault(d => d.Type == "IN").Quantity : 0,
                EndingQuantityPiece = 0,
                EndingQuantity = 0

            });

            var SaldoAkhir = SaldoAllAkhir.GroupBy(s => new
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

            }).Select(x => new ReportGreigeWeavingPerTypeViewModel
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
                InQuantityPiece = 0,
                InQuantity = 0,
                OutQuantityPiece = 0,
                OutQuantity = 0,
                BeginQuantityPiece = 0,
                BeginQuantity = 0,
                EndingQuantityPiece = x.FirstOrDefault(d => d.Type == "OUT") != null ? x.FirstOrDefault(d => d.Type == "IN").QuantityPiece - x.FirstOrDefault(d => d.Type == "OUT").QuantityPiece :
                                 x.FirstOrDefault(d => d.Type == "OUT") == null ? x.FirstOrDefault(d => d.Type == "IN").QuantityPiece : 0,
                EndingQuantity = x.FirstOrDefault(d => d.Type == "OUT") != null ? x.FirstOrDefault(d => d.Type == "IN").Quantity - x.FirstOrDefault(d => d.Type == "OUT").Quantity :
                                 x.FirstOrDefault(d => d.Type == "OUT") == null ? x.FirstOrDefault(d => d.Type == "IN").Quantity : 0


            });

            var cMasuk = Masuk.Count();
            var cKeluar = Keluar.Count();
            var cSaldoAwal = SaldoAwal.Count();
            var cSaldoAkhir = SaldoAkhir.Count();

            var Stock = SaldoAwal.Concat(SaldoAkhir).Concat(Masuk).Concat(Keluar).AsEnumerable();

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
            }, (key, data) => new ReportGreigeWeavingPerTypeViewModel
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
                InQuantityPiece = data.Sum(d => d.InQuantityPiece),
                InQuantity = data.Sum(d => d.InQuantity),
                OutQuantityPiece = data.Sum(d => d.OutQuantityPiece),
                OutQuantity = data.Sum(d => d.OutQuantity),
                BeginQuantityPiece = data.Sum(d => d.BeginQuantityPiece),
                BeginQuantity = data.Sum(d => d.BeginQuantity),
                EndingQuantityPiece = data.Sum(d => d.EndingQuantityPiece),
                EndingQuantity = data.Sum(d => d.EndingQuantity)

            });


            List<ReportGreigeWeavingPerTypeViewModel> stockReportViewModels = new List<ReportGreigeWeavingPerTypeViewModel>();
            int index = 1;
            foreach (var i in DataStock)
            {

                stockReportViewModels.Add(new ReportGreigeWeavingPerTypeViewModel
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
                    InQuantityPiece = i.InQuantityPiece,
                    InQuantity = i.InQuantity,
                    OutQuantityPiece = i.OutQuantityPiece,
                    OutQuantity = i.OutQuantity,
                    BeginQuantityPiece = i.BeginQuantityPiece,
                    BeginQuantity = i.BeginQuantity,
                    EndingQuantityPiece = i.EndingQuantityPiece,
                    EndingQuantity = i.EndingQuantity


                });


            }

            var total = new ReportGreigeWeavingPerTypeViewModel
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
                InQuantityPiece = Math.Round(DataStock.Sum(X => X.InQuantityPiece), 2),
                InQuantity = Math.Round(DataStock.Sum(X => X.InQuantity), 2),
                OutQuantityPiece = Math.Round(DataStock.Sum(X => X.OutQuantityPiece), 2),
                OutQuantity = Math.Round(DataStock.Sum(X => X.OutQuantity), 2),
                BeginQuantityPiece = Math.Round(DataStock.Sum(X => X.BeginQuantityPiece), 2),
                BeginQuantity = Math.Round(DataStock.Sum(X => X.BeginQuantity), 2),
                EndingQuantityPiece = Math.Round(DataStock.Sum(X => X.EndingQuantityPiece), 2),
                EndingQuantity = Math.Round(DataStock.Sum(X => X.EndingQuantity), 2),


            };


            stockReportViewModels.Add(total);

            return stockReportViewModels;


        }

        public async Task<MemoryStream> GenerateExcel(DateTime? dateTo, int offset)
        {
            var Query = await GetStockQuery(dateTo, offset);
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            //string Bulan = DateTo.ToString("MMM yyyy", new CultureInfo("id-ID"));
            string Tanggal = DateTo.ToString("dd MMM yyyy", new CultureInfo("id-ID"));

            DataTable result = new DataTable();
            var headers = new string[] { "No", "Konstruksi", "JL", "JP", "Gr", "Jn", "Awal", "Awal1", "Masuk", "Masuk1", "Keluar", "Keluar1", "Akhir", "Akhir1" };
            var subheaders = new string[] { "Piece", "Meter", "Piece", "Meter", "Piece", "Meter", "Piece", "Meter" };


            result.Columns.Add(new DataColumn() { ColumnName = headers[0], DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[1], DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[2], DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[3], DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[4], DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[5], DataType = typeof(string) });
            for (int i = 6; i < 14; i++)
            {
                result.Columns.Add(new DataColumn() { ColumnName = headers[i], DataType = typeof(Double) });
            }

            Query.RemoveAt(Query.Count() - 1 );

            double TotBeginQuantityPiece = 0;
            double TotBeginQuantity = 0;
            double TotInQuantityPiece = 0;
            double TotInQuantity = 0;
            double TotOutQuantityPiece = 0;
            double TotOutQuantity = 0;
            double TotEndingQuantityPiece = 0;
            double TotEndingQuantity = 0;



            foreach (var item in Query)
            {
                TotBeginQuantityPiece += item.BeginQuantityPiece;
                TotBeginQuantity += item.BeginQuantity;
                TotInQuantityPiece += item.InQuantityPiece;
                TotInQuantity += item.InQuantity;
                TotOutQuantityPiece += item.OutQuantityPiece;
                TotOutQuantity += item.OutQuantity;
                TotEndingQuantityPiece += item.EndingQuantityPiece;
                TotEndingQuantity += item.EndingQuantity;


                //result.Rows.Add(index++, item.ProductCode, item.RO, item.PlanPo, item.NoArticle, item.ProductName, item.Information, item.Buyer,

                //    item.BeginningBalanceQty, item.BeginningBalanceUom, item.ReceiptQty, item.ReceiptCorrectionQty, item.ReceiptUom,
                //    NumberFormat(item.ExpendQty),
                //    item.ExpandUom, item.EndingBalanceQty, item.EndingUom, item.From);


                result.Rows.Add(item.Nomor, item.Construction,"", "", "", "", item.BeginQuantityPiece, item.BeginQuantity, item.InQuantityPiece, item.InQuantity, item.OutQuantityPiece, item.OutQuantity,
                                item.EndingQuantityPiece, item.EndingQuantity);

            }

            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Data");


            var col = (char)('A' + (result.Columns.Count - 1));
            //string tglawal = new DateTimeOffset(dateFrom.Value.Date).ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            //string tglakhir = new DateTimeOffset(dateTo.Value.Date).ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            sheet.Cells[$"A1:{col}1"].Value = string.Format("LAPORAN SALDO AKHIR GUDANG GREY PER JENIS");
            sheet.Cells[$"A1:{col}1"].Merge = true;
            sheet.Cells[$"A1:{col}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A1:{col}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A1:{col}1"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Value = string.Format("Tanggal : {0}", Tanggal);
            sheet.Cells[$"A2:{col}2"].Merge = true;
            sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


            sheet.Cells["A6"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light14);
            sheet.Cells["G4"].Value = headers[6];
            sheet.Cells["G4:H4"].Merge = true;
            sheet.Cells["I4"].Value = headers[8];
            sheet.Cells["I4:J4"].Merge = true;
            sheet.Cells["K4"].Value = headers[10];
            sheet.Cells["K4:L4"].Merge = true;
            sheet.Cells["M4"].Value = headers[12];
            sheet.Cells["M4:N4"].Merge = true;


            foreach (var i in Enumerable.Range(0, 6))
            {
                col = (char)('A' + i);
                sheet.Cells[$"{col}4"].Value = headers[i];
                sheet.Cells[$"{col}4:{col}5"].Merge = true;
            }

            for (var i = 0; i < 8; i++)
            {
                col = (char)('G' + i);
                sheet.Cells[$"{col}5"].Value = subheaders[i];

            }

          /* foreach (var i in Enumerable.Range(0, 1))
            {
                col = (char)('N' + i);
                sheet.Cells[$"{col}4"].Value = headers[i + 14];
                sheet.Cells[$"{col}4:{col}5"].Merge = true;
            } */

            sheet.Cells["A4:N5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells["A4:N5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells["A4:N5"].Style.Font.Bold = true;
            var widths = new int[] { 10, 30, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
            foreach (var i in Enumerable.Range(0, headers.Length))
            {
                sheet.Column(i + 1).Width = widths[i];
            }

            sheet.Column(5).Hidden = true;

            var a = Query.Count();
            var date = a + 2;
            var dateNow = DateTime.Now.Date;


            //sheet.Cells[$"A{5}:J{5 + a + 3}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            sheet.Cells[$"A{3}:N{4 + a + 2}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"A{4}:O{4 + a + 2}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;


            sheet.Cells[$"A{6 + a}"].Value = "T O T A L  . . . . . . . . . . . . . . .";
            sheet.Cells[$"A{6 + a}:F{6 + a}"].Merge = true;
            sheet.Cells[$"A{6 + a}:N{6 + a}"].Style.Font.Bold = true;
            sheet.Cells[$"A{6 + a}:F{6 + a}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A{6 + a}:F{6 + a}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"G{6 + a}"].Value = TotBeginQuantityPiece;
            sheet.Cells[$"H{6 + a}"].Value = TotBeginQuantity;
            sheet.Cells[$"I{6 + a}"].Value = TotInQuantityPiece;
            sheet.Cells[$"J{6 + a}"].Value = TotInQuantity;
            sheet.Cells[$"K{6 + a}"].Value = TotOutQuantityPiece;
            sheet.Cells[$"L{6 + a}"].Value = TotOutQuantity;
            sheet.Cells[$"M{6 + a}"].Value = TotEndingQuantityPiece;
            sheet.Cells[$"N{6 + a}"].Value = TotEndingQuantity;




            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;


        }
    }

}
