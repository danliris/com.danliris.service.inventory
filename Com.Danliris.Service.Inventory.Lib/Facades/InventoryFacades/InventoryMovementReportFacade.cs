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

namespace Com.Danliris.Service.Inventory.Lib.Facades.InventoryFacades
{
    public class InventoryMovementReportFacade
    {
        private readonly InventoryDbContext DbContext;
        private readonly DbSet<InventoryMovement> DbSet;
        private IdentityService IdentityService;

        public InventoryMovementReportFacade(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<InventoryMovement>();
            this.IdentityService = serviceProvider.GetService<IdentityService>();
        }

        public IQueryable<InventoryMovementViewModel> GetReportQuery(string storageCode, string productCode, string type, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from a in DbContext.InventoryMovements
                             //Conditions
                         where a._IsDeleted == false
                             && a.StorageCode == (string.IsNullOrWhiteSpace(storageCode) ? a.StorageCode : storageCode)
                             && a.ProductCode == (string.IsNullOrWhiteSpace(productCode) ? a.ProductCode : productCode)
                             && a.Type == (string.IsNullOrWhiteSpace(type) ? a.Type : type)
                             && a.Date.AddHours(offset).Date >= DateFrom.Date
                             && a.Date.AddHours(offset).Date <= DateTo.Date
                         select new InventoryMovementViewModel
                         {
                             no = a.No,
                             date = a.Date,
                             referenceNo = a.ReferenceNo,
                             referenceType = a.ReferenceType,
                             productId = a.ProductId,
                             productCode = a.ProductCode,
                             productName = a.ProductName,
                             uomId = a.UomId,
                             uomUnit = a.UomUnit,
                             storageId = a.StorageId,
                             storageCode = a.StorageCode,
                             storageName = a.StorageName,
                             stockPlanning = a.StockPlanning,
                             before = a.Before,
                             quantity = a.Quantity,
                             after = a.After,
                             remark = a.Remark,
                             type = a.Type,
                             _LastModifiedUtc = a._LastModifiedUtc
                         });
            return Query;
        }
        public Tuple<List<InventoryMovementViewModel>, int> GetReport(string storageCode, string productCode, string type, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(storageCode, productCode, type, dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b._LastModifiedUtc);
            }

            Pageable<InventoryMovementViewModel> pageable = new Pageable<InventoryMovementViewModel>(Query, page - 1, size);
            List<InventoryMovementViewModel> Data = pageable.Data.ToList<InventoryMovementViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }
        public MemoryStream GenerateExcel(string storageCode, string productCode, string type, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQuery(storageCode, productCode, type, dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(b => b._LastModifiedUtc);
            DataTable result = new DataTable();
            //No	Unit	Budget	Kategori	Tanggal PR	Nomor PR	Kode Barang	Nama Barang	Jumlah	Satuan	Tanggal Diminta Datang	Status	Tanggal Diminta Datang Eksternal

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Storage", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Referensi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Referensi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "UOM", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Before", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kuantiti", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "After", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Type", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", 0, 0, 0, ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    //DateTimeOffset date = item.date ?? new DateTime(1970, 1, 1);
                    //string dateString = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.storageName, item.referenceNo, item.referenceType, item.date.ToString("dd MMM yyyy", new CultureInfo("id-ID")), item.productName, item.uomUnit, item.before,
                        item.quantity, item.after, item.type);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
    }
}
