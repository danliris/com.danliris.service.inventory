using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Facades.InventoryFacades
{
    public class InventoryMovementFacade
    {
        private readonly InventoryDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<InventoryMovement> dbSet;

        public InventoryMovementFacade(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<InventoryMovement>();
        }

        public Tuple<List<object>, int, Dictionary<string, string>> Read(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            IQueryable<InventoryMovement> Query = this.dbSet;

            Query = Query
                .Select(s => new InventoryMovement
                {
                    Id = s.Id,
                    No = s.No,
                    ReferenceNo = s.ReferenceNo,
                    ReferenceType = s.ReferenceType,
                    Date = s.Date,
                    StorageCode = s.StorageCode,
                    StorageId = s.StorageId,
                    StorageName = s.StorageName,
                    ProductCode = s.ProductCode,
                    ProductId = s.ProductId,
                    ProductName = s.ProductName,
                    Quantity = s.Quantity,
                    StockPlanning = s.StockPlanning,
                    Before = s.Before,
                    After = s.After
                });

            List<string> searchAttributes = new List<string>()
            {
                "No", "ReferenceNo", "StorageName","ReferenceType"
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

            #region Paging

            Pageable<InventoryMovement> pageable = new Pageable<InventoryMovement>(Query, page - 1, size);
            List<InventoryMovement> Data = pageable.Data.ToList<InventoryMovement>();
            int TotalData = pageable.TotalCount;

            #endregion Paging

            List<object> list = new List<object>();
            list.AddRange(
                Data.Select(s => new
                {
                    Id = s.Id,
                    No = s.No,
                    ReferenceNo = s.ReferenceNo,
                    ReferenceType = s.ReferenceType,
                    Date = s.Date,
                    StorageCode = s.StorageCode,
                    StorageId = s.StorageId,
                    StorageName = s.StorageName,
                    _CreatedUtc = s._CreatedUtc,
                    ProductCode = s.ProductCode,
                    ProductId = s.ProductId,
                    ProductName = s.ProductName,
                    Quantity = s.Quantity,
                    StockPlanning = s.StockPlanning,
                    Before = s.Before,
                    After = s.After
                }).ToList()
            );

            return Tuple.Create(list, TotalData, OrderDictionary);
        }

        public InventoryMovement ReadModelById(int id)
        {
            var a = this.dbSet.Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                .FirstOrDefault();
            return a;
        }

        public async Task<int> Create(InventoryMovement model, string username)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    model.No = await GenerateNo(model);
                    model._CreatedAgent = "Facade";
                    model._CreatedBy = username;
                    model._LastModifiedAgent = "Facade";
                    model._LastModifiedBy = username;
                    model._CreatedUtc = DateTime.UtcNow;
                    model._LastModifiedUtc = DateTime.UtcNow;

                    InventorySummaryFacade summary = new InventorySummaryFacade(this.serviceProvider, this.dbContext);

                    this.dbSet.Add(model);
                    Created = await dbContext.SaveChangesAsync();
                    transaction.Commit();

                    var SumQty = this.dbSet.Where(a => a._IsDeleted == false && a.StorageId == model.StorageId && a.ProductId == model.ProductId && a.UomId == model.UomId).Sum(a => a.Quantity);
                    var SumStock = this.dbSet.Where(a => a._IsDeleted == false && a.StorageId == model.StorageId && a.ProductId == model.ProductId && a.UomId == model.UomId).Sum(a => a.StockPlanning);
                    InventorySummary summaryModel = new InventorySummary
                    {
                        ProductId = model.ProductId,
                        ProductCode = model.ProductCode,
                        ProductName = model.ProductName,
                        UomId = model.UomId,
                        UomUnit = model.UomUnit,
                        StockPlanning = SumStock,
                        Quantity = SumQty,
                        StorageId = model.StorageId,
                        StorageCode = model.StorageCode,
                        StorageName = model.StorageName
                    };
                    await summary.Create(summaryModel, username);
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Created;
        }

        async Task<string> GenerateNo(InventoryMovement model)
        {
            CodeGenerator codeGenerator = new CodeGenerator();

            do
            {
                model.No = codeGenerator.GenerateCode();
            }
            while (this.dbSet.Any(d => d.No.Equals(model.No)));

            return model.No;
            //string Year = model.Date.ToString("yy");
            //string Month = model.Date.ToString("MM");


            //string no = $"MOV-{Year}-{Month}-{model.StorageCode}-";
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

