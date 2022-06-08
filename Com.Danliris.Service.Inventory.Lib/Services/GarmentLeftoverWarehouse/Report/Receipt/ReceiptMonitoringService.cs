using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Migrations;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Receipt;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Receipt
{
    public class ReceiptMonitoringService : IReceiptMonitoringService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseReceiptFabricMonitoringService";

        public InventoryDbContext DbContext;
        private DbSet<GarmentLeftoverWarehouseReceiptFabric> DbSet;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        private readonly string GarmentCustomsUri;

        public ReceiptMonitoringService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentLeftoverWarehouseReceiptFabric>();

            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));

            //StockService = (IGarmentLeftoverWarehouseStockService)serviceProvider.GetService(typeof(IGarmentLeftoverWarehouseStockService));

            GarmentCustomsUri = APIEndpoint.Purchasing + "garment-beacukai/by-poserialnumber";
        }

        public int TotalCountReport { get; set; } = 0;

        #region FABRIC
        private List<ReceiptMonitoringViewModel> GetFabricReceiptMonitoringQuery(DateTime? dateFrom, DateTime? dateTo, int offset, int page, int size)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            offset = 7;

            List<ReceiptMonitoringViewModel> listData = new List<ReceiptMonitoringViewModel>();

            var join = from a in DbContext.GarmentLeftoverWarehouseReceiptFabrics
                       join b in DbContext.GarmentLeftoverWarehouseReceiptFabricItems on a.Id equals b.GarmentLeftoverWarehouseReceiptFabricId
                       where a._IsDeleted == false
                       && a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date
                       && a.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                       select new
                       {
                           fabricId = a.Id,
                           itemId = b.Id,
                           date = a.ReceiptDate
                       };

            TotalCountReport = join.Distinct().OrderByDescending(o => o.date).Count();
            var queryResult = size != 0 && page != 0 ? join.Distinct().OrderByDescending(o => o.date).Skip((page - 1) * size).Take(size).ToList() : join.Distinct().OrderByDescending(o => o.date).ToList();

            var fabricIds = queryResult.Select(s => s.fabricId).Distinct().ToList();
            var fabrics = DbContext.GarmentLeftoverWarehouseReceiptFabrics.Where(w => fabricIds.Contains(w.Id)).Select(s => new { s.Id, s.ReceiptNoteNo, s.ReceiptDate, s.UnitFromCode, s.UENNo }).ToList();
            var itemIds = queryResult.Select(s => s.itemId).Distinct().ToList();
            var items = DbContext.GarmentLeftoverWarehouseReceiptFabricItems.Where(w => itemIds.Contains(w.Id)).Select(s => new { s.Id, s.POSerialNumber, s.ProductCode, s.ProductName, s.Quantity, s.UomUnit, s.ProductRemark, s.FabricRemark, s.Composition, s.BasicPrice }).ToList();

            int i = ((page - 1) * size) + 1;
            foreach (var item in queryResult)
            {
                var fabric = fabrics.FirstOrDefault(f => f.Id.Equals(item.fabricId));
                var fabricItem = items.FirstOrDefault(f => f.Id.Equals(item.itemId));

                ReceiptMonitoringViewModel vm = new ReceiptMonitoringViewModel();

                vm.ProductRemark = fabricItem.ProductRemark;
                vm.Product = new ProductViewModel { Code = fabricItem.ProductCode, Name = fabricItem.ProductName };
                vm.Quantity = fabricItem.Quantity;
                vm.Uom = new UomViewModel { Unit = fabricItem.UomUnit };
                vm.UnitFrom = new UnitViewModel { Code = fabric.UnitFromCode };
                vm.ReceiptDate = fabric.ReceiptDate;
                vm.ReceiptNoteNo = fabric.ReceiptNoteNo;
                vm.POSerialNumber = fabricItem.POSerialNumber;
                vm.index = i;
                vm.UENNo = fabric.UENNo;
                vm.FabricRemark = fabricItem.FabricRemark;
                vm.Composition = fabricItem.Composition;
                vm.Price = Math.Round(fabricItem.BasicPrice * fabricItem.Quantity,2);

                listData.Add(vm);
                i++;

            }

            listData.ForEach(c => {
                if (!String.IsNullOrEmpty(c.POSerialNumber))
                {
                    var bc = GetBcFromBC(c.POSerialNumber);
                    if (bc != null)
                    {
                        List<string> no = new List<string>();
                        List<string> type = new List<string>();
                        List<DateTimeOffset> date = new List<DateTimeOffset>();
                        foreach (var item in bc)
                        {
                            if (item["POSerialNumber"].ToString() == c.POSerialNumber)
                            {
                                no.Add(item["BeacukaiNo"].ToString());
                                date.Add(DateTimeOffset.Parse(item["BeacukaiDate"].ToString()));
                                type.Add(item["CustomsType"].ToString());
                            }
                        }
                        c.CustomsNo = no;
                        c.CustomsDate = date;
                        c.CustomsType = type;
                    }
                }

            });
            if (size != 0)
            {
                if (page == Math.Ceiling((double)TotalCountReport / (double)size) && TotalCountReport != 0)
                {
                    var QtyTotal = items.Sum(x => x.Quantity);
                    var PriceTotal = Math.Round(items.Sum(x => x.BasicPrice * x.Quantity), 2);
                    ReceiptMonitoringViewModel vm = new ReceiptMonitoringViewModel();

                    vm.ProductRemark = "";
                    vm.Product = new ProductViewModel();
                    vm.Quantity = QtyTotal;
                    vm.Uom = new UomViewModel();
                    vm.UnitFrom = new UnitViewModel();
                    vm.ReceiptDate = DateTimeOffset.MinValue;
                    vm.ReceiptNoteNo = "T O T A L";
                    vm.POSerialNumber = "";
                    vm.index = 0;
                    vm.UENNo = "";
                    vm.FabricRemark = "";
                    vm.Composition = "";
                    vm.Price = PriceTotal;

                    listData.Add(vm);

                }
            }


            return listData;
        }

        public Tuple<List<ReceiptMonitoringViewModel>, int> GetFabricReceiptMonitoring(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Data = GetFabricReceiptMonitoringQuery(dateFrom, dateTo, offset, page, size);


            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            return Tuple.Create(Data, TotalCountReport);
        }

        public Tuple<MemoryStream, string> GenerateExcelFabric(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            //var offset = 7;
            var Query = GetFabricReceiptMonitoringQuery(dateFrom, dateTo, offset, 0, 0);

            var QtyTotal = Query.Sum(x => x.Quantity);
            var PriceTotal = Math.Round(Query.Sum(x => x.Price),2);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon Terima", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Bon terima", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No BUK", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Asal Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Komposisi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Konstruksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Asal BC Masuk", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe Bc", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Bc", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", 0, "", 0, "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string no = "";
                    string type = "";
                    string date = "";
                    if (item.CustomsNo != null)
                    {
                        foreach (var x in item.CustomsNo)
                        {
                            if (no != "")
                            {
                                no += "\n";
                            }
                            no += x;
                        }
                        foreach (var y in item.CustomsType)
                        {
                            if (type != "")
                            {
                                type += "\n";
                            }
                            type += y;
                        }
                        foreach (var z in item.CustomsDate)
                        {
                            if (date != "")
                            {
                                date += "\n";
                            }
                            date += z.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                        }
                    }

                    result.Rows.Add(index, item.ReceiptNoteNo, item.ReceiptDate.ToString("dd MMM yyyy", new CultureInfo("id-ID")),
                        item.UENNo, item.UnitFrom.Code, item.POSerialNumber, item.Product.Name, item.Product.Code, item.Composition, item.FabricRemark, item.Quantity,
                        item.Uom.Unit, item.Price, no, type, date);
                }

                result.Rows.Add("", "T O T A L . . . . ", "",
                     "", "", "", "", "", "", "", QtyTotal,
                      "", PriceTotal, "", "", "");
            }
            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Report Penerimaan Gudang Sisa");
            sheet.Cells["A1"].LoadFromDataTable(result, true, OfficeOpenXml.Table.TableStyles.Light16);

            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            sheet.Cells[sheet.Dimension.Address].Style.WrapText = true;

            MemoryStream streamExcel = new MemoryStream();
            package.SaveAs(streamExcel);

            //Dictionary<string, string> FilterDictionary = new Dictionary<string, string>(JsonConvert.DeserializeObject<Dictionary<string, string>>(filter), StringComparer.OrdinalIgnoreCase);
            string fileName = string.Concat("Report Penerimaan Gudang Sisa - Fabric", DateTime.Now.Date, ".xlsx");

            return Tuple.Create(streamExcel, fileName);
        }

        #endregion

        #region ACCESSORIES
        private List<ReceiptMonitoringViewModel> GetAccessoriesReceiptMonitoringQuery(DateTime? dateFrom, DateTime? dateTo, int offset, int page, int size)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            offset = 7;

            List<ReceiptMonitoringViewModel> listData = new List<ReceiptMonitoringViewModel>();

            var join = from a in DbContext.GarmentLeftoverWarehouseReceiptAccessories
                       join b in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems on a.Id equals b.GarmentLeftOverWarehouseReceiptAccessoriesId
                       where a._IsDeleted == false
                       && a.StorageReceiveDate.AddHours(offset).Date >= DateFrom.Date
                       && a.StorageReceiveDate.AddHours(offset).Date <= DateTo.Date
                       select new
                       {
                           accId = a.Id,
                           itemId = b.Id,
                           date = a.StorageReceiveDate
                       };

            TotalCountReport = join.Distinct().OrderByDescending(o => o.date).Count();
            var queryResult = size != 0 && page != 0 ? join.Distinct().OrderByDescending(o => o.date).Skip((page - 1) * size).Take(size).ToList() : join.Distinct().OrderByDescending(o => o.date).ToList();

            var accIds = queryResult.Select(s => s.accId).Distinct().ToList();
            var accs = DbContext.GarmentLeftoverWarehouseReceiptAccessories.Where(w => accIds.Contains(w.Id)).Select(s => new { s.Id, s.InvoiceNoReceive, s.StorageReceiveDate, s.RequestUnitCode, s.UENNo }).ToList();
            var itemIds = queryResult.Select(s => s.itemId).Distinct().ToList();
            var items = DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems.Where(w => itemIds.Contains(w.Id)).Select(s => new { s.Id, s.POSerialNumber, s.ProductCode, s.ProductName, s.Quantity, s.UomUnit, s.ProductRemark, s.BasicPrice }).ToList();

            int i = ((page - 1) * size) + 1;
            foreach (var item in queryResult)
            {
                var acc = accs.FirstOrDefault(f => f.Id.Equals(item.accId));
                var accItem = items.FirstOrDefault(f => f.Id.Equals(item.itemId));

                ReceiptMonitoringViewModel vm = new ReceiptMonitoringViewModel();

                vm.ProductRemark = accItem.ProductRemark;
                vm.Product = new ProductViewModel { Code = accItem.ProductCode, Name = accItem.ProductName };
                vm.Quantity = accItem.Quantity;
                vm.Uom = new UomViewModel { Unit = accItem.UomUnit };
                vm.UnitFrom = new UnitViewModel { Code = acc.RequestUnitCode };
                vm.ReceiptDate = acc.StorageReceiveDate;
                vm.ReceiptNoteNo = acc.InvoiceNoReceive;
                vm.POSerialNumber = accItem.POSerialNumber;
                vm.index = i;
                vm.UENNo = acc.UENNo;
                vm.Price = Math.Round(accItem.BasicPrice * accItem.Quantity,2);

                listData.Add(vm);
                i++;

            }

            listData.ForEach(c => {
                if (!String.IsNullOrEmpty(c.POSerialNumber))
                {
                    var bc = GetBcFromBC(c.POSerialNumber);
                    if (bc != null)
                    {
                        List<string> no = new List<string>();
                        List<string> type = new List<string>();
                        List<DateTimeOffset> date = new List<DateTimeOffset>();
                        foreach (var item in bc)
                        {
                            if (item["POSerialNumber"].ToString() == c.POSerialNumber)
                            {
                                no.Add(item["BeacukaiNo"].ToString());
                                date.Add(DateTimeOffset.Parse(item["BeacukaiDate"].ToString()));
                                type.Add(item["CustomsType"].ToString());
                            }
                        }
                        c.CustomsNo = no;
                        c.CustomsDate = date;
                        c.CustomsType = type;
                    }
                }

            });
            if (size != 0)
            {
                if (page == Math.Ceiling((double)TotalCountReport / (double)size) && listData.Count() != 0)
                {
                    var QtyTotal = listData.Sum(x => x.Quantity);
                    var PriceTotal = Math.Round(listData.Sum(x => x.Price),2);
                    ReceiptMonitoringViewModel vm = new ReceiptMonitoringViewModel();

                    vm.ProductRemark = "";
                    vm.Product = new ProductViewModel();
                    vm.Quantity = QtyTotal;
                    vm.Uom = new UomViewModel();
                    vm.UnitFrom = new UnitViewModel();
                    vm.ReceiptDate = DateTimeOffset.MinValue;
                    vm.ReceiptNoteNo = "T O T A L";
                    vm.POSerialNumber = "";
                    vm.index = 0;
                    vm.UENNo = "";
                    vm.FabricRemark = "";
                    vm.Composition = "";
                    vm.Price = PriceTotal;

                    listData.Add(vm);

                }
            }


            return listData;
        }

        public Tuple<List<ReceiptMonitoringViewModel>, int> GetAccessoriesReceiptMonitoring(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Data = GetAccessoriesReceiptMonitoringQuery(dateFrom, dateTo, offset, page, size);

            Data.ForEach(c => {
                if (!String.IsNullOrEmpty(c.POSerialNumber))
                {
                    var bc = GetBcFromBC(c.POSerialNumber);
                    if (bc != null)
                    {
                        List<string> no = new List<string>();
                        List<string> type = new List<string>();
                        List<DateTimeOffset> date = new List<DateTimeOffset>();
                        foreach (var item in bc)
                        {
                            if (item["POSerialNumber"].ToString() == c.POSerialNumber)
                            {
                                no.Add(item["BeacukaiNo"].ToString());
                                date.Add(DateTimeOffset.Parse(item["BeacukaiDate"].ToString()));
                                type.Add(item["CustomsType"].ToString());
                            }
                        }
                        c.CustomsNo = no;
                        c.CustomsDate = date;
                        c.CustomsType = type;
                    }
                }

            });
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            return Tuple.Create(Data, TotalCountReport);
        }


        public Tuple<MemoryStream, string> GenerateExcelAccessories(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            //var offset = 7;

            var Query = GetAccessoriesReceiptMonitoringQuery(dateFrom, dateTo, offset, 0, 0);

            var QtyTotal = Query.Sum(x => x.Quantity);
            var PriceTotal = Math.Round(Query.Sum(x => x.Price),2);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon Terima", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Bon terima", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No BUK", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Asal Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Asal BC Masuk", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe Bc", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Bc", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", 0, "", 0, "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string no = "";
                    string type = "";
                    string date = "";

                    if (item.CustomsNo != null)
                    {
                        foreach (var x in item.CustomsNo)
                        {
                            if (no != "")
                            {
                                no += "\n";
                            }
                            no += x;
                        }
                        foreach (var y in item.CustomsType)
                        {
                            if (type != "")
                            {
                                type += "\n";
                            }
                            type += y;
                        }
                        foreach (var z in item.CustomsDate)
                        {
                            if (date != "")
                            {
                                date += "\n";
                            }
                            date += z.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                        }
                    }

                    result.Rows.Add(index, item.ReceiptNoteNo, item.ReceiptDate.ToString("dd MMM yyyy", new CultureInfo("id-ID")),
                        item.UENNo, item.UnitFrom.Code, item.POSerialNumber, item.Product.Name, item.Product.Code, item.ProductRemark, item.Quantity,
                        item.Uom.Unit, item.Price, no, type, date);
                }

                result.Rows.Add("", "T O T A L . . . . ", "",
                      "", "", "", "", "", "", QtyTotal,
                       "", PriceTotal, "", "", "");
            }
            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Report Penerimaan Gudang Sisa");
            sheet.Cells["A1"].LoadFromDataTable(result, true, OfficeOpenXml.Table.TableStyles.Light16);

            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            sheet.Cells[sheet.Dimension.Address].Style.WrapText = true;

            MemoryStream streamExcel = new MemoryStream();
            package.SaveAs(streamExcel);

            //Dictionary<string, string> FilterDictionary = new Dictionary<string, string>(JsonConvert.DeserializeObject<Dictionary<string, string>>(filter), StringComparer.OrdinalIgnoreCase);
            string fileName = string.Concat("Report Penerimaan Gudang Sisa - Accessories ", DateTime.Now.Date, ".xlsx");

            return Tuple.Create(streamExcel, fileName);
        }
        #endregion



        private List<Dictionary<string, object>> GetBcFromBC(string POSerialNumber)
        {
            var httpService = (IHttpService)ServiceProvider.GetService(typeof(IHttpService));

            Dictionary<string, object> filterLocalCoverLetter = new Dictionary<string, object> { { "POSerialNumber", POSerialNumber } };
            var filter = JsonConvert.SerializeObject(filterLocalCoverLetter);
            var responseLocalCoverLetter = httpService.GetAsync($"{GarmentCustomsUri}?filter=" + filter).Result.Content.ReadAsStringAsync();

            Dictionary<string, object> resultLocalCoverLetter = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseLocalCoverLetter.Result);
            var jsonLocalCoverLetter = resultLocalCoverLetter.Single(p => p.Key.Equals("data")).Value;
            var a = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonLocalCoverLetter.ToString());
            if (a.Count > 0)
            {
                //Dictionary<string, object> dataLocalCoverLetter = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonLocalCoverLetter.ToString())[0];
                return a;
            }
            return null;
        }

    }
}
