using Com.Danliris.Service.Inventory.Lib.Interfaces;
using Com.Danliris.Service.Inventory.Lib.Configs.InventoriesConfig;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Com.Danliris.Service.Inventory.Lib.Facades.InventoryFacades
{
    public class InventorySummaryReportFacade
    {
        private readonly InventoryDbContext DbContext;
        private readonly DbSet<InventorySummary> DbSet;
        private IdentityService IdentityService;

        public InventorySummaryReportFacade(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<InventorySummary>();
            this.IdentityService = serviceProvider.GetService<IdentityService>();
        }

        public IQueryable<InventorySummaryViewModel> GetReportQuery(string storageCode, string productCode, int offset)
        {
            var Query = (from a in DbContext.InventorySummaries
                             //Conditions
                         where a._IsDeleted == false
                             && a.StorageCode == (string.IsNullOrWhiteSpace(storageCode) ? a.StorageCode : storageCode)
                             && a.ProductCode == (string.IsNullOrWhiteSpace(productCode) ? a.ProductCode : productCode)
                         select new InventorySummaryViewModel
                         {
                             code = a.No,
                             productId = a.ProductId,
                             productCode = a.ProductCode,
                             productName = a.ProductName,
                             uomId = a.UomId,
                             uom = a.UomUnit,
                             storageId = a.StorageId,
                             storageCode = a.StorageCode,
                             storageName = a.StorageName,
                             quantity = a.Quantity,
                             
                             _LastModifiedUtc = a._LastModifiedUtc
                         });
            return Query;
        }
        public Tuple<List<InventorySummaryViewModel>, int> GetReport(string storageCode, string productCode, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(storageCode, productCode, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b._LastModifiedUtc);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];
                Query = Query.OrderBy(string.Concat(Key," ",OrderType));
            }


            Pageable<InventorySummaryViewModel> pageable = new Pageable<InventorySummaryViewModel>(Query, page - 1, size);
            List<InventorySummaryViewModel> Data = pageable.Data.ToList<InventorySummaryViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }
        public MemoryStream GenerateExcel(string storageCode, string productCode, int offset)
        {
            var Query = GetReportQuery(storageCode, productCode, offset);
            Query = Query.OrderByDescending(b => b._LastModifiedUtc);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Storage", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kuantiti", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "UOM", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", 0, ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    result.Rows.Add(index, item.storageName, item.productName, item.quantity, item.uom);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        public List<InventorySummaryViewModel> GetInventorySummaries(string productIds = "{}")
        {
            Dictionary<string, object> productIdDictionaries = JsonConvert.DeserializeObject<Dictionary<string, object>>(productIds);
            if (productIdDictionaries.Count == 0)
                return new List<InventorySummaryViewModel>();
            var productIdString = productIdDictionaries.Values.FirstOrDefault();
            var productIdList = JsonConvert.DeserializeObject<List<int>>(productIdString.ToString());

            IQueryable<InventorySummary> rootQuery = DbContext.InventorySummaries.Where(x => !x._IsDeleted);
            List<InventorySummary> documentItems = new List<InventorySummary>();
            foreach (var id in productIdList)
            {
                var result = rootQuery.Where(x => x.ProductId == id);
                documentItems.AddRange(result.ToList());
            }
            return documentItems.Select(x => new InventorySummaryViewModel()
            {
                productCode = x.ProductCode,
                productId = x.ProductId,
                productName = x.ProductName,
                quantity = x.Quantity,
                storageCode = x.StorageCode,
                Active = x.Active,
                Id  = x.Id,
                stockPlanning = x.StockPlanning.ToString(),
                storageId = x.StorageId,
                storageName = x.StorageName,
                code = x.No,
                uomId = x.UomId,
                uom = x.UomUnit,
            }).ToList();
        }

        public InventorySummary GetSummaryByParams(int storageId, int productId, int uomId)
        {
            return DbSet.FirstOrDefault(f => f.StorageId.Equals(storageId) && f.ProductId.Equals(productId) && f.UomId.Equals(uomId));
        }
    }
}
