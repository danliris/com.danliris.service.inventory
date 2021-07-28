using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Bookkeeping;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Stock;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Bookkeeping
{
    public class GarmentLeftoverWarehouseStockBookkeepingReportService : IGarmentLeftoverWarehouseStockBookkeepingReportService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseStockBookkeepingReportService";

        private InventoryDbContext DbContext;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        public GarmentLeftoverWarehouseStockBookkeepingReportService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));
        }
        class Products
        {
            public string ProductCode { get; internal set; }
            public string ProductName { get; internal set; }
            public string PONo { get; internal set; }
        }
        public List<GarmentLeftoverWarehouseStockBookkeepingReportViewModel> GetReportQuery(DateTime? dateTo, string type, int offset)
        {
            DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;
            DateTimeOffset DateFrom = new DateTime(1970, 1, 1);

            List<GarmentLeftoverWarehouseStockBookkeepingReportViewModel> GarmentLeftoverWarehouseStockBookkeepingReportViewModel = new List<GarmentLeftoverWarehouseStockBookkeepingReportViewModel>();
            var _productF = (from a in DbContext.GarmentLeftoverWarehouseReceiptFabricItems
                             where a._IsDeleted == false
                             select new Products { ProductCode = a.ProductCode, ProductName = a.ProductName, PONo = a.POSerialNumber }).Distinct();
            var _productB = (from a in DbContext.GarmentLeftoverWarehouseBalanceStocksItems
                             where a._IsDeleted == false
                             select new Products { ProductCode = a.ProductCode, ProductName = a.ProductName, PONo = a.PONo }).Distinct();
            var _productA = (from a in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems
                             where a._IsDeleted == false
                             select new Products { ProductCode = a.ProductCode, ProductName = a.ProductName, PONo = a.POSerialNumber }).Distinct();

            var _product = _productF.Union(_productB).Union(_productA);
            if (type == "FABRIC")
            {


                var QueryBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                              where data._IsDeleted == false && data.TypeOfGoods.ToString() == "FABRIC"
                                              select new { data._CreatedUtc, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
                                   select new GarmentLeftoverWarehouseStockBookkeepingReportViewModel
                                   {
                                       BeginingbalanceQty = b.Quantity,
                                       BeginingbalancePrice = b.Quantity * b.BasicPrice,
                                       QuantityReceipt = 0,
                                       PriceReceipt = 0,
                                       UomUnit = b.UomUnit,
                                       ProductCode = b.ProductCode,
                                       ProductName = b.ProductName,
                                       EndbalanceQty = 0,
                                       BasicPrice = b.BasicPrice,
                                       PriceExpend = 0,
                                       QuantityExpend = 0
                                   };
                var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFabrics
                                              where data._IsDeleted == false
                                         && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                                              select new { data.ReceiptDate, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseReceiptFabricItems on a.Id equals b.GarmentLeftoverWarehouseReceiptFabricId
                                   select new GarmentLeftoverWarehouseStockBookkeepingReportViewModel
                                   {
                                       BeginingbalanceQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date ? b.Quantity : 0,
                                       BeginingbalancePrice = (a.ReceiptDate.AddHours(offset).Date < DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
                                       QuantityReceipt = a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date ? b.Quantity : 0,
                                       PriceReceipt = (a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
                                       BasicPrice = b.BasicPrice,
                                       UomUnit = b.UomUnit,
                                       ProductCode = b.ProductCode,
                                       ProductName = b.ProductName,
                                       EndbalanceQty = 0,
                                       PriceExpend = 0,
                                       QuantityExpend = 0

                                   };
                var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFabrics
                                                  where data._IsDeleted == false
                                             && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date

                                                  select new { data.UnitExpenditureCode, data.ExpenditureDate, data.Id, data.ExpenditureDestination })
                                       join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFabricItems
                                                  select new { expend.BasicPrice, expend.UomUnit, expend.PONo, expend.Quantity, expend.UnitName, expend.ExpenditureId }) on a.Id equals b.ExpenditureId
                                       select new GarmentLeftoverWarehouseStockBookkeepingReportViewModel
                                       {
                                           BeginingbalanceQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date ? -b.Quantity : 0,
                                           BeginingbalancePrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date ? -b.Quantity : 0) * b.BasicPrice,
                                           QuantityReceipt = 0,
                                           PriceReceipt = 0,
                                           BasicPrice = b.BasicPrice,
                                           UomUnit = b.UomUnit,

                                           QuantityExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date ? b.Quantity : 0,
                                           PriceExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,

                                           ProductCode = (from aa in _product where aa.PONo == b.PONo select aa.ProductCode).FirstOrDefault(),
                                           ProductName = (from aa in _product where aa.PONo == b.PONo select aa.ProductName).FirstOrDefault(),
                                           EndbalanceQty = 0
                                       };
                var Query = QueryReceipt.Union(QueryExpenditure).Union(QueryBalance);
                var querySum = Query.ToList()
                    .GroupBy(x => new { x.UomUnit, x.ProductCode, x.ProductName }, (key, group) => new
                    {
                        begining = group.Sum(s => s.BeginingbalanceQty),
                        beginingPrice = group.Sum(s => s.BeginingbalancePrice),
                        receipt = group.Sum(s => s.QuantityReceipt),
                        priceReceipt = group.Sum(s => s.PriceReceipt),
                        basicprice = group.Sum(s => s.BasicPrice),
                        uomunit = key.UomUnit,
                        productCode = key.ProductCode,
                        productName = key.ProductName,
                        expend = group.Sum(s => s.QuantityExpend),
                        expendPrice = group.Sum(s => s.PriceExpend),
                    }).OrderBy(s => s.productCode);

                foreach (var data in querySum)
                {

                    GarmentLeftoverWarehouseStockBookkeepingReportViewModel garmentLeftover = new GarmentLeftoverWarehouseStockBookkeepingReportViewModel
                    {
                        BeginingbalanceQty = data.begining,
                        BeginingbalancePrice = data.beginingPrice,
                        QuantityReceipt = data.receipt,
                        PriceReceipt = data.priceReceipt,
                        BasicPrice = data.basicprice,
                        UomUnit = data.uomunit,
                        ProductCode = data.productCode,
                        ProductName = data.productName,
                        QuantityExpend=data.expend,
                        PriceExpend=data.expendPrice,
                        EndbalanceQty = data.begining + data.receipt - data.expend,
                        EndbalancePrice = data.beginingPrice + data.priceReceipt - data.expendPrice
                    };
                    GarmentLeftoverWarehouseStockBookkeepingReportViewModel.Add(garmentLeftover);
                }
            }
            else if (type == "BARANG JADI")
            {

                var QueryBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                              where data._IsDeleted == false && data.TypeOfGoods.ToString() == "BARANG JADI"
                                              select new { data._CreatedUtc, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId

                                   select new GarmentLeftoverWarehouseStockBookkeepingReportViewModel
                                   {
                                       BeginingbalanceQty = b.Quantity,
                                       BeginingbalancePrice = b.Quantity * b.BasicPrice,
                                       QuantityReceipt = 0,
                                       PriceReceipt = 0,
                                       UomUnit = b.UomUnit,
                                       ProductCode = b.LeftoverComodityCode,
                                       ProductName = b.LeftoverComodityName,
                                       EndbalanceQty = 0,
                                       PriceExpend = 0,
                                       QuantityExpend = 0
                                   };
                var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoods
                                              where data._IsDeleted == false
                                         && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                                              select new { data.ReceiptDate, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoodItems on a.Id equals b.FinishedGoodReceiptId
                                   select new GarmentLeftoverWarehouseStockBookkeepingReportViewModel
                                   {
                                       BeginingbalanceQty = a.ReceiptDate.AddHours(offset) < DateFrom.Date ? b.Quantity : 0,
                                       BeginingbalancePrice = (a.ReceiptDate.AddHours(offset) < DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
                                       QuantityReceipt = a.ReceiptDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0,
                                       PriceReceipt = (a.ReceiptDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
                                       BasicPrice = 0,
                                       UomUnit = "PCS",
                                       ProductCode = b.LeftoverComodityCode,
                                       ProductName = b.LeftoverComodityName,
                                       EndbalanceQty = 0,
                                       PriceExpend = 0,
                                       QuantityExpend = 0
                                   };
                var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoods
                                                  where data._IsDeleted == false
                                                        && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                                                  select new { data.ExpenditureDate, data.Id, data.ExpenditureTo })
                                       join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoodItems
                                                  select new { expend.BasicPrice, expend.FinishedGoodExpenditureId, expend.ExpenditureQuantity, expend.LeftoverComodityCode, expend.LeftoverComodityName }
                                                  ) on a.Id equals b.FinishedGoodExpenditureId
                                       select new GarmentLeftoverWarehouseStockBookkeepingReportViewModel
                                       {
                                           BeginingbalanceQty = a.ExpenditureDate.AddHours(offset) < DateFrom.Date ? -b.ExpenditureQuantity : 0,
                                           BeginingbalancePrice = (a.ExpenditureDate.AddHours(offset) < DateFrom.Date ? -b.ExpenditureQuantity : 0) * b.BasicPrice,
                                           QuantityReceipt = 0,
                                           PriceReceipt = 0,
                                           BasicPrice = 0,
                                           QuantityExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date ? b.ExpenditureQuantity : 0,
                                           PriceExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date ? b.ExpenditureQuantity : 0) * b.BasicPrice,
                                           UomUnit = "PCS",
                                           ProductCode = b.LeftoverComodityCode,
                                           ProductName = b.LeftoverComodityName,
                                           EndbalanceQty = 0
                                       };
                var Query = QueryReceipt.Union(QueryExpenditure).Union(QueryBalance);
                var querySum = Query.ToList()
                    .GroupBy(x => new { x.UomUnit, x.ProductCode, x.ProductName }, (key, group) => new
                    {
                        begining = group.Sum(s => s.BeginingbalanceQty),
                        beginingPrice = group.Sum(s => s.BeginingbalancePrice),
                        expend = group.Sum(s => s.QuantityExpend),
                        expendPrice = group.Sum(s => s.PriceExpend),
                        receipt = group.Sum(s => s.QuantityReceipt),
                        priceReceipt = group.Sum(s => s.PriceReceipt),
                        basicprice = group.Sum(s => s.BasicPrice),
                        uomunit = key.UomUnit,
                        productCode = key.ProductCode,
                        productName = key.ProductName
                    }).OrderBy(s => s.productCode);


                foreach (var data in querySum)
                {
                    GarmentLeftoverWarehouseStockBookkeepingReportViewModel garmentLeftover = new GarmentLeftoverWarehouseStockBookkeepingReportViewModel
                    {
                        BeginingbalanceQty = data.begining,
                        BeginingbalancePrice = data.beginingPrice,
                        QuantityReceipt = data.receipt,
                        PriceReceipt = data.priceReceipt,
                        BasicPrice = data.basicprice,
                        UomUnit = data.uomunit,
                        ProductCode = data.productCode,
                        ProductName = data.productName,
                        QuantityExpend = data.expend,
                        PriceExpend = data.expendPrice,
                        EndbalanceQty = data.begining + data.receipt - data.expend,
                        EndbalancePrice = data.beginingPrice + data.priceReceipt - data.expendPrice
                    };
                    GarmentLeftoverWarehouseStockBookkeepingReportViewModel.Add(garmentLeftover);
                }
            }
            else
            {
                var QueryBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                              where data._IsDeleted == false && data.TypeOfGoods.ToString() == "ACCESSORIES"
                                              select new { data._CreatedUtc, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
                                   select new GarmentLeftoverWarehouseStockBookkeepingReportViewModel
                                   {
                                       BeginingbalanceQty = b.Quantity,
                                       BeginingbalancePrice = b.Quantity * b.BasicPrice,
                                       QuantityReceipt = 0,
                                       PriceReceipt = 0,
                                       UomUnit = b.UomUnit,
                                       ProductCode = b.ProductCode,
                                       ProductName = b.ProductName,
                                       EndbalanceQty = 0,
                                       PriceExpend = 0,
                                       QuantityExpend = 0
                                   };
                var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptAccessories
                                              where data._IsDeleted == false
                                         && data.StorageReceiveDate.AddHours(offset).Date <= DateTo.Date
                                              select new { data.RequestUnitName, data.StorageReceiveDate, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems on a.Id equals b.GarmentLeftOverWarehouseReceiptAccessoriesId
                                   select new GarmentLeftoverWarehouseStockBookkeepingReportViewModel
                                   {
                                       BeginingbalanceQty = a.StorageReceiveDate.AddHours(offset) < DateFrom.Date ? b.Quantity : 0,
                                       BeginingbalancePrice = (a.StorageReceiveDate.AddHours(offset) < DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
                                       QuantityReceipt = a.StorageReceiveDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0,
                                       PriceReceipt = (a.StorageReceiveDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
                                       UomUnit = b.UomUnit,
                                       ProductCode = b.ProductCode,
                                       ProductName = b.ProductName,
                                       EndbalanceQty = 0,
                                       PriceExpend = 0,
                                       QuantityExpend = 0
                                   };
                var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureAccessories
                                                  where data._IsDeleted == false
                                             && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date

                                                  select new { data.ExpenditureDate, data.Id, data.ExpenditureDestination })
                                       join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureAccessoriesItems
                                                  select new { expend.BasicPrice, expend.ExpenditureId, expend.UomUnit, expend.UnitName, expend.Quantity, expend.PONo }
                                                  ) on a.Id equals b.ExpenditureId
                                       select new GarmentLeftoverWarehouseStockBookkeepingReportViewModel
                                       {
                                           BeginingbalanceQty = a.ExpenditureDate.AddHours(offset) < DateFrom.Date ? -b.Quantity : 0,
                                           BeginingbalancePrice = (a.ExpenditureDate.AddHours(offset) < DateFrom.Date ? -b.Quantity : 0) * b.BasicPrice,
                                           QuantityReceipt = 0,
                                           PriceReceipt = 0,
                                           QuantityExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date ? b.Quantity : 0,
                                           PriceExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
                                           UomUnit = b.UomUnit,
                                           ProductCode = (from aa in _product where aa.PONo == b.PONo select aa.ProductCode).FirstOrDefault(),
                                           ProductName = (from aa in _product where aa.PONo == b.PONo select aa.ProductName).FirstOrDefault(),
                                           EndbalanceQty = 0,
                                       };
                var Query = QueryReceipt.Union(QueryExpenditure).Union(QueryBalance);
                var querySum = Query.ToList()
                    .GroupBy(x => new { x.UomUnit, x.ProductCode, x.ProductName }, (key, group) => new
                    {
                        begining = group.Sum(s => s.BeginingbalanceQty),
                        beginingPrice = group.Sum(s => s.BeginingbalancePrice),
                        expend = group.Sum(s => s.QuantityExpend),
                        expendPrice = group.Sum(s => s.PriceExpend),
                        receipt = group.Sum(s => s.QuantityReceipt),
                        priceReceipt = group.Sum(s => s.PriceReceipt),
                        basicprice = group.Sum(s => s.BasicPrice),
                        uomunit = key.UomUnit,
                        productCode = key.ProductCode,
                        productName = key.ProductName
                    }).OrderBy(s => s.productCode);

                foreach (var data in querySum)
                {

                    GarmentLeftoverWarehouseStockBookkeepingReportViewModel garmentLeftover = new GarmentLeftoverWarehouseStockBookkeepingReportViewModel
                    {
                        BeginingbalanceQty = data.begining,
                        BeginingbalancePrice = data.beginingPrice,
                        QuantityReceipt = data.receipt,
                        PriceReceipt = data.priceReceipt,
                        BasicPrice = data.basicprice,
                        UomUnit = data.uomunit,
                        ProductCode = data.productCode,
                        ProductName = data.productName,
                        QuantityExpend = data.expend,
                        PriceExpend = data.expendPrice,
                        EndbalanceQty = data.begining + data.receipt - data.expend,
                        EndbalancePrice = data.beginingPrice + data.priceReceipt - data.expendPrice
                    };
                    GarmentLeftoverWarehouseStockBookkeepingReportViewModel.Add(garmentLeftover);
                }
            }

            var stockdata = GarmentLeftoverWarehouseStockBookkeepingReportViewModel.OrderBy(a => a.ProductCode).ToList();
            var Total = new GarmentLeftoverWarehouseStockBookkeepingReportViewModel
            {
                BeginingbalanceQty = 0,
                BeginingbalancePrice = 0,
                QuantityReceipt = 0,
                PriceReceipt = 0,
                BasicPrice = 0,
                UomUnit = "",
                ProductCode = "GRAND TOTAL",
                ProductName = "",
                EndbalanceQty = stockdata.Sum(a => a.EndbalanceQty),
                EndbalancePrice = stockdata.Sum(a => a.EndbalancePrice)
            };
            stockdata.Add(Total);
            return stockdata;
        }
        public Tuple<MemoryStream, string> GenerateExcel(DateTime? dateTo, string type, int offset)
        {
            var Query = GetReportQuery(dateTo, type, offset);
            var data = Query.ToList();

            //var QtyTotal = data.Sum(x => x.EndbalanceQty);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Rp", DataType = typeof(double) });

            //List<(string, Enum, Enum)> mergeCells = new List<(string, Enum, Enum)>() { };
            Dictionary<string, string> Rowcount = new Dictionary<string, string>();
            int idx = 1;
            var rCount = 0;
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {
                //int index = 0;

                foreach (var item in Query.ToList())
                {
                    result.Rows.Add(item.ProductCode, item.ProductName, item.UomUnit, item.EndbalanceQty, item.EndbalancePrice);

                }

            }
            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Report Stok Gudang Sisa");
            sheet.Cells["A1"].LoadFromDataTable(result, true, OfficeOpenXml.Table.TableStyles.Light16);

            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            MemoryStream streamExcel = new MemoryStream();
            package.SaveAs(streamExcel);

            //Dictionary<string, string> FilterDictionary = new Dictionary<string, string>(JsonConvert.DeserializeObject<Dictionary<string, string>>(filter), StringComparer.OrdinalIgnoreCase);
            string fileName = string.Concat("Report Stok Gudang Sisa", ".xlsx");

            return Tuple.Create(streamExcel, fileName);
        }

        public Tuple<List<GarmentLeftoverWarehouseStockBookkeepingReportViewModel>, int> GetReport(DateTime? dateTo, string type, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(dateTo, type, offset);
            int totaldata = Query.Count();
            return Tuple.Create(Query, totaldata);
        }
    }
}
