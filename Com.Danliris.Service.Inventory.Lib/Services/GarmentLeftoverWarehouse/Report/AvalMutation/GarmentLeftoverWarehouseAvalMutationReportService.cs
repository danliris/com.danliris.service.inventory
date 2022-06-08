using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseAval;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Mutation;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.AvalMutation
{
    public class GarmentLeftoverWarehouseAvalMutationReportService : IGarmentLeftoverWarehouseAvalMutationReportService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseAvalMutationReportService";

        private InventoryDbContext DbContext;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        public GarmentLeftoverWarehouseAvalMutationReportService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));
        }

        public Tuple<List<GarmentLeftoverWarehouseAval>, int> GetAvalBesarIn(DateTime? dateFrom, DateTime? dateTo, int page, int size)
        {
            var Query = QueryAvalBesarIn(dateFrom, dateTo);

            //Pageable<GarmentLeftoverWarehouseAval> pageable = new Pageable<GarmentLeftoverWarehouseAval>(Query, page - 1, size);
            //List<GarmentLeftoverWarehouseAval> Data = pageable.Data.ToList<GarmentLeftoverWarehouseAval>();

            int TotalData = Query.Count();

            return Tuple.Create(Query, TotalData);
        }

        public Tuple<List<GarmentLeftoverWarehouseAval>, int> GetAvalBesarOut(DateTime? dateFrom, DateTime? dateTo, int page, int size)
        {
            var Query = QueryAvalBesarOut(dateFrom, dateTo);

            //Pageable<GarmentLeftoverWarehouseAval> pageable = new Pageable<GarmentLeftoverWarehouseAval>(Query, page - 1, size);
            //List<GarmentLeftoverWarehouseAval> Data = pageable.Data.ToList<GarmentLeftoverWarehouseAval>();

            int TotalData = Query.Count();

            return Tuple.Create(Query, TotalData);
        }

        public Tuple<List<GarmentLeftoverWarehouseAval>, int> GetAvalKomponenIn(DateTime? dateFrom, DateTime? dateTo, int page, int size)
        {
            var Query = QueryAvalKomponenIn(dateFrom, dateTo);

            //Pageable<GarmentLeftoverWarehouseAval> pageable = new Pageable<GarmentLeftoverWarehouseAval>(Query, page - 1, size);
            //List<GarmentLeftoverWarehouseAval> Data = pageable.Data.ToList<GarmentLeftoverWarehouseAval>();

            int TotalData = Query.Count();

            return Tuple.Create(Query, TotalData);
        }
        public Tuple<List<GarmentLeftoverWarehouseAval>, int> GetAvalKomponenOut(DateTime? dateFrom, DateTime? dateTo, int page, int size)
        {
            var Query = QueryAvalKomponenOut(dateFrom, dateTo);

            //Pageable<GarmentLeftoverWarehouseAval> pageable = new Pageable<GarmentLeftoverWarehouseAval>(Query, page - 1, size);
            //List<GarmentLeftoverWarehouseAval> Data = pageable.Data.ToList<GarmentLeftoverWarehouseAval>();

            int TotalData = Query.Count();

            return Tuple.Create(Query, TotalData);
        }
        public List<GarmentLeftoverWarehouseAval> QueryAvalBesarIn(DateTime? dateFrom, DateTime? dateTo)
        {
            DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
            DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;

            var BalanceDate = DbContext.GarmentLeftoverWarehouseBalanceStocks.OrderByDescending(x => x.BalanceStockDate).Select(x => x.BalanceStockDate).FirstOrDefault();

            var Query = (from a in DbContext.GarmentLeftoverWarehouseReceiptAvals
                         join b in DbContext.GarmentLeftoverWarehouseReceiptAvalItems on a.Id equals b.AvalReceiptId
                         where a._IsDeleted == false && b._IsDeleted == false
                         && a.ReceiptDate.AddHours(7).Date >= DateFrom.Date
                         && a.ReceiptDate.AddHours(7).Date <= DateTo.Date
                         && a.AvalType == "AVAL FABRIC"
                         select new GarmentLeftoverWarehouseAval
                         {
                             BonNo = a.AvalReceiptNo,
                             TransactionDate = a.ReceiptDate,
                             Keterangan = a.UnitFromName,
                             Product = b.ProductCode,
                             Quantity = b.Quantity,
                             UomUnit = b.UomUnit

                         }).GroupBy(x => new { x.BonNo, x.TransactionDate, x.Keterangan, x.Product, x.UomUnit }, (key, group) => new GarmentLeftoverWarehouseAval
                         {
                             BonNo = key.BonNo,
                             TransactionDate = key.TransactionDate,
                             Keterangan = key.Keterangan,
                             Product = key.Product,
                             Quantity = group.Sum(x => x.Quantity),
                             UomUnit = key.UomUnit,

                         }).ToList();

            foreach (var item in Query)
            {
                item.Quantity = Math.Round(item.Quantity, 2);
            }
            return Query;
        }

        public List<GarmentLeftoverWarehouseAval> QueryAvalKomponenIn(DateTime? dateFrom, DateTime? dateTo)
        {
            DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
            DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;

            var BalanceDate = DbContext.GarmentLeftoverWarehouseBalanceStocks.OrderByDescending(x => x.BalanceStockDate).Select(x => x.BalanceStockDate).FirstOrDefault();

            var Query = (from a in DbContext.GarmentLeftoverWarehouseReceiptAvals
                         //join b in DbContext.GarmentLeftoverWarehouseReceiptAvalItems on a.Id equals b.AvalReceiptId
                         where a._IsDeleted == false 
                         && a.ReceiptDate.AddHours(7).Date >= DateFrom.Date
                         && a.ReceiptDate.AddHours(7).Date <= DateTo.Date
                         && a.AvalType == "AVAL KOMPONEN"
                         select new GarmentLeftoverWarehouseAval
                         {
                             BonNo = a.AvalReceiptNo,
                             TransactionDate = a.ReceiptDate,
                             Keterangan = a.UnitFromName,
                             Product = a.UnitFromCode,
                             Quantity = a.TotalAval,
                             UomUnit = "KG"

                         }).GroupBy(x => new { x.BonNo, x.TransactionDate, x.Keterangan, x.Product, x.UomUnit }, (key, group) => new GarmentLeftoverWarehouseAval
                         {
                             BonNo = key.BonNo,
                             TransactionDate = key.TransactionDate,
                             Keterangan = key.Keterangan,
                             Product = key.Product,
                             Quantity = group.Sum(x => x.Quantity),
                             UomUnit = key.UomUnit,

                         }).ToList();

            foreach (var item in Query)
            {
                item.Quantity = Math.Round(item.Quantity, 2);
            }
            return Query;
        }

        public List<GarmentLeftoverWarehouseAval> QueryAvalBesarOut(DateTime? dateFrom, DateTime? dateTo)
        {
            DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
            DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;

            var BalanceDate = DbContext.GarmentLeftoverWarehouseBalanceStocks.OrderByDescending(x => x.BalanceStockDate).Select(x => x.BalanceStockDate).FirstOrDefault();

            var Query = (from a in DbContext.GarmentLeftoverWarehouseExpenditureAvals
                         join b in DbContext.GarmentLeftoverWarehouseExpenditureAvalItems on a.Id equals b.AvalExpenditureId
                         where a._IsDeleted == false && b._IsDeleted == false
                         && a.ExpenditureDate.AddHours(7).Date >= DateFrom.Date
                         && a.ExpenditureDate.AddHours(7).Date <= DateTo.Date
                         && a.AvalType == "AVAL FABRIC"
                         select new GarmentLeftoverWarehouseAval
                         {
                             BonNo = a.AvalExpenditureNo,
                             TransactionDate = a.ExpenditureDate,
                             Keterangan = b.UnitName,
                             Product = a.BuyerName,
                             Quantity = b.Quantity,
                             UomUnit = b.UomUnit

                         }).GroupBy(x => new { x.BonNo, x.TransactionDate, x.Keterangan, x.Product, x.UomUnit }, (key, group) => new GarmentLeftoverWarehouseAval
                         {
                             BonNo = key.BonNo,
                             TransactionDate = key.TransactionDate,
                             Keterangan = key.Keterangan,
                             Product = key.Product,
                             Quantity = group.Sum(x => x.Quantity),
                             UomUnit = key.UomUnit,

                         }).ToList();

            foreach (var item in Query)
            {
                item.Quantity = Math.Round(item.Quantity, 2);
            }
            return Query;
        }

        public List<GarmentLeftoverWarehouseAval> QueryAvalKomponenOut(DateTime? dateFrom, DateTime? dateTo)
        {
            DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
            DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;

            var BalanceDate = DbContext.GarmentLeftoverWarehouseBalanceStocks.OrderByDescending(x => x.BalanceStockDate).Select(x => x.BalanceStockDate).FirstOrDefault();

            var Query = (from a in DbContext.GarmentLeftoverWarehouseExpenditureAvals
                         join b in DbContext.GarmentLeftoverWarehouseExpenditureAvalItems on a.Id equals b.AvalExpenditureId
                         where a._IsDeleted == false && b._IsDeleted == false
                         && a.ExpenditureDate.AddHours(7).Date >= DateFrom.Date
                         && a.ExpenditureDate.AddHours(7).Date <= DateTo.Date
                        && a.AvalType == "AVAL KOMPONEN"
                         select new GarmentLeftoverWarehouseAval
                         {
                             BonNo = a.AvalExpenditureNo,
                             TransactionDate = a.ExpenditureDate,
                             Keterangan = b.UnitName,
                             Product = a.BuyerName,
                             Quantity = b.Quantity,
                             UomUnit = b.UomUnit

                         }).GroupBy(x => new { x.BonNo, x.TransactionDate, x.Keterangan, x.Product, x.UomUnit }, (key, group) => new GarmentLeftoverWarehouseAval
                         {
                             BonNo = key.BonNo,
                             TransactionDate = key.TransactionDate,
                             Keterangan = key.Keterangan,
                             Product = key.Product,
                             Quantity = group.Sum(x => x.Quantity),
                             UomUnit = key.UomUnit,

                         }).ToList();

            foreach (var item in Query)
            {
                item.Quantity = Math.Round(item.Quantity, 2);
            }
            return Query;
        }

        public MemoryStream GenerateExcelAvalBesar(DateTime dateFrom, DateTime dateTo)
        {
            var Query = QueryAvalBesarIn(dateFrom, dateTo);
            DataTable Result = new DataTable();
            //Result.Columns.Add(new DataColumn() { ColumnName = "NO", DataType = typeof(int) });
            Result.Columns.Add(new DataColumn() { ColumnName = "NOMOR BON MASUK", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL PEMASUKAN", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "ASAL TERIMA", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "KODE BARANG", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "QUANTITY", DataType = typeof(Double) });
            Result.Columns.Add(new DataColumn() { ColumnName = "SATUAN", DataType = typeof(String) });

            var index = 1;
            int idx = 1;
            var rCount = 0;
            Dictionary<string, string> Rowcount = new Dictionary<string, string>();

            if (Query.ToArray().Count() == 0)
                Result.Rows.Add( "", "", "", "", 0, "");
            else
                foreach(var item in Query)
                {
                    idx++;
                    if (!Rowcount.ContainsKey(item.BonNo))
                    {
                        rCount = 0;
                        var index1 = idx;
                        Rowcount.Add(item.BonNo, index1.ToString());
                    }
                    else
                    {
                        rCount += 1;
                        Rowcount[item.BonNo] = Rowcount[item.BonNo] + "-" + rCount.ToString();
                        var val = Rowcount[item.BonNo].Split("-");
                        if ((val).Length > 0)
                        {
                            Rowcount[item.BonNo] = val[0] + "-" + rCount.ToString();
                        }
                    }

                    var receiptDate = item.TransactionDate.ToString("dd MMM yyyy");
                    Result.Rows.Add( item.BonNo, receiptDate, item.Keterangan, item.Product, item.Quantity, item.UomUnit);
                }

            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Sheet1");

            var countdata = Query.Count();

            sheet.Cells["A" + 1 + ":F" + 4 + ""].Style.Font.Bold = true;
            sheet.Cells["A1"].Value = "Laporan Monitoring Pemasukan Aval Besar";
            sheet.Cells["A2"].Value = "Periode " + dateFrom.ToString("dd-MM-yyyy") + " s/d " + dateTo.ToString("dd-MM-yyyy");
            sheet.Cells["A" + 1 + ":F" + 1 + ""].Merge = true;
            sheet.Cells["A" + 2 + ":F" + 2 + ""].Merge = true;
            sheet.Cells["A" + 1 + ":F" + 4 + ""].Style.Font.Bold = true;

            if (countdata > 0)
            {
                sheet.Cells["F" + 5 + ":F" + (4 + countdata) + ""].Merge = true;
                sheet.Cells["F" + 5 + ":F" + (4 + countdata) + ""].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                sheet.Cells[$"A{(5 + countdata)}:D{(5 + countdata)}"].Merge = true;
                sheet.Cells[$"A{(5 + countdata)}:F{(5 + countdata)}"].Style.Font.Bold = true;
                //ADD SUMMARY OF QUANTITY
                sheet.Cells[$"A{(5 + countdata)}"].Value = "TOTAL";
                sheet.Cells[$"E{(5 + countdata)}"].Formula = "SUM(" + sheet.Cells["E" + 5 + ":E" + (4 + countdata) + ""].Address + ")";
                sheet.Calculate();
            }

            sheet.Cells.AutoFitColumns();
            sheet.Cells["A4"].LoadFromDataTable(Result, true);

            foreach(var a in Rowcount)
            {
                var UnitrowNum = a.Value.Split("-");
                int rowNum2 = 1;
                int rowNum1 = Convert.ToInt32(UnitrowNum[0]);
                if (UnitrowNum.Length > 1)
                {
                    rowNum2 = Convert.ToInt32(rowNum1) + Convert.ToInt32(UnitrowNum[1]);
                }
                else
                {
                    rowNum2 = Convert.ToInt32(rowNum1);
                }

                sheet.Cells[$"A{(rowNum1 + 3)}:A{(rowNum2) + 3}"].Merge = true;
                sheet.Cells[$"A{(rowNum1 + 3)}:A{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[$"A{(rowNum1 + 3)}:A{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                sheet.Cells[$"B{(rowNum1 + 3)}:B{(rowNum2) + 3}"].Merge = true;
                sheet.Cells[$"B{(rowNum1 + 3)}:B{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[$"B{(rowNum1 + 3)}:B{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                sheet.Cells[$"C{(rowNum1 + 3)}:C{(rowNum2) + 3}"].Merge = true;
                sheet.Cells[$"C{(rowNum1 + 3)}:C{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[$"C{(rowNum1 + 3)}:C{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
        }

        public MemoryStream GenerateExcelAvalBesarOut(DateTime dateFrom, DateTime dateTo)
        {
            var Query = QueryAvalBesarOut(dateFrom, dateTo);
            DataTable Result = new DataTable();
            //Result.Columns.Add(new DataColumn() { ColumnName = "NO", DataType = typeof(int) });
            Result.Columns.Add(new DataColumn() { ColumnName = "NOMOR BON KELUAR", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL PENGELUARAN", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "ASAL BARANG", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "BUYER", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "QUANTITY", DataType = typeof(Double) });
            Result.Columns.Add(new DataColumn() { ColumnName = "SATUAN", DataType = typeof(String) });

            var index = 1;
            int idx = 1;
            var rCount = 0;
            Dictionary<string, string> Rowcount = new Dictionary<string, string>();

            if (Query.ToArray().Count() == 0)
                Result.Rows.Add("", "", "", "", 0, "");
            else
                foreach (var item in Query)
                {
                    idx++;
                    if (!Rowcount.ContainsKey(item.BonNo))
                    {
                        rCount = 0;
                        var index1 = idx;
                        Rowcount.Add(item.BonNo, index1.ToString());
                    }
                    else
                    {
                        rCount += 1;
                        Rowcount[item.BonNo] = Rowcount[item.BonNo] + "-" + rCount.ToString();
                        var val = Rowcount[item.BonNo].Split("-");
                        if ((val).Length > 0)
                        {
                            Rowcount[item.BonNo] = val[0] + "-" + rCount.ToString();
                        }
                    }

                    var receiptDate = item.TransactionDate.ToString("dd MMM yyyy");
                    Result.Rows.Add(item.BonNo, receiptDate, item.Keterangan, item.Product, item.Quantity, item.UomUnit);
                }

            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Sheet1");

            var countdata = Query.Count();

            sheet.Cells["A" + 1 + ":F" + 4 + ""].Style.Font.Bold = true;
            sheet.Cells["A1"].Value = "Laporan Monitoring Pengeluaran Aval Besar";
            sheet.Cells["A2"].Value = "Periode " + dateFrom.ToString("dd-MM-yyyy") + " s/d " + dateTo.ToString("dd-MM-yyyy");
            sheet.Cells["A" + 1 + ":F" + 1 + ""].Merge = true;
            sheet.Cells["A" + 2 + ":F" + 2 + ""].Merge = true;
            sheet.Cells["A" + 1 + ":F" + 4 + ""].Style.Font.Bold = true;

            if (countdata > 0)
            {
                sheet.Cells["F" + 5 + ":F" + (4 + countdata) + ""].Merge = true;
                sheet.Cells["F" + 5 + ":F" + (4 + countdata) + ""].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                sheet.Cells[$"A{(5 + countdata)}:D{(5 + countdata)}"].Merge = true;
                sheet.Cells[$"A{(5 + countdata)}:F{(5 + countdata)}"].Style.Font.Bold = true;
                //ADD SUMMARY OF QUANTITY
                sheet.Cells[$"A{(5 + countdata)}"].Value = "TOTAL";
                sheet.Cells[$"E{(5 + countdata)}"].Formula = "SUM(" + sheet.Cells["E" + 5 + ":E" + (4 + countdata) + ""].Address + ")";
                sheet.Calculate();
            }

            sheet.Cells.AutoFitColumns();
            sheet.Cells["A4"].LoadFromDataTable(Result, true);

            foreach (var a in Rowcount)
            {
                var UnitrowNum = a.Value.Split("-");
                int rowNum2 = 1;
                int rowNum1 = Convert.ToInt32(UnitrowNum[0]);
                if (UnitrowNum.Length > 1)
                {
                    rowNum2 = Convert.ToInt32(rowNum1) + Convert.ToInt32(UnitrowNum[1]);
                }
                else
                {
                    rowNum2 = Convert.ToInt32(rowNum1);
                }

                sheet.Cells[$"A{(rowNum1 + 3)}:A{(rowNum2) + 3}"].Merge = true;
                sheet.Cells[$"A{(rowNum1 + 3)}:A{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[$"A{(rowNum1 + 3)}:A{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                sheet.Cells[$"B{(rowNum1 + 3)}:B{(rowNum2) + 3}"].Merge = true;
                sheet.Cells[$"B{(rowNum1 + 3)}:B{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[$"B{(rowNum1 + 3)}:B{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                sheet.Cells[$"D{(rowNum1 + 3)}:D{(rowNum2) + 3}"].Merge = true;
                sheet.Cells[$"D{(rowNum1 + 3)}:D{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[$"D{(rowNum1 + 3)}:D{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            }

            var stream = new MemoryStream();
                package.SaveAs(stream);
                return stream;
            
        }

        public MemoryStream GenerateExcelAvalKomponenIn(DateTime dateFrom, DateTime dateTo)
        {
            var Query = QueryAvalKomponenIn(dateFrom, dateTo);
            DataTable Result = new DataTable();
            //Result.Columns.Add(new DataColumn() { ColumnName = "NO", DataType = typeof(int) });
            Result.Columns.Add(new DataColumn() { ColumnName = "NOMOR BON MASUK", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL PEMASUKAN", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "KODE ASAL", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "ASAL TERIMA", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "QUANTITY", DataType = typeof(Double) });
            Result.Columns.Add(new DataColumn() { ColumnName = "SATUAN", DataType = typeof(String) });

            var index = 1;
            int idx = 1;
            var rCount = 0;
            Dictionary<string, string> Rowcount = new Dictionary<string, string>();

            if (Query.ToArray().Count() == 0)
                Result.Rows.Add("", "", "", "", 0, "");
            else
                foreach (var item in Query)
                {
                    //idx++;
                    //if (!Rowcount.ContainsKey(item.BonNo))
                    //{
                    //    rCount = 0;
                    //    var index1 = idx;
                    //    Rowcount.Add(item.BonNo, index1.ToString());
                    //}
                    //else
                    //{
                    //    rCount += 1;
                    //    Rowcount[item.BonNo] = Rowcount[item.BonNo] + "-" + rCount.ToString();
                    //    var val = Rowcount[item.BonNo].Split("-");
                    //    if ((val).Length > 0)
                    //    {
                    //        Rowcount[item.BonNo] = val[0] + "-" + rCount.ToString();
                    //    }
                    //}

                    var receiptDate = item.TransactionDate.ToString("dd MMM yyyy");
                    Result.Rows.Add(item.BonNo, receiptDate, item.Product, item.Keterangan, item.Quantity, item.UomUnit);
                }

            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Sheet1");

            var countdata = Query.Count();

            sheet.Cells["A" + 1 + ":F" + 4 + ""].Style.Font.Bold = true;
            sheet.Cells["A1"].Value = "Laporan Monitoring Pemasukan Aval Komponen";
            sheet.Cells["A2"].Value = "Periode " + dateFrom.ToString("dd-MM-yyyy") + " s/d " + dateTo.ToString("dd-MM-yyyy");
            sheet.Cells["A" + 1 + ":F" + 1 + ""].Merge = true;
            sheet.Cells["A" + 2 + ":F" + 2 + ""].Merge = true;
            sheet.Cells["A" + 1 + ":F" + 4 + ""].Style.Font.Bold = true;

            if (countdata > 0)
            {
                //    sheet.Cells["F" + 5 + ":F" + (4 + countdata) + ""].Merge = true;
                //    sheet.Cells["F" + 5 + ":F" + (4 + countdata) + ""].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                //    sheet.Cells[$"A{(5 + countdata)}:D{(5 + countdata)}"].Merge = true;
                //    sheet.Cells[$"A{(5 + countdata)}:F{(5 + countdata)}"].Style.Font.Bold = true;
                //ADD SUMMARY OF QUANTITY
                sheet.Cells[$"A{(5 + countdata)}"].Value = "TOTAL";
                sheet.Cells[$"E{(5 + countdata)}"].Formula = "SUM(" + sheet.Cells["E" + 5 + ":E" + (4 + countdata) + ""].Address + ")";
                sheet.Calculate();
            }

            sheet.Cells.AutoFitColumns();
            sheet.Cells["A4"].LoadFromDataTable(Result, true);

            //foreach (var a in Rowcount)
            //{
            //    var UnitrowNum = a.Value.Split("-");
            //    int rowNum2 = 1;
            //    int rowNum1 = Convert.ToInt32(UnitrowNum[0]);
            //    if (UnitrowNum.Length > 1)
            //    {
            //        rowNum2 = Convert.ToInt32(rowNum1) + Convert.ToInt32(UnitrowNum[1]);
            //    }
            //    else
            //    {
            //        rowNum2 = Convert.ToInt32(rowNum1);
            //    }

            //    sheet.Cells[$"A{(rowNum1 + 3)}:A{(rowNum2) + 3}"].Merge = true;
            //    sheet.Cells[$"A{(rowNum1 + 3)}:A{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //    sheet.Cells[$"A{(rowNum1 + 3)}:A{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            //    sheet.Cells[$"B{(rowNum1 + 3)}:B{(rowNum2) + 3}"].Merge = true;
            //    sheet.Cells[$"B{(rowNum1 + 3)}:B{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //    sheet.Cells[$"B{(rowNum1 + 3)}:B{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            //    sheet.Cells[$"C{(rowNum1 + 3)}:C{(rowNum2) + 3}"].Merge = true;
            //    sheet.Cells[$"C{(rowNum1 + 3)}:C{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //    sheet.Cells[$"C{(rowNum1 + 3)}:C{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            //}

            var stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
        }
        public MemoryStream GenerateExcelAvalKomponenOut(DateTime dateFrom, DateTime dateTo)
        {
            var Query = QueryAvalKomponenOut(dateFrom, dateTo);
            DataTable Result = new DataTable();
            //Result.Columns.Add(new DataColumn() { ColumnName = "NO", DataType = typeof(int) });
            Result.Columns.Add(new DataColumn() { ColumnName = "NOMOR BON KELUAR", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL PENGELUARAN", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "ASAL BARANG", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "BUYER", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "QUANTITY", DataType = typeof(Double) });
            Result.Columns.Add(new DataColumn() { ColumnName = "SATUAN", DataType = typeof(String) });

            var index = 1;
            int idx = 1;
            var rCount = 0;
            Dictionary<string, string> Rowcount = new Dictionary<string, string>();

            if (Query.ToArray().Count() == 0)
                Result.Rows.Add("", "", "", "", 0, "");
            else
                foreach (var item in Query)
                {
                    idx++;
                    if (!Rowcount.ContainsKey(item.BonNo))
                    {
                        rCount = 0;
                        var index1 = idx;
                        Rowcount.Add(item.BonNo, index1.ToString());
                    }
                    else
                    {
                        rCount += 1;
                        Rowcount[item.BonNo] = Rowcount[item.BonNo] + "-" + rCount.ToString();
                        var val = Rowcount[item.BonNo].Split("-");
                        if ((val).Length > 0)
                        {
                            Rowcount[item.BonNo] = val[0] + "-" + rCount.ToString();
                        }
                    }

                    var receiptDate = item.TransactionDate.ToString("dd MMM yyyy");
                    Result.Rows.Add(item.BonNo, receiptDate, item.Keterangan, item.Product, item.Quantity, item.UomUnit);
                }

            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Sheet1");

            var countdata = Query.Count();

            sheet.Cells["A" + 1 + ":F" + 4 + ""].Style.Font.Bold = true;
            sheet.Cells["A1"].Value = "Laporan Monitoring Pengeluaran Aval Komponen";
            sheet.Cells["A2"].Value = "Periode " + dateFrom.ToString("dd-MM-yyyy") + " s/d " + dateTo.ToString("dd-MM-yyyy");
            sheet.Cells["A" + 1 + ":F" + 1 + ""].Merge = true;
            sheet.Cells["A" + 2 + ":F" + 2 + ""].Merge = true;
            sheet.Cells["A" + 1 + ":F" + 4 + ""].Style.Font.Bold = true;

            if (countdata > 0)
            {
                sheet.Cells["F" + 5 + ":F" + (4 + countdata) + ""].Merge = true;
                sheet.Cells["F" + 5 + ":F" + (4 + countdata) + ""].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                sheet.Cells[$"A{(5 + countdata)}:D{(5 + countdata)}"].Merge = true;
                sheet.Cells[$"A{(5 + countdata)}:F{(5 + countdata)}"].Style.Font.Bold = true;
                //ADD SUMMARY OF QUANTITY
                sheet.Cells[$"A{(5 + countdata)}"].Value = "TOTAL";
                sheet.Cells[$"E{(5 + countdata)}"].Formula = "SUM(" + sheet.Cells["E" + 5 + ":E" + (4 + countdata) + ""].Address + ")";
                sheet.Calculate();
            }

            sheet.Cells.AutoFitColumns();
            sheet.Cells["A4"].LoadFromDataTable(Result, true);

            foreach (var a in Rowcount)
            {
                var UnitrowNum = a.Value.Split("-");
                int rowNum2 = 1;
                int rowNum1 = Convert.ToInt32(UnitrowNum[0]);
                if (UnitrowNum.Length > 1)
                {
                    rowNum2 = Convert.ToInt32(rowNum1) + Convert.ToInt32(UnitrowNum[1]);
                }
                else
                {
                    rowNum2 = Convert.ToInt32(rowNum1);
                }

                sheet.Cells[$"A{(rowNum1 + 3)}:A{(rowNum2) + 3}"].Merge = true;
                sheet.Cells[$"A{(rowNum1 + 3)}:A{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[$"A{(rowNum1 + 3)}:A{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                sheet.Cells[$"B{(rowNum1 + 3)}:B{(rowNum2) + 3}"].Merge = true;
                sheet.Cells[$"B{(rowNum1 + 3)}:B{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[$"B{(rowNum1 + 3)}:B{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                sheet.Cells[$"D{(rowNum1 + 3)}:D{(rowNum2) + 3}"].Merge = true;
                sheet.Cells[$"D{(rowNum1 + 3)}:D{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[$"D{(rowNum1 + 3)}:D{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;

        }

    }
}
