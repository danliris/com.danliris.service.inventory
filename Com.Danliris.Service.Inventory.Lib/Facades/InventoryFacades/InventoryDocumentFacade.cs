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
    public class InventoryDocumentFacade
    {
        private readonly InventoryDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<InventoryDocument> dbSet;

        public InventoryDocumentFacade(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<InventoryDocument>();
        }

        public Tuple<List<InventoryDocument>, int, Dictionary<string, string>> Read(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            IQueryable<InventoryDocument> Query = this.dbSet;

            List<string> SearchAttributes = new List<string>()
                {
                    "No", "ReferenceNo", "StorageName","ReferenceType","Type"
                };
            if (keyword != null)
            {
                Query = Query.Where(General.BuildSearch(SearchAttributes), keyword);
            }

            Query = Query
                .Select(s => new InventoryDocument
                {
                    Id = s.Id,
                    No = s.No,
                    ReferenceNo = s.ReferenceNo,
                    ReferenceType = s.ReferenceType,
                    Date = s.Date,
                    StorageCode = s.StorageCode,
                    StorageId = s.StorageId,
                    StorageName = s.StorageName,
                    Type=s.Type,
                    _LastModifiedUtc=s._LastModifiedUtc,
                    Items = s.Items.Select(a=>new InventoryDocumentItem {
                        Quantity=a.Quantity,
                        ProductCode=a.ProductCode,
                        ProductId=a.ProductId,
                        ProductName=a.ProductName,
                        UomId=a.UomId,
                        UomUnit=a.UomUnit,
                    }).ToList()
                });

            //List<string> searchAttributes = new List<string>()
            //{
            //    "No", "ReferenceNo", "StorageName","ReferenceType","Type"
            //};

            //if (keyword != null)
            //{
            //    Query = Query.Where(General.BuildSearch(searchAttributes), keyword);
            //}
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

            Pageable<InventoryDocument> pageable = new Pageable<InventoryDocument>(Query, page - 1, size);
            List<InventoryDocument> Data = pageable.Data.ToList<InventoryDocument>();
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
                    Items = s.Items.ToList(),
                    _CreatedUtc = s._CreatedUtc
                }).ToList()
            );

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public InventoryDocument ReadModelById(int id)
        {
            var a = this.dbSet.Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                .Include(p => p.Items)
                .FirstOrDefault();
            return a;
        }

        public async Task<int> Create(InventoryDocument model, string username)
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

                    foreach (var item in model.Items)
                    {
                        item._CreatedAgent = "Facade";
                        item._CreatedBy = username;
                        item._LastModifiedAgent = "Facade";
                        item._LastModifiedBy = username;
                        item._CreatedUtc = DateTime.UtcNow;
                        item._LastModifiedUtc = DateTime.UtcNow;


                    }

                    this.dbSet.Add(model);
                    Created = await dbContext.SaveChangesAsync();
                    transaction.Commit();

                    foreach (var item in model.Items)
                    {
                        var qty = item.Quantity;
                        if (model.Type == "OUT")
                        {
                            qty = item.Quantity * -1;
                        }
                        var SumQty = dbContext.InventoryMovements.Where(a => a._IsDeleted == false && a.StorageId == model.StorageId && a.ProductId == item.ProductId && a.UomId == item.UomId).Sum(a => a.Quantity);
                        InventoryMovementFacade movement = new InventoryMovementFacade(this.serviceProvider, this.dbContext);
                        InventoryMovement movementModel = new InventoryMovement
                        {
                            ProductCode = item.ProductCode,
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            StorageCode = model.StorageCode,
                            StorageId = model.StorageId,
                            StorageName = model.StorageName,
                            Before = SumQty,
                            Quantity = qty,
                            After = SumQty + qty,
                            ReferenceNo = model.ReferenceNo,
                            ReferenceType = model.ReferenceType,
                            Type=model.Type,
                            Date = model.Date,
                            UomId=item.UomId,
                            UomUnit=item.UomUnit,
                            Remark=item.ProductRemark
                        };
                        await movement.Create(movementModel, username);
                    }
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Created;
        }

        async Task<string> GenerateNo(InventoryDocument model)
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



            //string no = $"DOC-{Year}-{Month}-{model.StorageCode}-";
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