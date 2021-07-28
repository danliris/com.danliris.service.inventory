using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Bookkeeping;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Stock;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Bookkeeping
{
   public class GarmentLeftoverWarehouseFlowStockReportService : IGarmentLeftoverWarehouseFlowStockReportService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseFlowStockReportService";

        private InventoryDbContext DbContext;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        public GarmentLeftoverWarehouseFlowStockReportService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
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
        #region REPORT
        public List<GarmentLeftoverWarehouseFlowStockMonitoringViewModel> GetReportQuery(string category,DateTime? dateFrom, DateTime? dateTo, int UnitId,  int offset, string typeAval = "")
        {

            DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
            DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;


            List<GarmentLeftoverWarehouseFlowStockMonitoringViewModel> GarmentLeftoverWarehouseFlowStockMonitoringViewModel = new List<GarmentLeftoverWarehouseFlowStockMonitoringViewModel>();
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
            if (category == "FABRIC")
            {

               
                var QueryBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                              where data._IsDeleted == false && data.TypeOfGoods.ToString() == "FABRIC"
                                              select new { data._CreatedUtc, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
                                   where b.UnitId == (UnitId == 0 ? b.UnitId : UnitId)
                                   select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
                                   {
                                       PONo = b.PONo,
                                       BeginingbalanceQty = b.Quantity,
                                       BeginingbalancePrice = b.Quantity * b.BasicPrice,
                                       QuantityReceipt=0,
                                       PriceReceipt = 0,
                                       QuantityUnitExpend = 0,
                                       PriceUnitExpend = 0,
                                       QuantityLocalExpend=0,
                                       PriceLocalExpend = 0,
                                       QuantityOtherExpend=0,
                                       PriceOtherExpend =0,
                                       QuantitySampleExpend = 0,
                                       PriceSampleExpend =0,
                                       UomUnit = b.UomUnit,
                                       UnitName = b.UnitName,
                                       ProductCode = b.ProductCode,
                                       ProductName = b.ProductName,
                                       EndbalanceQty = 0,
                                       BasicPrice = b.BasicPrice,
                                   };
                var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFabrics
                                              where data._IsDeleted == false
                                         && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                                         && data.UnitFromId == (UnitId == 0 ? data.UnitFromId : UnitId)
                                              select new { data.UnitFromName, data.ReceiptDate, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseReceiptFabricItems on a.Id equals b.GarmentLeftoverWarehouseReceiptFabricId
                                   select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
                                   {
                                       PONo = b.POSerialNumber,
                                       BeginingbalanceQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date ? b.Quantity : 0,
                                       BeginingbalancePrice = (a.ReceiptDate.AddHours(offset).Date < DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
                                       QuantityReceipt = a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date ? b.Quantity : 0,
                                       PriceReceipt= (a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
                                       BasicPrice= b.BasicPrice,
                                       QuantityUnitExpend = 0,
                                       PriceUnitExpend=0,
                                       QuantityLocalExpend = 0,
                                       PriceLocalExpend =0,
                                       QuantityOtherExpend = 0,
                                       PriceOtherExpend =0,
                                       QuantitySampleExpend = 0,
                                       PriceSampleExpend =0,
                                       UomUnit = b.UomUnit,
                                       UnitName = a.UnitFromName,
                                       ProductCode = b.ProductCode,
                                       ProductName = b.ProductName,
                                       EndbalanceQty = 0
                                     
                                   };
                var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFabrics
                                                  where data._IsDeleted == false
                                             && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date

                                                  select new { data.UnitExpenditureCode, data.ExpenditureDate, data.Id,data.ExpenditureDestination })
                                       join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFabricItems
                                                  where expend.UnitId == (UnitId == 0 ? expend.UnitId : UnitId)
                                                  select new { expend.BasicPrice,expend.UomUnit, expend.PONo, expend.Quantity, expend.UnitName, expend.ExpenditureId }) on a.Id equals b.ExpenditureId
                                       select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
                                       {
                                           PONo = b.PONo,
                                           BeginingbalanceQty = a.ExpenditureDate.AddHours(offset).Date  < DateFrom.Date ? -b.Quantity : 0,
                                           BeginingbalancePrice = (a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date ? -b.Quantity : 0)* b.BasicPrice,
                                           QuantityReceipt = 0,
                                           PriceReceipt =0,
                                           QuantityUnitExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "UNIT" ? b.Quantity : 0,
                                           PriceUnitExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "UNIT" ? b.Quantity : 0)* b.BasicPrice,
                                           QuantityLocalExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "JUAL LOKAL" ? b.Quantity : 0,
                                           PriceLocalExpend =( a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "JUAL LOKAL" ? b.Quantity : 0)* b.BasicPrice,
                                           QuantityOtherExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "LAIN-LAIN" ? b.Quantity : 0,
                                           PriceOtherExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "LAIN-LAIN" ? b.Quantity : 0) * b.BasicPrice,
                                           QuantitySampleExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "SAMPLE" ? b.Quantity : 0,
                                           PriceSampleExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "SAMPLE" ? b.Quantity : 0)* b.BasicPrice,
                                           BasicPrice= b.BasicPrice,
                                           UomUnit = b.UomUnit,
                                           UnitName = b.UnitName,
                                           ProductCode = (from aa in _product where aa.PONo == b.PONo select aa.ProductCode).FirstOrDefault() ,
                                           ProductName = (from aa in _product where aa.PONo == b.PONo select aa.ProductName).FirstOrDefault() ,
                                           EndbalanceQty = 0
                                       };
                 var Query = QueryReceipt.Union(QueryExpenditure).Union(QueryBalance);
                var querySum = Query.ToList()
                    .GroupBy(x => new { x.PONo, x.UnitName, x.UomUnit,x.ProductCode,x.ProductName }, (key, group) => new
                    {
                        pono = key.PONo,
                        begining = group.Sum(s => s.BeginingbalanceQty),
                        beginingPrice= group.Sum(s=>s.BeginingbalancePrice),
                        expendunit = group.Sum(s => s.QuantityUnitExpend),
                        expendunitPrice= group.Sum(s=>s.PriceUnitExpend),
                        expendsample = group.Sum(s=> s.QuantitySampleExpend),
                        expendsamplePrice = group.Sum(s => s.PriceSampleExpend),
                        expendother = group.Sum(s => s.QuantityOtherExpend),
                        expendotherPrice = group.Sum(s => s.PriceOtherExpend),
                        expendlocal = group.Sum(s => s.QuantityLocalExpend),
                        expendlocalPrice = group.Sum(s => s.PriceLocalExpend),
                        receipt = group.Sum(s => s.QuantityReceipt),
                        priceReceipt= group.Sum(s => s.PriceReceipt),
                        basicprice = group.Sum(s=>s.BasicPrice),
                        uomunit = key.UomUnit,
                        unit = key.UnitName,
                        productCode= key.ProductCode,
                        productName= key.ProductName
                    }).OrderBy(s => s.pono);
                 
                foreach (var data in querySum)
                {
                    
                    GarmentLeftoverWarehouseFlowStockMonitoringViewModel garmentLeftover = new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
                    {
                        PONo = data.pono,
                        BeginingbalanceQty = data.begining,
                        BeginingbalancePrice =data.beginingPrice,
                        QuantityReceipt = data.receipt,
                        PriceReceipt =data.priceReceipt,
                        BasicPrice = data.basicprice,
                        QuantityUnitExpend = data.expendunit,
                        PriceUnitExpend=data.expendunitPrice,
                        QuantityLocalExpend = data.expendlocal,
                        PriceLocalExpend = data.expendlocalPrice,
                        QuantityOtherExpend = data.expendother,
                        PriceOtherExpend = data.expendotherPrice,
                        QuantitySampleExpend = data.expendsample,
                        PriceSampleExpend = data.expendsamplePrice,
                        UnitName = data.unit,
                        UomUnit = data.uomunit,
                        ProductCode = data.productCode,
                        ProductName = data.productName,
                        
                        EndbalanceQty = data.begining + data.receipt - data.expendlocal - data.expendother - data.expendsample-data.expendunit,
                        EndbalancePrice = data.beginingPrice + data.priceReceipt - data.expendlocalPrice - data.expendotherPrice - data.expendsamplePrice - data.expendunitPrice
                    };
                    GarmentLeftoverWarehouseFlowStockMonitoringViewModel.Add(garmentLeftover);
                }
            }
            else if (category == "BARANG JADI")
            {

                var QueryBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                              where data._IsDeleted == false && data.TypeOfGoods.ToString() == "BARANG JADI"
                                              select new { data._CreatedUtc, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
                                   where b.UnitId == (UnitId == 0 ? b.UnitId : UnitId)
                                   select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
                                   {
                                       RO = b.RONo,
                                       BeginingbalanceQty= b.Quantity,
                                       BeginingbalancePrice = b.Quantity * b.BasicPrice,
                                       QuantityReceipt = 0,
                                       PriceReceipt = 0,
                                       QuantityUnitExpend = 0,
                                       PriceUnitExpend = 0,
                                       QuantityLocalExpend = 0,
                                       PriceLocalExpend = 0,
                                       QuantityOtherExpend = 0,
                                       PriceOtherExpend = 0,
                                       QuantitySampleExpend = 0,
                                       PriceSampleExpend = 0,
                                       UomUnit = b.UomUnit,
                                       UnitName = b.UnitName,
                                       ProductCode = b.LeftoverComodityCode,
                                       ProductName = b.LeftoverComodityName,
                                       EndbalanceQty = 0
                                   };
                var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoods
                                              where data._IsDeleted == false
                                         && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                                         && data.UnitFromId == (UnitId == 0 ? data.UnitFromId : UnitId)
                                              select new { data.UnitFromName, data.ReceiptDate, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoodItems on a.Id equals b.FinishedGoodReceiptId
                                   select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
                                   {
                                       RO = b.RONo,
                                       BeginingbalanceQty = a.ReceiptDate.AddHours(offset) < DateFrom.Date ? b.Quantity : 0,
                                       BeginingbalancePrice = (a.ReceiptDate.AddHours(offset) < DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
                                       QuantityReceipt = a.ReceiptDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0,                                       
                                       PriceReceipt = (a.ReceiptDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
                                       QuantityUnitExpend = 0,
                                       PriceUnitExpend = 0,
                                       QuantityLocalExpend = 0,
                                       PriceLocalExpend = 0,
                                       QuantityOtherExpend = 0,
                                       PriceOtherExpend = 0,
                                       QuantitySampleExpend = 0,
                                       PriceSampleExpend = 0,
                                       BasicPrice =0,
                                       UomUnit = "PCS",
                                       UnitName = a.UnitFromName,
                                       ProductCode = b.LeftoverComodityCode,
                                       ProductName = b.LeftoverComodityName,
                                       EndbalanceQty = 0
                                   };
                var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoods
                                                  where data._IsDeleted == false
                                                        && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                                                  select new { data.ExpenditureDate, data.Id,data.ExpenditureTo })
                                       join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoodItems
                                                  where expend.UnitId == (UnitId == 0 ? expend.UnitId : UnitId)
                                                  select new {expend.BasicPrice, expend.FinishedGoodExpenditureId, expend.UnitName, expend.ExpenditureQuantity,expend.LeftoverComodityCode, expend.RONo, expend.LeftoverComodityName }
                                                  ) on a.Id equals b.FinishedGoodExpenditureId
                                       select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
                                       {
                                           RO = b.RONo,
                                           BeginingbalanceQty = a.ExpenditureDate.AddHours(offset) < DateFrom.Date ? -b.ExpenditureQuantity : 0,
                                           BeginingbalancePrice = (a.ExpenditureDate.AddHours(offset) < DateFrom.Date ? -b.ExpenditureQuantity : 0) * b.BasicPrice,
                                           QuantityReceipt = 0,
                                           PriceReceipt=0,
                                           BasicPrice =0,
                                           QuantityUnitExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureTo == "UNIT" ? b.ExpenditureQuantity : 0,
                                           PriceUnitExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureTo == "UNIT" ? b.ExpenditureQuantity : 0) * b.BasicPrice,
                                           QuantityLocalExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureTo == "JUAL LOKAL" ? b.ExpenditureQuantity : 0,
                                           PriceLocalExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureTo == "JUAL LOKAL" ? b.ExpenditureQuantity : 0)* b.BasicPrice,
                                           QuantityOtherExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureTo == "LAIN-LAIN" ? b.ExpenditureQuantity : 0,
                                           PriceOtherExpend = (a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureTo == "LAIN-LAIN" ? b.ExpenditureQuantity : 0)*b.BasicPrice,
                                           QuantitySampleExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureTo == "SAMPLE" ? b.ExpenditureQuantity : 0,
                                           PriceSampleExpend =( a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureTo == "SAMPLE" ? b.ExpenditureQuantity : 0) * b.BasicPrice,
                                           UomUnit = "PCS",
                                           UnitName = b.UnitName,
                                           ProductCode = b.LeftoverComodityCode,
                                           ProductName = b.LeftoverComodityName,
                                           EndbalanceQty = 0
                                       };
                var Query = QueryReceipt.Union(QueryExpenditure).Union(QueryBalance);
                var querySum = Query.ToList()
                    .GroupBy(x => new { x.RO, x.UnitName, x.UomUnit,x.ProductCode,x.ProductName }, (key, group) => new
                    {
                        rono = key.RO,
                        begining = group.Sum(s => s.BeginingbalanceQty),
                        beginingPrice = group.Sum(s => s.BeginingbalancePrice),
                        expendunit = group.Sum(s => s.QuantityUnitExpend),
                        expendunitPrice = group.Sum(s => s.PriceUnitExpend),
                        expendsample = group.Sum(s => s.QuantitySampleExpend),
                        expendsamplePrice = group.Sum(s => s.PriceSampleExpend),
                        expendother = group.Sum(s => s.QuantityOtherExpend),
                        expendotherPrice = group.Sum(s => s.PriceOtherExpend),
                        expendlocal = group.Sum(s => s.QuantityLocalExpend),
                        expendlocalPrice = group.Sum(s => s.PriceLocalExpend),
                        receipt = group.Sum(s => s.QuantityReceipt),
                        priceReceipt = group.Sum(s => s.PriceReceipt),
                        basicprice = group.Sum(s => s.BasicPrice),
                        uomunit = key.UomUnit,
                        unit = key.UnitName ,
                        productCode= key.ProductCode,
                        productName= key.ProductName
                    }).OrderBy(s => s.rono);


                foreach (var data in querySum)
                {
                    GarmentLeftoverWarehouseFlowStockMonitoringViewModel garmentLeftover = new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
                    {
                        RO = data.rono,
                        BeginingbalanceQty = data.begining,
                        BeginingbalancePrice = data.beginingPrice,
                        QuantityReceipt = data.receipt,
                        PriceReceipt= data.priceReceipt,
                        BasicPrice = data.basicprice,
                        QuantityUnitExpend = data.expendunit,
                        PriceUnitExpend= data.expendunitPrice,
                        QuantityLocalExpend = data.expendlocal,
                        PriceLocalExpend=data.expendlocalPrice,
                        QuantityOtherExpend = data.expendother,
                        PriceOtherExpend=data.expendotherPrice,
                        QuantitySampleExpend = data.expendsample,
                        PriceSampleExpend= data.expendsamplePrice,
                        UnitName = data.unit,
                        UomUnit = data.uomunit,
                        ProductCode = data.productCode,
                        ProductName = data.productName,
                        EndbalanceQty = data.begining + data.receipt - data.expendlocal-data.expendother-data.expendsample-data.expendunit,
                        EndbalancePrice = data.beginingPrice + data.priceReceipt - data.expendlocalPrice - data.expendotherPrice - data.expendsamplePrice - data.expendunitPrice

                    };
                    GarmentLeftoverWarehouseFlowStockMonitoringViewModel.Add(garmentLeftover);
                }
            }
            else
            {
                var QueryBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                              where data._IsDeleted == false && data.TypeOfGoods.ToString() == "ACCESSORIES"
                                              select new { data._CreatedUtc, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
                                   where b.UnitId == (UnitId == 0 ? b.UnitId : UnitId)
                                   select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
                                   {
                                       PONo = b.PONo,
                                       BeginingbalanceQty = b.Quantity,
                                       BeginingbalancePrice = b.Quantity * b.BasicPrice,
                                       QuantityReceipt = 0,
                                       PriceReceipt = 0,
                                       QuantityUnitExpend = 0,
                                       PriceUnitExpend = 0,
                                       QuantityLocalExpend = 0,
                                       PriceLocalExpend = 0,
                                       QuantityOtherExpend = 0,
                                       PriceOtherExpend = 0,
                                       QuantitySampleExpend = 0,
                                       PriceSampleExpend = 0,
                                       UomUnit = b.UomUnit,
                                       UnitName = b.UnitName,
                                       ProductCode = b.ProductCode,
                                       ProductName = b.ProductName,
                                       EndbalanceQty = 0
                                   };
                var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptAccessories
                                              where data._IsDeleted == false
                                         && data.StorageReceiveDate.AddHours(offset).Date <= DateTo.Date
                                         && data.RequestUnitId ==  (UnitId == 0 ? data.RequestUnitId : UnitId)
                                              select new { data.RequestUnitName, data.StorageReceiveDate, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems on a.Id equals b.GarmentLeftOverWarehouseReceiptAccessoriesId
                                   select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
                                   {
                                       PONo = b.POSerialNumber,
                                       BeginingbalanceQty = a.StorageReceiveDate.AddHours(offset) < DateFrom.Date ? b.Quantity : 0,
                                       BeginingbalancePrice =(a.StorageReceiveDate.AddHours(offset) < DateFrom.Date ? b.Quantity : 0)* b.BasicPrice,
                                       QuantityReceipt = a.StorageReceiveDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0,
                                       PriceReceipt =(a.StorageReceiveDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0) * b.BasicPrice,
                                       QuantityUnitExpend = 0,
                                       PriceUnitExpend = 0,
                                       QuantityLocalExpend = 0,
                                       PriceLocalExpend = 0,
                                       QuantityOtherExpend = 0,
                                       PriceOtherExpend = 0,
                                       QuantitySampleExpend = 0,
                                       PriceSampleExpend = 0,
                                       UomUnit = b.UomUnit,
                                       UnitName = a.RequestUnitName,
                                       ProductCode = b.ProductCode,
                                       ProductName = b.ProductName,
                                       EndbalanceQty = 0
                                   };
                var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureAccessories
                                                  where data._IsDeleted == false
                                             && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                                              
                                                  select new { data.ExpenditureDate, data.Id ,data.ExpenditureDestination})
                                       join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureAccessoriesItems
                                                  where expend.UnitId == (UnitId == 0 ? expend.UnitId : UnitId)
                                                  select new { expend.BasicPrice,expend.ExpenditureId, expend.UomUnit, expend.UnitName, expend.Quantity, expend.PONo }
                                                  ) on a.Id equals b.ExpenditureId
                                       select new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
                                       {
                                           PONo = b.PONo,
                                           BeginingbalanceQty = a.ExpenditureDate.AddHours(offset) < DateFrom.Date ? -b.Quantity : 0,
                                           BeginingbalancePrice =(a.ExpenditureDate.AddHours(offset) < DateFrom.Date ? -b.Quantity : 0) * b.BasicPrice,
                                           QuantityReceipt = 0,
                                           PriceReceipt =0,
                                           QuantityUnitExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "UNIT" ? b.Quantity : 0,
                                           PriceUnitExpend=(a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "UNIT" ? b.Quantity : 0) * b.BasicPrice,
                                           QuantityLocalExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "JUAL LOKAL" ? b.Quantity : 0,
                                           PriceLocalExpend =(a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "JUAL LOKAL" ? b.Quantity : 0) * b.BasicPrice,
                                           QuantityOtherExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "LAIN-LAIN" ? b.Quantity : 0,
                                           PriceOtherExpend =(a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "LAIN-LAIN" ? b.Quantity : 0) * b.BasicPrice,
                                           QuantitySampleExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "SAMPLE" ? b.Quantity : 0,
                                           PriceSampleExpend =(a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date && a.ExpenditureDestination == "SAMPLE" ? b.Quantity : 0) * b.BasicPrice,
                                           UomUnit = b.UomUnit,
                                           UnitName = b.UnitName,
                                           ProductCode = (from aa in _product where aa.PONo == b.PONo select aa.ProductCode).FirstOrDefault(),
                                           ProductName = (from aa in _product where aa.PONo == b.PONo select aa.ProductName).FirstOrDefault(),
                                           EndbalanceQty = 0
                                       };
                var Query = QueryReceipt.Union(QueryExpenditure).Union(QueryBalance);
                var querySum = Query.ToList()
                    .GroupBy(x => new { x.PONo, x.UnitName, x.UomUnit,x.ProductCode,x.ProductName }, (key, group) => new
                    {
                        pono = key.PONo,
                        begining = group.Sum(s => s.BeginingbalanceQty),
                        beginingPrice = group.Sum(s => s.BeginingbalancePrice),
                        expendunit = group.Sum(s => s.QuantityUnitExpend),
                        expendunitPrice = group.Sum(s => s.PriceUnitExpend),
                        expendsample = group.Sum(s => s.QuantitySampleExpend),
                        expendsamplePrice = group.Sum(s => s.PriceSampleExpend),
                        expendother = group.Sum(s => s.QuantityOtherExpend),
                        expendotherPrice = group.Sum(s => s.PriceOtherExpend),
                        expendlocal = group.Sum(s => s.QuantityLocalExpend),
                        expendlocalPrice = group.Sum(s => s.PriceLocalExpend),
                        receipt = group.Sum(s => s.QuantityReceipt),
                        priceReceipt = group.Sum(s => s.PriceReceipt),
                        basicprice = group.Sum(s => s.BasicPrice),
                        uomunit = key.UomUnit,
                        unit = key.UnitName,
                        productCode= key.ProductCode,
                        productName= key.ProductName
                    }).OrderBy(s => s.pono);
                 
                foreach (var data in querySum)
                {
                     
                    GarmentLeftoverWarehouseFlowStockMonitoringViewModel garmentLeftover = new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
                    {
                        PONo = data.pono,
                        BeginingbalanceQty = data.begining,
                        BeginingbalancePrice = data.beginingPrice,
                        QuantityReceipt = data.receipt,
                        PriceReceipt = data.priceReceipt,
                        BasicPrice = data.basicprice,
                        QuantityUnitExpend = data.expendunit,
                        PriceUnitExpend = data.expendunitPrice,
                        QuantityLocalExpend = data.expendlocal,
                        PriceLocalExpend = data.expendlocalPrice,
                        QuantityOtherExpend = data.expendother,
                        PriceOtherExpend = data.expendotherPrice,
                        QuantitySampleExpend = data.expendsample,
                        PriceSampleExpend = data.expendsamplePrice,
                        UnitName = data.unit,
                        UomUnit = data.uomunit,
                        ProductCode = data.productCode,
                        ProductName = data.productName,
                        EndbalanceQty = data.begining + data.receipt - data.expendunit-data.expendsample-data.expendother-data.expendlocal,
                        EndbalancePrice = data.beginingPrice + data.priceReceipt - data.expendunitPrice - data.expendsamplePrice - data.expendotherPrice - data.expendlocalPrice
                    };
                    GarmentLeftoverWarehouseFlowStockMonitoringViewModel.Add(garmentLeftover);
                }
            }

            var queryGroup = GarmentLeftoverWarehouseFlowStockMonitoringViewModel.GroupBy(x => new
            {
                x.ProductCode,
                x.ProductName,
                x.UomUnit,
                x.UnitName
            }, (key, group) => new
            {
                productcode = key.ProductCode,
                productname = key.ProductName,
                uomunit = key.UomUnit,
                unitname = key.UnitName,
                begining = group.Sum(s => s.BeginingbalanceQty),
                beginingPrice = group.Sum(s => s.BeginingbalancePrice),
                expendunit = group.Sum(s => s.QuantityUnitExpend),
                expendunitPrice = group.Sum(s => s.PriceUnitExpend),
                expendsample = group.Sum(s => s.QuantitySampleExpend),
                expendsamplePrice = group.Sum(s => s.PriceSampleExpend),
                expendother = group.Sum(s => s.QuantityOtherExpend),
                expendotherPrice = group.Sum(s => s.PriceOtherExpend),
                expendlocal = group.Sum(s => s.QuantityLocalExpend),
                expendlocalPrice = group.Sum(s => s.PriceLocalExpend),
                receipt = group.Sum(s => s.QuantityReceipt),
                priceReceipt = group.Sum(s => s.PriceReceipt),
                basicprice = group.Sum(s => s.BasicPrice),
            });
            var queryGroupTotal = GarmentLeftoverWarehouseFlowStockMonitoringViewModel.GroupBy(x => new
            {
                x.UnitName
            }, (key, group) => new
            {
                productcode = "",
                productname = "",
                uomunit = "",
                unitname =  key.UnitName +" - TOTAL ",
                begining = group.Sum(s => s.BeginingbalanceQty),
                beginingPrice = group.Sum(s => s.BeginingbalancePrice),
                expendunit = group.Sum(s => s.QuantityUnitExpend),
                expendunitPrice = group.Sum(s => s.PriceUnitExpend),
                expendsample = group.Sum(s => s.QuantitySampleExpend),
                expendsamplePrice = group.Sum(s => s.PriceSampleExpend),
                expendother = group.Sum(s => s.QuantityOtherExpend),
                expendotherPrice = group.Sum(s => s.PriceOtherExpend),
                expendlocal = group.Sum(s => s.QuantityLocalExpend),
                expendlocalPrice = group.Sum(s => s.PriceLocalExpend),
                receipt = group.Sum(s => s.QuantityReceipt),
                priceReceipt = group.Sum(s => s.PriceReceipt),
                basicprice = group.Sum(s => s.BasicPrice)
            });
            var query = queryGroup.Union(queryGroupTotal).OrderBy(s => s.unitname);
            List<GarmentLeftoverWarehouseFlowStockMonitoringViewModel> stockMonitoringViewModels = new List<GarmentLeftoverWarehouseFlowStockMonitoringViewModel>();
            var queryGrandTotal = queryGroupTotal.GroupBy(x => new
            {
                x.productcode
            }, (key, group) => new
            {
                productcode = "",
                productname = "",
                uomunit = "",
                unitname = "GRAND TOTAL ",
                begining = group.Sum(s => s.begining),
                beginingPrice = group.Sum(s => s.beginingPrice),
                expendunit = group.Sum(s => s.expendunit),
                expendunitPrice = group.Sum(s => s.expendunitPrice),
                expendsample = group.Sum(s => s.expendsample),
                expendsamplePrice = group.Sum(s => s.expendsamplePrice),
                expendother = group.Sum(s => s.expendother),
                expendotherPrice = group.Sum(s => s.expendotherPrice),
                expendlocal = group.Sum(s => s.expendlocal),
                expendlocalPrice = group.Sum(s => s.expendlocalPrice),
                receipt = group.Sum(s => s.receipt),
                priceReceipt = group.Sum(s => s.priceReceipt),
                basicprice = group.Sum(s => s.basicprice)
            }
                );
            foreach (var data in query)
            {

                GarmentLeftoverWarehouseFlowStockMonitoringViewModel garmentLeftover = new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
                {
                    
                    BeginingbalanceQty = data.begining,
                    BeginingbalancePrice = data.beginingPrice,
                    QuantityReceipt = data.receipt,
                    PriceReceipt = data.priceReceipt,
                    BasicPrice = data.basicprice,
                    QuantityUnitExpend = data.expendunit,
                    PriceUnitExpend = data.expendunitPrice,
                    QuantityLocalExpend = data.expendlocal,
                    PriceLocalExpend = data.expendlocalPrice,
                    QuantityOtherExpend = data.expendother,
                    PriceOtherExpend = data.expendotherPrice,
                    QuantitySampleExpend = data.expendsample,
                    PriceSampleExpend = data.expendsamplePrice,
                    UnitName = data.unitname,
                    UomUnit = data.uomunit,
                    ProductCode =data.productcode,
                    ProductName =data.productname,
                    EndbalanceQty = data.begining + data.receipt - data.expendunit - data.expendsample - data.expendother - data.expendlocal,
                    EndbalancePrice = data.beginingPrice + data.priceReceipt - data.expendunitPrice - data.expendsamplePrice - data.expendotherPrice - data.expendlocalPrice
                };
                stockMonitoringViewModels.Add(garmentLeftover);
            }
            foreach (var data in queryGrandTotal)
            {

                GarmentLeftoverWarehouseFlowStockMonitoringViewModel garmentLeftover = new GarmentLeftoverWarehouseFlowStockMonitoringViewModel
                {

                    BeginingbalanceQty = data.begining,
                    BeginingbalancePrice = data.beginingPrice,
                    QuantityReceipt = data.receipt,
                    PriceReceipt = data.priceReceipt,
                    BasicPrice = data.basicprice,
                    QuantityUnitExpend = data.expendunit,
                    PriceUnitExpend = data.expendunitPrice,
                    QuantityLocalExpend = data.expendlocal,
                    PriceLocalExpend = data.expendlocalPrice,
                    QuantityOtherExpend = data.expendother,
                    PriceOtherExpend = data.expendotherPrice,
                    QuantitySampleExpend = data.expendsample,
                    PriceSampleExpend = data.expendsamplePrice,
                    UnitName = data.unitname,
                    UomUnit = data.uomunit,
                    ProductCode = data.productcode,
                    ProductName = data.productname,
                    EndbalanceQty = data.begining + data.receipt - data.expendunit - data.expendsample - data.expendother - data.expendlocal,
                    EndbalancePrice = data.beginingPrice + data.priceReceipt - data.expendunitPrice - data.expendsamplePrice - data.expendotherPrice - data.expendlocalPrice
                };
                stockMonitoringViewModels.Add(garmentLeftover);
            }
            var result = stockMonitoringViewModels.OrderByDescending(b => b.PONo);
            return result.ToList();
        }

     
        public Tuple<List<GarmentLeftoverWarehouseFlowStockMonitoringViewModel>, int> GetMonitoringFlowStock(string category, DateTime? dateFrom, DateTime? dateTo, int unit, int page, int size, string order, int offset)
        {
            var Query = GetReportQuery(category,dateFrom, dateTo, unit, offset);

            /*Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.PONo);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];
                Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<GarmentLeftoverWarehouseFlowStockMonitoringViewModel> pageable = new Pageable<GarmentLeftoverWarehouseFlowStockMonitoringViewModel>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseFlowStockMonitoringViewModel> Data = pageable.Data.ToList<GarmentLeftoverWarehouseFlowStockMonitoringViewModel>();
            */

            //int TotalData = pageable.TotalCount;
            int TotalData = Query.Count();
            return Tuple.Create(Query, TotalData);
        }

        public MemoryStream GenerateExcelFlowStock(string category, DateTime? dateFrom, DateTime? dateTo, int unit, int offset)
        {
            var Query = GetReportQuery(category, dateFrom, dateTo, unit, offset);
            //Query = Query.OrderByDescending(b => b.PONo);



            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "ASAL", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NAMA BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KODE BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "SATUAN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "SALDO AWAL", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "SALDO AWAL1", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TERIMA", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TERIMA1", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "U/ UNIT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "U/ UNIT1", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "U/ SAMPLE", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "U/ SAMPLE1", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "U/ JUAL LOKAL", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "U/ JUAL LOKAL1", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "U/ LAIN-LAIN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "U/ LAIN-LAIN1", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "SALDO AKHIR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "SALDO AKHIR1", DataType = typeof(String) });
            int counter = 0;
            result.Rows.Add("", "", "", "",
                    "Qty", "Harga", "Qty", "Harga", "Qty", "Harga", "Qty", "Harga", "Qty", "Harga", "Qty", "Harga", "Qty", "Harga");
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {

                foreach (var item in Query)
                {
                    counter++;
                    //DateTimeOffset date = item.date ?? new DateTime(1970, 1, 1);
                    //string dateString = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(item.UnitName, item.ProductName, item.ProductCode, item.UomUnit, Math.Round(item.BeginingbalanceQty,2),Math.Round(item.BeginingbalancePrice,2),Math.Round( item.QuantityReceipt,2), Math.Round( item.PriceReceipt,2),Math.Round( item.QuantityUnitExpend,2),Math.Round( item.PriceUnitExpend,2),Math.Round( item.QuantitySampleExpend,2),Math.Round( item.PriceSampleExpend,2),Math.Round( item.QuantityLocalExpend,2),Math.Round( item.PriceLocalExpend,2),Math.Round( item.QuantityOtherExpend,2),Math.Round( item.PriceOtherExpend,2),Math.Round( item.EndbalanceQty,2),Math.Round( item.EndbalancePrice,2));
                }

            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");
                worksheet.Cells["A1"].LoadFromDataTable(result, true);
                worksheet.Cells["E" + 1 + ":F" + 1 + ""].Merge = true;
                worksheet.Cells["G" + 1 + ":H" + 1 + ""].Merge = true;
                worksheet.Cells["I" + 1 + ":J" + 1 + ""].Merge = true;
                worksheet.Cells["K" + 1 + ":L" + 1 + ""].Merge = true;
                worksheet.Cells["M" + 1 + ":N" + 1 + ""].Merge = true;
                worksheet.Cells["O" + 1 + ":P" + 1 + ""].Merge = true;
                worksheet.Cells["Q" + 1 + ":R" + 1 + ""].Merge = true;
                worksheet.Cells["A" + 1 + ":R" + 2 + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A" + 1 + ":R" + 1 + ""].Style.Font.Bold = true;
                worksheet.Cells["A" + 1 + ":R" + (counter + 2) + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A" + 1 + ":R" + (counter + 2) + ""].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A" + 1 + ":R" + (counter + 2) + ""].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A" + 1 + ":R" + (counter + 2) + ""].Style.Border.Right.Style = ExcelBorderStyle.Thin;
             
                worksheet.Cells["A" + 1 + ":R" + (counter + 2) + ""].Style.Numberformat.Format = "#,##0.00";

                for (int i = 1; i < counter + 3; i++)
                {
                    if (worksheet.Cells["A" + i].Value != null)
                    {
                        string _val = worksheet.Cells["A" + i].Value.ToString();

                        if (_val.Contains("TOTAL"))
                        {

                            worksheet.Cells["A" + i + ":D" + i + ""].Merge = true;
                            worksheet.Cells["A" + i + ":R" + i + ""].Style.Font.Bold = true;
                        }
                        if (_val.Contains("GRAND TOTAL"))
                        {
                            worksheet.Cells["A" + i + ":R" + i + ""].Style.Fill.PatternType = ExcelFillStyle.Solid;

                            worksheet.Cells["A" + i + ":R" + i + ""].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }
                    }
                }

                worksheet.Cells["E" + 3 + ":R" + (counter + 2) + ""].Style.HorizontalAlignment= ExcelHorizontalAlignment.Right;
                foreach (var cell in worksheet.Cells["E" + 3 + ":R" + (counter + 2) + ""])
                {
                    cell.Value = Convert.ToDecimal(cell.Value);
                }
                worksheet.Cells["A" + 1 + ":R" + (counter + 2) + ""].AutoFitColumns();
                var stream = new MemoryStream();

                package.SaveAs(stream);

                return stream;
            }

        }
    }
}
#endregion
