using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Facades.InventoryFacades
{
    public class InventorySummaryFacade
    {
        private readonly InventoryDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<InventorySummary> dbSet;

        public InventorySummaryFacade(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<InventorySummary>();
        }

        public Tuple<List<object>, int, Dictionary<string, string>> Read(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            IQueryable<InventorySummary> Query = this.dbSet;

            Query = Query
                .Select(s => new InventorySummary
                {
                    Id = s.Id,
                    No = s.No,
                    StorageCode = s.StorageCode,
                    StorageId = s.StorageId,
                    StorageName = s.StorageName,
                    ProductCode = s.ProductCode,
                    ProductId = s.ProductId,
                    ProductName = s.ProductName,
                    Quantity = s.Quantity,
                    StockPlanning = s.StockPlanning
                });

            List<string> searchAttributes = new List<string>()
            {
                "ProductCode",
            };

            if (keyword != null)
            {
                Query = Query.Where(General.BuildSearch(searchAttributes), keyword);
            }

            #region OrderBy

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
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

            #endregion OrderBy

            //Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            //if (FilterDictionary != null && !FilterDictionary.Count.Equals(0))
            //{
            //    foreach (var f in FilterDictionary)
            //    {
            //        string Key = f.Key;
            //        object Value = f.Value;
            //        string filterQuery = string.Concat(string.Empty, Key, " == @0");

            //        Query = Query.Where(filterQuery, Value);
            //    }
            //}

            #region Paging

            Pageable<InventorySummary> pageable = new Pageable<InventorySummary>(Query, page - 1, size);
            List<InventorySummary> Data = pageable.Data.ToList<InventorySummary>();
            int TotalData = pageable.TotalCount;

            #endregion Paging

            List<object> list = new List<object>();
            list.AddRange(
                Data.Select(s => new
                {
                    Id = s.Id,
                    No = s.No,
                    StorageCode = s.StorageCode,
                    StorageId = s.StorageId,
                    StorageName = s.StorageName,
                    _CreatedUtc = s._CreatedUtc,
                    ProductCode = s.ProductCode,
                    ProductId = s.ProductId,
                    ProductName = s.ProductName,
                    Quantity = s.Quantity,
                    StockPlanning = s.StockPlanning
                }).ToList()
            );

            return Tuple.Create(list, TotalData, OrderDictionary);
        }

        public InventorySummary ReadModelById(int id)
        {
            var a = this.dbSet.Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                .FirstOrDefault();
            return a;
        }

        public List<string> GetProductCodeForMaterialRequestNote(string storageName)
        {
            IQueryable<InventorySummary> Query = this.dbSet;

            Query = Query
                .Where(s => s.StorageName.Equals(storageName) && s.UomUnit.Equals("MTR"));

            return Query.Select(s=> s.ProductCode).ToList();
        }

        public async Task<int> Create(InventorySummary model, string username)
        {
            int Created = 0;
            var internalTransaction = dbContext.Database.CurrentTransaction == null;
            var transaction = !internalTransaction ? dbContext.Database.CurrentTransaction : dbContext.Database.BeginTransaction();

            try
            {
                var exist = this.dbSet.Where(a => a._IsDeleted == false && a.StorageId == model.StorageId && a.ProductId == model.ProductId && a.UomId == model.UomId).FirstOrDefault();

                if (exist == null)
                {
                    model.No = await GenerateNo(model);
                    model._CreatedAgent = "Facade";
                    model._CreatedBy = username;
                    model._LastModifiedAgent = "Facade";
                    model._LastModifiedBy = username;
                    model._CreatedUtc = DateTime.UtcNow;
                    model._LastModifiedUtc = DateTime.UtcNow;

                    this.dbSet.Add(model);
                }
                else
                {
                    model._LastModifiedAgent = "Facade";
                    model._LastModifiedBy = username;
                    model._LastModifiedUtc = DateTime.UtcNow;

                    exist.Quantity = model.Quantity;
                    exist.StockPlanning = model.StockPlanning;
                    this.dbSet.Update(exist);
                }
                Created = await dbContext.SaveChangesAsync();

                if (internalTransaction)
                    transaction.Commit();

                return Created;
            }
            catch (Exception e)
            {
                if (internalTransaction)
                    transaction.Rollback();
                throw new Exception(e.Message);
            }
        }

        async Task<string> GenerateNo(InventorySummary model)
        {
            do
            {
                model.No = CodeGenerator.GenerateCode();
            }
            while (this.dbSet.Any(d => d.No.Equals(model.No)));

            return model.No;
            //string Year = DateTimeOffset.Now.ToString("yy");
            //string Month = DateTimeOffset.Now.ToString("MM");


            //string no = $"SUM-{Year}-{Month}-{model.StorageCode}-";
            //int Padding = 7;

            //var lastNo = await this.dbSet.Where(w => w.No.StartsWith(no) && !w._IsDeleted).OrderByDescending(o => o.No).FirstOrDefaultAsync();

            //if (lastNo == null)
            //{
            //    return no + "1".PadLeft(Padding, '0');
            //}
            //else
            //{
            //    int lastNoNumber = Int32.Parse(lastNo.No.Replace(no, "")) + 1;
            //    return no + lastNoNumber.ToString().PadLeft(Padding, '0');
            //}
        }
    }
}

