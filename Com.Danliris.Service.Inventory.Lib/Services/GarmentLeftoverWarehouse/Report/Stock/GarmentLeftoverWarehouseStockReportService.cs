using Com.Danliris.Service.Inventory.Lib.Enums;
using Com.Danliris.Service.Inventory.Lib.Helpers;
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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Stock
{
    public class GarmentLeftoverWarehouseStockReportService : IGarmentLeftoverWarehouseStockReportService
    {
        private const string UserAgent = "GarmentLeftoverWarehouseStockReportService";

        private InventoryDbContext DbContext;

        private readonly IServiceProvider ServiceProvider;
        private readonly IIdentityService IdentityService;

        private readonly string GarmentExpenditureGoodUri;

        public GarmentLeftoverWarehouseStockReportService(InventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            IdentityService = (IIdentityService)serviceProvider.GetService(typeof(IIdentityService));

            //GarmentExpenditureGoodUri = APIEndpoint.GarmentProduction + "expenditure-goods/byRO";
            GarmentExpenditureGoodUri = APIEndpoint.GarmentProduction + "expenditure-goods/traceable-by-ro";
        }
        #region REPORT
        public IQueryable<GarmentLeftoverWarehouseStockMonitoringViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, int UnitId, string type, int offset, string typeAval = "")
        {

            DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
            DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;


            List<GarmentLeftoverWarehouseStockMonitoringViewModel> garmentLeftoverWarehouseStockMonitoringViewModel = new List<GarmentLeftoverWarehouseStockMonitoringViewModel>();

            if (type == "FABRIC")
            {
                var QueryBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                              where data._IsDeleted == false && data.TypeOfGoods.ToString() == "FABRIC"
                                              select new { data._CreatedUtc, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
                                   where b.UnitId == (UnitId == 0 ? b.UnitId : UnitId)
                                   select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                   {
                                       PONo = b.PONo,
                                       BeginingbalanceQty = b.Quantity,
                                       QuantityReceipt = 0,
                                       QuantityExpend = 0,
                                       UomUnit = b.UomUnit,
                                       UnitCode = b.UnitCode,
                                       ProductCode = "",
                                       ProductRemark = "",
                                       ProductName = "",
                                       FabricRemark = "",
                                       EndbalanceQty = 0,
                                       index = 0
                                   };
                var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFabrics
                                              where data._IsDeleted == false
                                         && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                                         && data.UnitFromId == (UnitId == 0 ? data.UnitFromId : UnitId)
                                              select new { data.UnitFromCode, data.ReceiptDate, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseReceiptFabricItems on a.Id equals b.GarmentLeftoverWarehouseReceiptFabricId
                                   select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                   {
                                       PONo = b.POSerialNumber,
                                       BeginingbalanceQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date ? b.Quantity : 0,
                                       QuantityReceipt = a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date ? b.Quantity : 0,
                                       QuantityExpend = 0,
                                       UomUnit = b.UomUnit,
                                       UnitCode = a.UnitFromCode,
                                       ProductCode = "",
                                       ProductRemark = "",
                                       ProductName = "",
                                       FabricRemark = "",
                                       EndbalanceQty = 0,
                                       index = 0
                                   };
                var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFabrics
                                                  where data._IsDeleted == false
                                             && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date

                                                  select new { data.UnitExpenditureCode, data.ExpenditureDate, data.Id })
                                       join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFabricItems
                                                  where expend.UnitId == (UnitId == 0 ? expend.UnitId : UnitId)
                                                  select new { expend.UomUnit, expend.PONo, expend.Quantity, expend.UnitCode, expend.ExpenditureId }) on a.Id equals b.ExpenditureId
                                       select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                       {
                                           PONo = b.PONo,
                                           BeginingbalanceQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date ? -b.Quantity : 0,
                                           QuantityReceipt = 0,
                                           QuantityExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date ? b.Quantity : 0,
                                           UomUnit = b.UomUnit,
                                           UnitCode = b.UnitCode,
                                           ProductCode = "",
                                           ProductRemark = "",
                                           ProductName = "",
                                           FabricRemark = "",
                                           EndbalanceQty = 0,
                                           index = 0
                                       };
                var Query = QueryReceipt.Union(QueryExpenditure).Union(QueryBalance);
                var querySum = Query.ToList()
                    .GroupBy(x => new { x.PONo, x.UnitCode, x.UomUnit, x.index }, (key, group) => new
                    {
                        pono = key.PONo,
                        begining = group.Sum(s => s.BeginingbalanceQty),
                        expend = group.Sum(s => s.QuantityExpend),
                        receipt = group.Sum(s => s.QuantityReceipt),
                        uomunit = key.UomUnit,
                        unit = key.UnitCode,
                        index = key.index
                    }).OrderBy(s => s.pono);

                var fabricType = GarmentLeftoverWarehouseStockReferenceTypeEnum.FABRIC;
                foreach (var data in querySum)
                {
                    var Product = (from aa in DbContext.GarmentLeftoverWarehouseStocks
                                   where aa.ReferenceType == fabricType && aa.PONo == data.pono && aa.UomUnit == data.uomunit && aa.UnitCode == data.unit
                                   select new
                                   {
                                       aa.ProductCode,
                                       aa.ProductName
                                   }).FirstOrDefault();
                    var remarkReceipt = (from aa in DbContext.GarmentLeftoverWarehouseReceiptFabricItems
                                  where aa.POSerialNumber == data.pono
                                  select new
                                  {
                                      ProductRemark= aa.Composition,
                                      aa.FabricRemark
                                  }).FirstOrDefault();
                    var remarkBalance = (from aa in DbContext.GarmentLeftoverWarehouseBalanceStocksItems
                                         where aa.PONo == data.pono
                                         select new
                                         {
                                             ProductRemark = aa.Composition,
                                             FabricRemark = aa.Construction + " ;" + aa.Yarn + " ;" + aa.Width
                                         }).FirstOrDefault();
                    GarmentLeftoverWarehouseStockMonitoringViewModel garmentLeftover = new GarmentLeftoverWarehouseStockMonitoringViewModel
                    {
                        PONo = data.pono,
                        BeginingbalanceQty = Math.Round(data.begining, 2, MidpointRounding.AwayFromZero),
                        QuantityReceipt = Math.Round(data.receipt, 2, MidpointRounding.AwayFromZero),
                        QuantityExpend = Math.Round(data.expend, 2, MidpointRounding.AwayFromZero),
                        UnitCode = data.unit,
                        UomUnit = data.uomunit,
                        ProductCode = Product.ProductCode,
                        ProductName = Product.ProductName,
                        ProductRemark = remarkReceipt !=null ? remarkReceipt.ProductRemark : remarkBalance != null ? remarkBalance.ProductRemark : "-",
                        FabricRemark = remarkReceipt != null ? remarkReceipt.FabricRemark : remarkBalance != null ? remarkBalance.FabricRemark : "-",
                        EndbalanceQty = Math.Round(data.begining + data.receipt - data.expend, 2, MidpointRounding.AwayFromZero)
                    };
                    garmentLeftoverWarehouseStockMonitoringViewModel.Add(garmentLeftover);
                }
            }
            else if (type == "Barang Jadi")
            {

                var QueryBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                              where data._IsDeleted == false && data.TypeOfGoods.ToString() == "BARANG JADI"
                                              select new { data._CreatedUtc, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
                                   where b.UnitId == (UnitId == 0 ? b.UnitId : UnitId)
                                   select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                   {
                                       RO = b.RONo,
                                       BeginingbalanceQty = b.Quantity,
                                       QuantityReceipt = 0,
                                       QuantityExpend = 0,
                                       UomUnit = b.UomUnit,
                                       UnitCode = b.UnitCode,
                                       ProductCode = "",
                                       ProductRemark = "",
                                       ProductName = "",
                                       FabricRemark = "",
                                       EndbalanceQty = 0,
                                       index = 0,
                                       Comodity= b.LeftoverComodityName,
                                       ComodityCode = b.LeftoverComodityCode
                                   };
                var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoods
                                              where data._IsDeleted == false
                                         && data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                                         && data.UnitFromId == (UnitId == 0 ? data.UnitFromId : UnitId)
                                              select new { data.UnitFromCode, data.ReceiptDate, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoodItems on a.Id equals b.FinishedGoodReceiptId
                                   select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                   {
                                       RO = b.RONo,
                                       BeginingbalanceQty = a.ReceiptDate.AddHours(offset) < DateFrom.Date ? b.Quantity : 0,
                                       QuantityReceipt = a.ReceiptDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0,
                                       QuantityExpend = 0,
                                       UomUnit = "PCS",
                                       UnitCode = a.UnitFromCode,
                                       ProductCode = "",
                                       ProductRemark = "",
                                       ProductName = "",
                                       FabricRemark = "",
                                       EndbalanceQty = 0,
                                       index = 0,
                                       Comodity= b.LeftoverComodityName,
                                       ComodityCode = b.LeftoverComodityCode
                                   };
                var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoods
                                                  where data._IsDeleted == false
                                                        && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                                                  select new { data.ExpenditureDate, data.Id })
                                       join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureFinishedGoodItems
                                                  where expend.UnitId == (UnitId == 0 ? expend.UnitId : UnitId)
                                                  select new { expend.FinishedGoodExpenditureId, expend.UnitCode, expend.ExpenditureQuantity, expend.RONo, expend.LeftoverComodityName }
                                                  ) on a.Id equals b.FinishedGoodExpenditureId
                                       select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                       {
                                           RO = b.RONo,
                                           BeginingbalanceQty = a.ExpenditureDate.AddHours(offset) < DateFrom.Date ? -b.ExpenditureQuantity : 0,
                                           QuantityReceipt = 0,
                                           QuantityExpend = a.ExpenditureDate.AddHours(offset) >= DateFrom.Date ? b.ExpenditureQuantity : 0,
                                           UomUnit = "PCS",
                                           UnitCode = b.UnitCode,
                                           ProductCode = "",
                                           ProductRemark = "",
                                           ProductName = "",
                                           FabricRemark = "",
                                           EndbalanceQty = 0,
                                           index = 0,
                                           Comodity= b.LeftoverComodityName,
                                           ComodityCode = b.LeftoverComodityName
                                       };
                var Query = QueryReceipt.Union(QueryExpenditure).Union(QueryBalance);
                var querySum = Query.ToList()
                    .GroupBy(x => new { x.RO, x.UnitCode, x.UomUnit, x.index, x.Comodity, x.ComodityCode }, (key, group) => new
                    {
                        rono = key.RO,
                        begining = group.Sum(s => s.BeginingbalanceQty),
                        expend = group.Sum(s => s.QuantityExpend),
                        receipt = group.Sum(s => s.QuantityReceipt),
                        uomunit = key.UomUnit,
                        unit = key.UnitCode,
                        index = key.index,
                        comodity= key.Comodity,
                        comoditycode = key.ComodityCode
                    }).OrderBy(s => s.rono);

                var ronos = string.Join(",", querySum.Select(x => x.rono).Distinct());

                var comodities1 = GetComodityFromProduction(ronos);

                var comodities2 = comodities1.Select(x => new { x.RONo, Comodity = x.Comodity.Code }).Distinct();

                garmentLeftoverWarehouseStockMonitoringViewModel = (from a in querySum
                                                                    join b in comodities2 on a.rono equals b.RONo into com2
                                                                    from bb in com2.DefaultIfEmpty()
                                                                    select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                                                    {
                                                                        RO = a.rono,
                                                                        BeginingbalanceQty = Math.Round(a.begining, 2, MidpointRounding.AwayFromZero),
                                                                        QuantityReceipt = Math.Round(a.receipt, 2, MidpointRounding.AwayFromZero),
                                                                        QuantityExpend = Math.Round(a.expend, 2, MidpointRounding.AwayFromZero),
                                                                        UnitCode = a.unit,
                                                                        UomUnit = a.uomunit,
                                                                        //ProductRemark = (from aa in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoodItems where aa.RONo == data.rono select aa.LeftoverComodityName).FirstOrDefault(),
                                                                        EndbalanceQty = Math.Round(a.begining + a.receipt - a.expend, 2, MidpointRounding.AwayFromZero),
                                                                        Comodity = a.comodity,
                                                                        ComodityCode = a.comoditycode,
                                                                        ComodityUnitCode = bb != null ? bb.Comodity : "-"
                                                                    }).ToList();

                //foreach (var data in querySum)
                //{
                //    var comodity = GetComodityFromProduction(data.rono).FirstOrDefault();

                                                                   //    GarmentLeftoverWarehouseStockMonitoringViewModel garmentLeftover = new GarmentLeftoverWarehouseStockMonitoringViewModel
                                                                   //    {
                                                                   //        RO = data.rono,
                                                                   //        BeginingbalanceQty = Math.Round(data.begining, 2, MidpointRounding.AwayFromZero),
                                                                   //        QuantityReceipt = Math.Round(data.receipt, 2, MidpointRounding.AwayFromZero),
                                                                   //        QuantityExpend = Math.Round(data.expend, 2, MidpointRounding.AwayFromZero),
                                                                   //        UnitCode = data.unit,
                                                                   //        UomUnit = data.uomunit,
                                                                   //        //ProductRemark = (from aa in DbContext.GarmentLeftoverWarehouseReceiptFinishedGoodItems where aa.RONo == data.rono select aa.LeftoverComodityName).FirstOrDefault(),
                                                                   //        EndbalanceQty = Math.Round(data.begining + data.receipt - data.expend, 2, MidpointRounding.AwayFromZero),
                                                                   //        Comodity=data.comodity,
                                                                   //        ComodityCode = data.comoditycode,
                                                                   //        ComodityUnitCode = comodity != null ? comodity.Comodity.Code : "-"
                                                                   //    };
                                                                   //    garmentLeftoverWarehouseStockMonitoringViewModel.Add(garmentLeftover);
                                                                   //}
            }
            else if (type == "AVAL")
            {
                var AvalType = typeAval == "AVAL FABRIC" ? GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_FABRIC : typeAval == "AVAL KOMPONEN" ? GarmentLeftoverWarehouseStockReferenceTypeEnum.COMPONENT : GarmentLeftoverWarehouseStockReferenceTypeEnum.AVAL_BAHAN_PENOLONG;
                var queryReceiptHeader = from data in DbContext.GarmentLeftoverWarehouseReceiptAvals
                                         where data.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                                        && data.UnitFromId == (UnitId == 0 ? data.UnitFromId : UnitId)
                                        && data.AvalType == typeAval
                                         select new { data.UnitFromCode, data.ReceiptDate, data.Id, data.AvalType, data.TotalAval };
                IQueryable<GarmentLeftoverWarehouseStockMonitoringViewModel> QueryReceipt = Enumerable.Empty<GarmentLeftoverWarehouseStockMonitoringViewModel>().AsQueryable();
                if (typeAval == "AVAL BAHAN PENOLONG")
                {
                    QueryReceipt = from a in (queryReceiptHeader)
                                   join b in DbContext.GarmentLeftoverWarehouseReceiptAvalItems on a.Id equals b.AvalReceiptId
                                   select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                   {
                                       BeginingbalanceQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date ? b.Quantity : 0,
                                       QuantityReceipt = a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date ? b.Quantity : 0,
                                       QuantityExpend = 0,
                                       UomUnit = b.UomUnit,
                                       UnitCode = a.UnitFromCode,
                                       index = 0,
                                       ProductCode = b.ProductCode,
                                       ReferenceType = a.AvalType
                                   };
                }
                else
                {
                    QueryReceipt = from a in (queryReceiptHeader)
                                   select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                   {
                                       BeginingbalanceQty = a.ReceiptDate.AddHours(offset).Date < DateFrom.Date ? a.TotalAval : 0,
                                       QuantityReceipt = a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date ? a.TotalAval : 0,
                                       QuantityExpend = 0,
                                       UomUnit = "KG",
                                       UnitCode = a.UnitFromCode,
                                       index = 0,
                                       ProductCode = null,
                                       ReferenceType = a.AvalType
                                   };
                }
                var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureAvals
                                                  where data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                                                    && data.AvalType == typeAval
                                                  select new { data.ExpenditureDate, data.Id, data.AvalType })
                                       join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureAvalItems
                                                  where expend.UnitId == (UnitId == 0 ? expend.UnitId : UnitId)
                                                  select new { expend.UomUnit, expend.Quantity, expend.UnitCode, expend.AvalExpenditureId, expend.ProductCode }) on a.Id equals b.AvalExpenditureId
                                       select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                       {
                                           BeginingbalanceQty = a.ExpenditureDate.AddHours(offset).Date < DateFrom.Date ? -b.Quantity : 0,
                                           QuantityReceipt = 0,
                                           QuantityExpend = a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date ? b.Quantity : 0,
                                           UomUnit = typeAval == "AVAL BAHAN PENOLONG" ? b.UomUnit : "KG",
                                           index = 0,
                                           UnitCode = b.UnitCode,
                                           ProductCode = typeAval == "AVAL BAHAN PENOLONG" ? b.ProductCode : null,
                                           ReferenceType = a.AvalType
                                       };
                var Query = QueryReceipt.Union(QueryExpenditure);
                var querySum = Query.ToList()
                    .GroupBy(x => new { x.UnitCode, x.UomUnit, x.index, x.ProductCode, x.ReferenceType }, (key, group) => new
                    {
                        begining = group.Sum(s => s.BeginingbalanceQty),
                        expend = group.Sum(s => s.QuantityExpend),
                        receipt = group.Sum(s => s.QuantityReceipt),
                        uomunit = key.UomUnit,
                        unit = key.UnitCode,
                        productCode = key.ProductCode,
                        index = key.index
                    });

                foreach (var data in querySum)
                {
                    GarmentLeftoverWarehouseStockMonitoringViewModel garmentLeftover = new GarmentLeftoverWarehouseStockMonitoringViewModel
                    {
                        BeginingbalanceQty = Math.Round(data.begining, MidpointRounding.AwayFromZero),
                        QuantityReceipt = Math.Round(data.receipt, MidpointRounding.AwayFromZero),
                        QuantityExpend = Math.Round(data.expend, MidpointRounding.AwayFromZero),
                        UnitCode = data.unit,
                        UomUnit = data.uomunit,
                        ProductCode = typeAval == "AVAL BAHAN PENOLONG" ? (from aa in DbContext.GarmentLeftoverWarehouseStocks where aa.ReferenceType == AvalType && aa.UnitId == (UnitId == 0 ? aa.UnitId : UnitId) && aa.UomUnit == data.uomunit && aa.ProductCode == data.productCode select aa.ProductCode).FirstOrDefault() : "-",
                        ProductName = typeAval == "AVAL BAHAN PENOLONG" ? (from aa in DbContext.GarmentLeftoverWarehouseStocks where aa.ReferenceType == AvalType && aa.UnitId == (UnitId == 0 ? aa.UnitId : UnitId) && aa.UomUnit == data.uomunit && aa.ProductCode == data.productCode select aa.ProductName).FirstOrDefault() : "-",
                        EndbalanceQty = Math.Round(data.begining + data.receipt - data.expend, MidpointRounding.AwayFromZero),
                        ReferenceType = typeAval
                    };
                    garmentLeftoverWarehouseStockMonitoringViewModel.Add(garmentLeftover);
                }
            }
            else
            {
                var QueryBalance = from a in (from data in DbContext.GarmentLeftoverWarehouseBalanceStocks
                                              where data._IsDeleted == false && data.TypeOfGoods.ToString() == "ACCESSORIES"
                                              select new { data._CreatedUtc, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseBalanceStocksItems on a.Id equals b.BalanceStockId
                                   where b.UnitId == UnitId
                                   select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                   {
                                       PONo = b.PONo,
                                       BeginingbalanceQty = b.Quantity,
                                       QuantityReceipt = 0,
                                       QuantityExpend = 0,
                                       UomUnit = b.UomUnit,
                                       UnitCode = b.UnitCode,
                                       ProductCode = "",
                                       ProductRemark = "",
                                       ProductName = "",
                                       FabricRemark = "",
                                       EndbalanceQty = 0,
                                       index = 0
                                   };
                var QueryReceipt = from a in (from data in DbContext.GarmentLeftoverWarehouseReceiptAccessories
                                              where data._IsDeleted == false
                                         && data.StorageReceiveDate.AddHours(offset).Date <= DateTo.Date
                                         && data.RequestUnitId == UnitId
                                              select new { data.RequestUnitCode, data.StorageReceiveDate, data.Id })
                                   join b in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems on a.Id equals b.GarmentLeftOverWarehouseReceiptAccessoriesId
                                   select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                   {
                                       PONo = b.POSerialNumber,
                                       BeginingbalanceQty = a.StorageReceiveDate.AddHours(offset) < DateFrom.Date ? b.Quantity : 0,
                                       QuantityReceipt = a.StorageReceiveDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0,
                                       QuantityExpend = 0,
                                       UomUnit = b.UomUnit,
                                       UnitCode = a.RequestUnitCode,
                                       ProductCode = "",
                                       ProductRemark = "",
                                       ProductName = "",
                                       FabricRemark = "",
                                       EndbalanceQty = 0,
                                       index = 0
                                   };
                var QueryExpenditure = from a in (from data in DbContext.GarmentLeftoverWarehouseExpenditureAccessories
                                                  where data._IsDeleted == false
                                             && data.ExpenditureDate.AddHours(offset).Date <= DateTo.Date

                                                  select new { data.ExpenditureDate, data.Id })
                                       join b in (from expend in DbContext.GarmentLeftoverWarehouseExpenditureAccessoriesItems
                                                  where expend.UnitId == UnitId
                                                  select new { expend.ExpenditureId, expend.UomUnit, expend.UnitCode, expend.Quantity, expend.PONo }
                                                  ) on a.Id equals b.ExpenditureId
                                       select new GarmentLeftoverWarehouseStockMonitoringViewModel
                                       {
                                           PONo = b.PONo,
                                           BeginingbalanceQty = a.ExpenditureDate.AddHours(offset) < DateFrom.Date ? -b.Quantity : 0,
                                           QuantityReceipt = 0,
                                           QuantityExpend = a.ExpenditureDate.AddHours(offset) >= DateFrom.Date ? b.Quantity : 0,
                                           UomUnit = b.UomUnit,
                                           UnitCode = b.UnitCode,
                                           ProductCode = "",
                                           ProductRemark = "",
                                           ProductName = "",
                                           FabricRemark = "",
                                           EndbalanceQty = 0,
                                           index = 0
                                       };
                var Query = QueryReceipt.Union(QueryExpenditure).Union(QueryBalance);
                var querySum = Query.ToList()
                    .GroupBy(x => new { x.PONo, x.UnitCode, x.UomUnit, x.index }, (key, group) => new
                    {
                        pono = key.PONo,
                        begining = group.Sum(s => s.BeginingbalanceQty),
                        expend = group.Sum(s => s.QuantityExpend),
                        receipt = group.Sum(s => s.QuantityReceipt),
                        uomunit = key.UomUnit,
                        unit = key.UnitCode,
                        index = key.index
                    }).OrderBy(s => s.pono);

                var accType = GarmentLeftoverWarehouseStockReferenceTypeEnum.ACCESSORIES;
                foreach (var data in querySum)
                {
                    var remarkReceipt = (from aa in DbContext.GarmentLeftoverWarehouseReceiptAccessoryItems
                                         where aa.POSerialNumber == data.pono
                                         select new
                                         {
                                             aa.ProductRemark
                                         }).FirstOrDefault();
                    var remarkBalance = (from aa in DbContext.GarmentLeftoverWarehouseBalanceStocksItems
                                         where aa.PONo == data.pono
                                         select new
                                         {
                                             aa.ProductRemark
                                         }).FirstOrDefault();
                    GarmentLeftoverWarehouseStockMonitoringViewModel garmentLeftover = new GarmentLeftoverWarehouseStockMonitoringViewModel
                    {
                        PONo = data.pono,
                        BeginingbalanceQty = Math.Round(data.begining, MidpointRounding.AwayFromZero),
                        QuantityReceipt = Math.Round(data.receipt, MidpointRounding.AwayFromZero),
                        QuantityExpend = Math.Round(data.expend, MidpointRounding.AwayFromZero),
                        UnitCode = data.unit,
                        UomUnit = data.uomunit,
                        ProductCode = (from aa in DbContext.GarmentLeftoverWarehouseStocks where aa.ReferenceType == accType && aa.PONo == data.pono && aa.UomUnit == data.uomunit && aa.UnitCode == data.unit select aa.ProductCode).FirstOrDefault(),
                        ProductName = (from aa in DbContext.GarmentLeftoverWarehouseStocks where aa.ReferenceType == accType && aa.PONo == data.pono && aa.UomUnit == data.uomunit && aa.UnitCode == data.unit select aa.ProductName).FirstOrDefault(),
                        ProductRemark = remarkReceipt==null ? remarkBalance.ProductRemark : remarkReceipt.ProductRemark,
                        EndbalanceQty = Math.Round(data.begining + data.receipt - data.expend, MidpointRounding.AwayFromZero)
                    };
                    garmentLeftoverWarehouseStockMonitoringViewModel.Add(garmentLeftover);
                }
            }

            return garmentLeftoverWarehouseStockMonitoringViewModel.AsQueryable();
        }

        public Tuple<List<GarmentLeftoverWarehouseStockMonitoringViewModel>, int> GetMonitoringFabric(DateTime? dateFrom, DateTime? dateTo, int unitId, int page, int size, string order, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, unitId, "FABRIC", offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
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

            Pageable<GarmentLeftoverWarehouseStockMonitoringViewModel> pageable = new Pageable<GarmentLeftoverWarehouseStockMonitoringViewModel>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseStockMonitoringViewModel> Data = pageable.Data.ToList<GarmentLeftoverWarehouseStockMonitoringViewModel>();

            int TotalData = pageable.TotalCount;
            int index = 0;
            Data.ForEach(c =>
            {
                index += 1;
                c.index = index;

            });

            if (page == Math.Ceiling((double)TotalData / (double)size) && TotalData != 0)
            {
                var BeginingbalanceQtyTotal = Math.Round(Query.Sum(x => x.BeginingbalanceQty), MidpointRounding.AwayFromZero);
                var QuantityReceiptTotal = Math.Round(Query.Sum(x => x.QuantityReceipt), MidpointRounding.AwayFromZero);
                var QuantityExpendTotal = Math.Round(Query.Sum(x => x.QuantityExpend), MidpointRounding.AwayFromZero);
                var EndbalanceQtyTotal = Math.Round(Query.Sum(x => x.EndbalanceQty), MidpointRounding.AwayFromZero);


                Data.Add(new GarmentLeftoverWarehouseStockMonitoringViewModel
                {
                    index = 0,
                    BeginingbalanceQty = BeginingbalanceQtyTotal,
                    EndbalanceQty = EndbalanceQtyTotal,
                    FabricRemark = "",
                    PONo = "TOTAL",
                    ProductCode = "",
                    ProductName = "",
                    ProductRemark = "",
                    QuantityExpend = QuantityExpendTotal,
                    QuantityReceipt = QuantityReceiptTotal,
                    ReferenceType = "",
                    RO = "",
                    UnitCode = "",
                    UomUnit = ""
                });
            }
            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcelFabric(DateTime? dateFrom, DateTime? dateTo, int unitId, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, unitId, "FABRIC", offset);
            Query = Query.OrderByDescending(b => b.PONo);

            var BeginingbalanceQtyTotal = Math.Round(Query.Sum(x => x.BeginingbalanceQty), MidpointRounding.AwayFromZero);
            var QuantityReceiptTotal = Math.Round(Query.Sum(x => x.QuantityReceipt), MidpointRounding.AwayFromZero);
            var QuantityExpendTotal = Math.Round(Query.Sum(x => x.QuantityExpend), MidpointRounding.AwayFromZero);
            var EndbalanceQtyTotal = Math.Round(Query.Sum(x => x.EndbalanceQty), MidpointRounding.AwayFromZero);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Konstruksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penerimaan", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pengeluaran", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Akhir", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", 0, 0, 0, 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    //DateTimeOffset date = item.date ?? new DateTime(1970, 1, 1);
                    //string dateString = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.UnitCode, item.PONo, item.ProductCode, item.ProductName, item.ProductRemark, item.FabricRemark, item.BeginingbalanceQty, item.QuantityReceipt, item.QuantityExpend, item.EndbalanceQty, item.UomUnit);
                }

                result.Rows.Add("", "", "T O T A L......", "", "", "", "", BeginingbalanceQtyTotal, QuantityReceiptTotal, QuantityExpendTotal, EndbalanceQtyTotal, "");
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Report Stock Gudang Sisa - FABRIC") }, true);

        }

        public MemoryStream GenerateExcelAcc(DateTime? dateFrom, DateTime? dateTo, int unitId, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, unitId, "ACC", offset);
            Query = Query.OrderByDescending(b => b.PONo);

            var BeginingbalanceQtyTotal = Math.Round(Query.Sum(x => x.BeginingbalanceQty), MidpointRounding.AwayFromZero);
            var QuantityReceiptTotal = Math.Round(Query.Sum(x => x.QuantityReceipt), MidpointRounding.AwayFromZero);
            var QuantityExpendTotal = Math.Round(Query.Sum(x => x.QuantityExpend), MidpointRounding.AwayFromZero);
            var EndbalanceQtyTotal = Math.Round(Query.Sum(x => x.EndbalanceQty), MidpointRounding.AwayFromZero);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penerimaan", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pengeluaran", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Akhir", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", 0, 0, 0, 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    //DateTimeOffset date = item.date ?? new DateTime(1970, 1, 1);
                    //string dateString = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.UnitCode, item.PONo, item.ProductCode, item.ProductName, item.ProductRemark, item.BeginingbalanceQty, item.QuantityReceipt, item.QuantityExpend, item.EndbalanceQty, item.UomUnit);
                }

                result.Rows.Add("", "", "T O T A L......", "", "", "", BeginingbalanceQtyTotal, QuantityReceiptTotal, QuantityExpendTotal, EndbalanceQtyTotal, "");
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Report Stock Gudang Sisa - ACC") }, true);

        }

        public Tuple<List<GarmentLeftoverWarehouseStockMonitoringViewModel>, int> GetMonitoringAcc(DateTime? dateFrom, DateTime? dateTo, int unitId, int page, int size, string order, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, unitId, "ACC", offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
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

            Pageable<GarmentLeftoverWarehouseStockMonitoringViewModel> pageable = new Pageable<GarmentLeftoverWarehouseStockMonitoringViewModel>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseStockMonitoringViewModel> Data = pageable.Data.ToList<GarmentLeftoverWarehouseStockMonitoringViewModel>();

            int TotalData = pageable.TotalCount;
            int index = 0;
            Data.ForEach(c =>
            {
                index += 1;
                c.index = index;

            });

            if (page == Math.Ceiling((double)TotalData / (double)size) && TotalData != 0)
            {
                var BeginingbalanceQtyTotal = Math.Round(Query.Sum(x => x.BeginingbalanceQty), MidpointRounding.AwayFromZero);
                var QuantityReceiptTotal = Math.Round(Query.Sum(x => x.QuantityReceipt), MidpointRounding.AwayFromZero);
                var QuantityExpendTotal = Math.Round(Query.Sum(x => x.QuantityExpend), MidpointRounding.AwayFromZero);
                var EndbalanceQtyTotal = Math.Round(Query.Sum(x => x.EndbalanceQty), MidpointRounding.AwayFromZero);


                Data.Add(new GarmentLeftoverWarehouseStockMonitoringViewModel
                {
                    index = 0,
                    BeginingbalanceQty = BeginingbalanceQtyTotal,
                    EndbalanceQty = EndbalanceQtyTotal,
                    FabricRemark = "",
                    PONo = "TOTAL",
                    ProductCode = "",
                    ProductName = "",
                    ProductRemark = "",
                    QuantityExpend = QuantityExpendTotal,
                    QuantityReceipt = QuantityReceiptTotal,
                    ReferenceType = "",
                    RO = "",
                    UnitCode = "",
                    UomUnit = ""
                });
            }

            return Tuple.Create(Data, TotalData);
        }




        public MemoryStream GenerateExcelFinishedGood(DateTime? dateFrom, DateTime? dateTo, int unitId, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, unitId, "Barang Jadi", offset);
            Query = Query.OrderByDescending(b => b.PONo);

            var BeginingbalanceQtyTotal = Math.Round(Query.Sum(x => x.BeginingbalanceQty), MidpointRounding.AwayFromZero);
            var QuantityReceiptTotal = Math.Round(Query.Sum(x => x.QuantityReceipt), MidpointRounding.AwayFromZero);
            var QuantityExpendTotal = Math.Round(Query.Sum(x => x.QuantityExpend), MidpointRounding.AwayFromZero);
            var EndbalanceQtyTotal = Math.Round(Query.Sum(x => x.EndbalanceQty), MidpointRounding.AwayFromZero);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Komoditi Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Komoditi Gudang Sisa", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Komoditi Gudang Sisa", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penerimaan", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pengeluaran", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Akhir", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "","", "", 0, 0, 0, 0, 0,""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    //DateTimeOffset date = item.date ?? new DateTime(1970, 1, 1);
                    //string dateString = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.UnitCode, item.RO, item.ComodityUnitCode, item.ComodityCode, item.Comodity, item.BeginingbalanceQty, item.QuantityReceipt, item.QuantityExpend, item.EndbalanceQty, item.UomUnit);
                }

                result.Rows.Add("", "", "T O T A L......", "", "", "", BeginingbalanceQtyTotal, QuantityReceiptTotal, QuantityExpendTotal, EndbalanceQtyTotal, "");
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Report Stock Gudang Sisa - Barang Jadi") }, true);

        }

        public Tuple<List<GarmentLeftoverWarehouseStockMonitoringViewModel>, int> GetMonitoringFinishedGood(DateTime? dateFrom, DateTime? dateTo, int unitId, int page, int size, string order, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, unitId, "Barang Jadi", offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
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

            Pageable<GarmentLeftoverWarehouseStockMonitoringViewModel> pageable = new Pageable<GarmentLeftoverWarehouseStockMonitoringViewModel>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseStockMonitoringViewModel> Data = pageable.Data.ToList<GarmentLeftoverWarehouseStockMonitoringViewModel>();

            int totalCountReport = Query.Count();
            int TotalData = pageable.TotalCount;
            int index = 0;
            Data.ForEach(c =>
            {
                index += 1;
                c.index = index;

            });

            if (page == Math.Ceiling((double)totalCountReport / (double)size) && TotalData != 0)
            {
                var BeginingbalanceQtyTotal = Math.Round(Query.Sum(x => x.BeginingbalanceQty), MidpointRounding.AwayFromZero);
                var QuantityReceiptTotal = Math.Round(Query.Sum(x => x.QuantityReceipt), MidpointRounding.AwayFromZero);
                var QuantityExpendTotal = Math.Round(Query.Sum(x => x.QuantityExpend), MidpointRounding.AwayFromZero);
                var EndbalanceQtyTotal = Math.Round(Query.Sum(x => x.EndbalanceQty), MidpointRounding.AwayFromZero);


                Data.Add(new GarmentLeftoverWarehouseStockMonitoringViewModel
                {
                    index = 0,
                    BeginingbalanceQty = BeginingbalanceQtyTotal,
                    EndbalanceQty = EndbalanceQtyTotal,
                    FabricRemark = "",
                    PONo = "",
                    ProductCode = "",
                    ProductName = "",
                    ProductRemark = "",
                    ComodityUnitCode = "",
                    QuantityExpend = QuantityExpendTotal,
                    QuantityReceipt = QuantityReceiptTotal,
                    ReferenceType = "",
                    RO = "TOTAL",
                    UnitCode = "",
                    UomUnit = ""
                });
            }

            return Tuple.Create(Data, TotalData);
        }

        public Tuple<List<GarmentLeftoverWarehouseStockMonitoringViewModel>, int> GetMonitoringAval(DateTime? dateFrom, DateTime? dateTo, int unitId, int page, int size, string order, int offset, string typeAval)
        {
            var Query = GetReportQuery(dateFrom, dateTo, unitId, "AVAL", offset, typeAval);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
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

            Pageable<GarmentLeftoverWarehouseStockMonitoringViewModel> pageable = new Pageable<GarmentLeftoverWarehouseStockMonitoringViewModel>(Query, page - 1, size);
            List<GarmentLeftoverWarehouseStockMonitoringViewModel> Data = pageable.Data.ToList<GarmentLeftoverWarehouseStockMonitoringViewModel>();

            int TotalData = pageable.TotalCount;
            int index = 0;
            Data.ForEach(c =>
            {
                index += 1;
                c.index = index;

            });

            if (page == Math.Ceiling((double)TotalData / (double)size) && TotalData != 0)
            {
                var BeginingbalanceQtyTotal = Math.Round(Query.Sum(x => x.BeginingbalanceQty), MidpointRounding.AwayFromZero);
                var QuantityReceiptTotal = Math.Round(Query.Sum(x => x.QuantityReceipt), MidpointRounding.AwayFromZero);
                var QuantityExpendTotal = Math.Round(Query.Sum(x => x.QuantityExpend), MidpointRounding.AwayFromZero);
                var EndbalanceQtyTotal = Math.Round(Query.Sum(x => x.EndbalanceQty), MidpointRounding.AwayFromZero);


                Data.Add(new GarmentLeftoverWarehouseStockMonitoringViewModel
                {
                    index = 0,
                    BeginingbalanceQty = BeginingbalanceQtyTotal,
                    EndbalanceQty = EndbalanceQtyTotal,
                    FabricRemark = "",
                    PONo = "",
                    ProductCode = "",
                    ProductName = "",
                    ProductRemark = "",
                    QuantityExpend = QuantityExpendTotal,
                    QuantityReceipt = QuantityReceiptTotal,
                    ReferenceType = "TOTAL",
                    RO = "",
                    UnitCode = "",
                    UomUnit = "",
                });
            }

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcelAval(DateTime? dateFrom, DateTime? dateTo, int unitId, int offset, string typeAval)
        {
            var Query = GetReportQuery(dateFrom, dateTo, unitId, "AVAL", offset, typeAval);
            Query = Query.OrderByDescending(b => b.PONo);

            var BeginingbalanceQtyTotal = Math.Round(Query.Sum(x => x.BeginingbalanceQty), MidpointRounding.AwayFromZero);
            var QuantityReceiptTotal = Math.Round(Query.Sum(x => x.QuantityReceipt), MidpointRounding.AwayFromZero);
            var QuantityExpendTotal = Math.Round(Query.Sum(x => x.QuantityExpend), MidpointRounding.AwayFromZero);
            var EndbalanceQtyTotal = Math.Round(Query.Sum(x => x.EndbalanceQty), MidpointRounding.AwayFromZero);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Aval", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penerimaan", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pengeluaran", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Akhir", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", 0, 0, 0, 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    //DateTimeOffset date = item.date ?? new DateTime(1970, 1, 1);
                    //string dateString = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.UnitCode, typeAval, item.ProductCode, item.ProductName, item.BeginingbalanceQty, item.QuantityReceipt, item.QuantityExpend, item.EndbalanceQty, item.UomUnit);
                }

                result.Rows.Add("", "", "T O T A L......", "", "", BeginingbalanceQtyTotal, QuantityReceiptTotal, QuantityExpendTotal, EndbalanceQtyTotal, "");
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Report Stock Gudang Sisa - FABRIC") }, true);

        }

        private List<ExpendGoodViewModel> GetComodityFromProduction(string RONo)
        {
            var httpService = (IHttpService)ServiceProvider.GetService(typeof(IHttpService));

            var httpContent = new StringContent(JsonConvert.SerializeObject(RONo), Encoding.UTF8, "application/json");

            var responseLocalCoverLetter = httpService.SendAsync(HttpMethod.Get, GarmentExpenditureGoodUri, httpContent).Result;
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

        #endregion
    }
}
