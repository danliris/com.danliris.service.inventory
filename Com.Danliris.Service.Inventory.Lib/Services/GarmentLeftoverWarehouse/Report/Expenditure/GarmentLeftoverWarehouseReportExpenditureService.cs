using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Expenditure;
using Com.Moonlay.NetCore.Lib;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Expenditure
{
    public class GarmentLeftoverWarehouseReportExpenditureService : IGarmentLeftoverWarehouseReportExpenditureService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseReportExpenditureService";
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;
        private readonly string GarmentShippingLocalCoverLetterUri;
        private readonly string GarmentCoreProductUri;

        public GarmentLeftoverWarehouseReportExpenditureService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            IdentityService = serviceProvider.GetService<IIdentityService>();
            GarmentShippingLocalCoverLetterUri = APIEndpoint.PackingInventory + "garment-shipping/local-cover-letters";
            GarmentCoreProductUri = APIEndpoint.Core + "master/garmentProducts";
        }

        public Tuple<List<GarmentLeftoverWarehouseReportExpenditureViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, string receiptType, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, receiptType, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b._LastModifiedUtc);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];
                Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<GarmentLeftoverWarehouseReportExpenditureViewModel> pageable = new Pageable<GarmentLeftoverWarehouseReportExpenditureViewModel>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseReportExpenditureViewModel> Data = pageable.Data.ToList<GarmentLeftoverWarehouseReportExpenditureViewModel>();

            Data.ForEach(c => {
                if(!String.IsNullOrEmpty(c.LocalSalesNoteNo))
                {
                    var salesNotes = GetBcFromShipping(c.LocalSalesNoteNo);
                    
                    if(salesNotes != null)
                    {
                        if (salesNotes["noteNo"].ToString() == c.LocalSalesNoteNo)
                        {
                            c.BCNo = salesNotes["bcNo"].ToString();
                            c.BCDate = DateTimeOffset.Parse(salesNotes["bcDate"].ToString());
                            c.BCType = "BC 25";
                        }
                    } 
                }
                var garmentProduct = GetProductFromCore(c.Product.Id);
                if (garmentProduct != null)
                {
                    c.Composition = garmentProduct["Composition"].ToString();
                    c.Const = receiptType == "ACCESSORIES" ? "-" : garmentProduct["Const"].ToString() + "; " + garmentProduct["Yarn"].ToString() + "; " + garmentProduct["Width"].ToString();
                }
            });


            

            int TotalData = pageable.TotalCount;
            
            if (page == ((TotalData / size) + 1) && TotalData != 0)
            {
                var QtyTotal = Query.Sum(x => x.Quantity);
                var ExpendQtyTotal = Query.Sum(x => x.QtyKG);
                GarmentLeftoverWarehouseReportExpenditureViewModel vm = new GarmentLeftoverWarehouseReportExpenditureViewModel();

                vm.ExpenditureNo = "T O T A L";
                vm.ExpenditureDate = DateTimeOffset.MinValue;
                vm.ExpenditureDestination = "";
                vm.DescriptionOfPurpose = "";
                vm.Buyer = new BuyerViewModel();
                vm.UnitExpenditure = new UnitViewModel();
                vm.EtcRemark = "";
                vm.PONo = "";
                vm.Product = new ProductViewModel();
                vm.ProductRemark = "";
                vm.Quantity = QtyTotal;
                vm.Uom = new UomViewModel();
                vm.LocalSalesNoteNo = "";
                vm.BCNo = "";
                vm.BCType = "";
                vm.BCDate = null;
                vm.QtyKG = ExpendQtyTotal;
                vm.Composition = "";
                vm.Const = "";

                Data.Add(vm);

            }


            return Tuple.Create(Data, TotalData);
        }

        public IQueryable<GarmentLeftoverWarehouseReportExpenditureViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, string receiptType, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            IQueryable<GarmentLeftoverWarehouseReportExpenditureViewModel> Query = null;
            IQueryable<GarmentLeftoverWarehouseReportExpenditureViewModel> QueryFabric = null;
            IQueryable<GarmentLeftoverWarehouseReportExpenditureViewModel> QueryAcc = null;


            if (receiptType == "FABRIC")
            {
                QueryFabric = (from a in DbContext.GarmentLeftoverWarehouseExpenditureFabrics
                               join b in DbContext.GarmentLeftoverWarehouseExpenditureFabricItems on a.Id equals b.ExpenditureId
                               from c in DbContext.GarmentLeftoverWarehouseStocks
                                .Where(o => b.PONo == o.PONo).Take(1)
                                .DefaultIfEmpty()
                               where a._IsDeleted == false
                                 && a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date
                                 && a.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                               select new GarmentLeftoverWarehouseReportExpenditureViewModel
                                {
                                     ExpenditureNo = a.ExpenditureNo,
                                     ExpenditureDate = a.ExpenditureDate,
                                     ExpenditureDestination = a.ExpenditureDestination,
                                     DescriptionOfPurpose = a.ExpenditureDestination == "JUAL LOKAL" ? a.BuyerName : a.ExpenditureDestination == "UNIT" ? a.UnitExpenditureCode : a.ExpenditureDestination == "LAIN-LAIN" ? a.EtcRemark : "SAMPLE",
                                     PONo = b.PONo,
                                     Product = new ProductViewModel
                                     {
                                         Id = Convert.ToString(c.ProductId),
                                         Code = c.ProductCode,
                                         Name = c.ProductName,
                                     },
                                     Quantity = b.Quantity,
                                     Uom = new UomViewModel
                                     {
                                         Id = Convert.ToString(b.UomId),
                                         Unit = b.UomUnit
                                     },
                                     QtyKG = a.QtyKG,
                                     LocalSalesNoteNo = a.LocalSalesNoteNo,
                                     _LastModifiedUtc = a._LastModifiedUtc
                                });
                Query = QueryFabric;
            } 
            if(receiptType == "ACCESSORIES")
            {
                QueryAcc = (from a in DbContext.GarmentLeftoverWarehouseExpenditureAccessories
                            join b in DbContext.GarmentLeftoverWarehouseExpenditureAccessoriesItems on a.Id equals b.ExpenditureId
                            from c in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems
                             .Where(o => b.PONo == o.POSerialNumber).Take(1)
                             .DefaultIfEmpty()
                            where a._IsDeleted == false
                                 && a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date
                                 && a.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                             select new GarmentLeftoverWarehouseReportExpenditureViewModel
                             {
                                 ExpenditureNo = a.ExpenditureNo,
                                 ExpenditureDate = a.ExpenditureDate,
                                 ExpenditureDestination = a.ExpenditureDestination,
                                 DescriptionOfPurpose = a.ExpenditureDestination == "JUAL LOKAL" ? a.BuyerName : a.ExpenditureDestination == "UNIT" ? a.UnitExpenditureCode : a.ExpenditureDestination == "LAIN-LAIN" ? a.EtcRemark : "SAMPLE",
                                 PONo = b.PONo,
                                 Product = new ProductViewModel
                                 {
                                     Id = c!=null ? Convert.ToString(c.ProductId) : DbContext.GarmentLeftoverWarehouseBalanceStocksItems.Where(aa => aa.PONo == b.PONo).Select(aa => aa.ProductId.ToString()).FirstOrDefault(),
                                     Code = c != null ? c.ProductCode : DbContext.GarmentLeftoverWarehouseBalanceStocksItems.Where(aa => aa.PONo == b.PONo).Select(aa => aa.ProductCode).FirstOrDefault(),
                                     Name = c != null ? c.ProductName : DbContext.GarmentLeftoverWarehouseBalanceStocksItems.Where(aa => aa.PONo == b.PONo).Select(aa => aa.ProductName).FirstOrDefault(),
                                 },
                                 ProductRemark = c.ProductRemark!= null ? c.ProductRemark : DbContext.GarmentLeftoverWarehouseBalanceStocksItems.Where(aa=> aa.PONo == b.PONo).Select(aa=>aa.ProductRemark).FirstOrDefault(),
                                 Quantity = b.Quantity,
                                 Uom = new UomViewModel
                                 {
                                     Id = Convert.ToString(b.UomId),
                                     Unit = b.UomUnit
                                 },
                                 LocalSalesNoteNo = a.LocalSalesNoteNo,
                                 _LastModifiedUtc = a._LastModifiedUtc
                             });
                Query = QueryAcc;
            } 
            if (string.IsNullOrEmpty(receiptType))
            {
                QueryFabric = (from a in DbContext.GarmentLeftoverWarehouseExpenditureFabrics
                               join b in DbContext.GarmentLeftoverWarehouseExpenditureFabricItems on a.Id equals b.ExpenditureId
                               join c in DbContext.GarmentLeftoverWarehouseReceiptFabricItems on b.PONo equals c.POSerialNumber
                               where a._IsDeleted == false
                                   && a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date
                                   && a.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                               select new GarmentLeftoverWarehouseReportExpenditureViewModel
                               {
                                   ExpenditureNo = a.ExpenditureNo,
                                   ExpenditureDate = a.ExpenditureDate,
                                   ExpenditureDestination = a.ExpenditureDestination,
                                   DescriptionOfPurpose = a.ExpenditureDestination == "JUAL LOKAL" ? a.BuyerName : a.ExpenditureDestination == "UNIT" ? a.UnitExpenditureCode : a.ExpenditureDestination == "LAIN-LAIN" ? a.EtcRemark : "SAMPLE",
                                   PONo = b.PONo,
                                   Product = new ProductViewModel
                                   {
                                       Id = Convert.ToString(c.ProductId),
                                       Code = c.ProductCode,
                                       Name = c.ProductName,
                                   },
                                   ProductRemark = c.ProductRemark,
                                   Quantity = b.Quantity,
                                   Uom = new UomViewModel
                                   {
                                       Id = Convert.ToString(b.UomId),
                                       Unit = b.UomUnit
                                   },
                                   LocalSalesNoteNo = a.LocalSalesNoteNo,
                                   _LastModifiedUtc = a._LastModifiedUtc
                               });
                QueryAcc = (from a in DbContext.GarmentLeftoverWarehouseExpenditureAccessories
                            join b in DbContext.GarmentLeftoverWarehouseExpenditureAccessoriesItems on a.Id equals b.ExpenditureId
                            join c in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems on b.PONo equals c.POSerialNumber
                            where a._IsDeleted == false
                                && a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date
                                && a.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                            select new GarmentLeftoverWarehouseReportExpenditureViewModel
                            {
                                ExpenditureNo = a.ExpenditureNo,
                                ExpenditureDate = a.ExpenditureDate,
                                ExpenditureDestination = a.ExpenditureDestination,
                                DescriptionOfPurpose = a.ExpenditureDestination == "JUAL LOKAL" ? a.BuyerName : a.ExpenditureDestination == "UNIT" ? a.UnitExpenditureCode : a.ExpenditureDestination == "LAIN-LAIN" ? a.EtcRemark : "SAMPLE",
                                PONo = b.PONo,
                                Product = new ProductViewModel
                                {
                                    Id = Convert.ToString(c.ProductId),
                                    Code = c.ProductCode,
                                    Name = c.ProductName,
                                },
                                ProductRemark = c.ProductRemark,
                                Quantity = b.Quantity,
                                Uom = new UomViewModel
                                {
                                    Id = Convert.ToString(b.UomId),
                                    Unit = b.UomUnit
                                },
                                LocalSalesNoteNo = a.LocalSalesNoteNo,
                                _LastModifiedUtc = a._LastModifiedUtc
                            });

                Query = QueryFabric.Concat(QueryAcc);
            }
            


            return Query.Distinct();
        }

        private Dictionary<string, Object> GetBcFromShipping(string localSalesNoteNo)
        {
            var httpService = (IHttpService)ServiceProvider.GetService(typeof(IHttpService));

            Dictionary<string, object> filterLocalCoverLetter = new Dictionary<string, object> { { "NoteNo", localSalesNoteNo } };
            var filter = JsonConvert.SerializeObject(filterLocalCoverLetter);
            var responseLocalCoverLetter = httpService.GetAsync($"{GarmentShippingLocalCoverLetterUri}?filter=" + filter).Result.Content.ReadAsStringAsync();

            Dictionary<string, object> resultLocalCoverLetter = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseLocalCoverLetter.Result);
            var jsonLocalCoverLetter = resultLocalCoverLetter.Single(p => p.Key.Equals("data")).Value;
            var a = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonLocalCoverLetter.ToString());
            if (a.Count > 0)
            {
                Dictionary<string, object> dataLocalCoverLetter = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonLocalCoverLetter.ToString())[0];
                return dataLocalCoverLetter;
            }
            return null;
        }
        
        private Dictionary<string, Object> GetProductFromCore(string productId)
        {
            var httpService = (IHttpService)ServiceProvider.GetService(typeof(IHttpService));
            var responseGarmentProduct = httpService.GetAsync($"{GarmentCoreProductUri}/" + productId).Result.Content.ReadAsStringAsync();

            Dictionary<string, object> resultGarmentProduct = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseGarmentProduct.Result);
            var jsonLocalCoverLetter = resultGarmentProduct.Single(p => p.Key.Equals("data")).Value;
            var a = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonLocalCoverLetter.ToString());
            //if (a.Count > 0)
            //{
                Dictionary<string, object> dataLocalCoverLetter = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonLocalCoverLetter.ToString());
                return dataLocalCoverLetter;
            //}
           // return null;

        }

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, string receiptType, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, receiptType, offset);
            Query = Query.OrderByDescending(b => b._LastModifiedUtc);

            var QtyTotal = Query.Sum(x => x.Quantity);
            var ExpendQtyTotal = Query.Sum(x => x.QtyKG);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Bon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tujuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Tujuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Keluar (KG)", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Komposisi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Konstruksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Nota Penjualan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bc Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe Bc", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Bc", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "",0, "", "", "", "", "", 0, "", "", 0, "",""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    var garmentProduct = GetProductFromCore(item.Product.Id);
                    if (garmentProduct != null)
                    {
                        item.Composition = garmentProduct["Composition"].ToString();
                        item.Const = receiptType == "ACCESSORIES" ? "-" : garmentProduct["Const"].ToString() + "; " + garmentProduct["Yarn"].ToString() + "; " + garmentProduct["Width"].ToString();
                    }
                    result.Rows.Add(
                        index, 
                        item.ExpenditureNo, 
                        item.ExpenditureDate.ToString("dd MMM yyyy", new CultureInfo("id-ID")), 
                        item.ExpenditureDestination, 
                        item.DescriptionOfPurpose, 
                        item.QtyKG,
                        item.Composition,
                        item.Const,
                        item.PONo, 
                        item.Product.Name, 
                        item.Product.Code, 
                        item.Quantity, 
                        item.Uom.Unit, 
                        item.LocalSalesNoteNo, 
                        item.BCNo, 
                        item.BCType, 
                        item.BCDate?.ToString("dd MMM yyyy", new CultureInfo("id-ID")));
                }

                result.Rows.Add(
                         "",
                         "T O T A L .......",
                         "",
                         "",
                         "",
                         ExpendQtyTotal,
                         "",
                         "",
                         "",
                         "",
                         "",
                         QtyTotal,
                         "",
                         "",
                         "",
                         "",
                         "");
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Report Pengeluaran Gudang Sisa") }, true);
        }
    }
}
