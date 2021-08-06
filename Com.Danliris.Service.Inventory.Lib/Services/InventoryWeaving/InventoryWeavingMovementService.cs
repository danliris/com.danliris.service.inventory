using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using Com.Moonlay.Models;
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
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving
{
    public class InventoryWeavingMovementService : IInventoryWeavingMovementService
    {
        private string USER_AGENT = "Service";
        private const string UserAgent = "inventory-service";
        //private const string UserAgent = "inventory-service";
        protected DbSet<InventoryWeavingMovement> DbSet;
        protected DbSet<InventoryWeavingMovement> DbSetMovement;
        protected DbSet<InventoryWeavingDocumentItem> DbSetItem;
        protected DbSet<InventoryWeavingDocument> DbSetDoc;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;

        public InventoryWeavingMovementService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSetItem = dbContext.Set<InventoryWeavingDocumentItem>();
            DbSetMovement = dbContext.Set<InventoryWeavingMovement>();
            DbSetDoc = dbContext.Set<InventoryWeavingDocument>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
            //IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> Create(InventoryWeavingMovement model, string username)
        {
            

            int Created = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    model.FlagForCreate(username, USER_AGENT);
                    model.FlagForUpdate(username, USER_AGENT);
                    DbSet.Add(model);
                    Created = await DbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception("Insert Error : " + e.Message);
                }
            }

            return Created;
        }


        public async Task<int> UpdateAsync(InventoryWeavingMovement model)
        {
            
                try
                {
                    int Updated = 0;

                    var existingStock = DbSetMovement.Where(x => x.InventoryWeavingDocumentItemId == model.InventoryWeavingDocumentItemId).FirstOrDefault();


                    if (existingStock.Quantity != model.Quantity)
                    {
                        existingStock.Quantity = model.Quantity;
                    }
                    if (existingStock.QuantityPiece != model.QuantityPiece)
                    {
                        existingStock.QuantityPiece = model.QuantityPiece;
                    }
                    existingStock.FlagForUpdate(IdentityService.Username, UserAgent);

                    Updated = await DbContext.SaveChangesAsync();



                   

                    return Updated;
                }
                catch (Exception e)
                {
                    
                    throw e;
                }
            
        }


        /*  public ListResult<InventoryWeavingMovementDetailViewModel> ReadReport(string grade, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int page, int size, string order, int offset)
           {
               IQueryable<InventoryWeavingMovement> Query = this.DbSet;


               List<string> SelectedFields = new List<string>()
                        {
                          "grade", "quantityPiece", "quantity"
                        };

               DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
               DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;
               var query = (from a in DbContext.InventoryWeavingDocuments
                            join b in DbContext.InventoryWeavingMovements on a.Id equals b.InventoryWeavingDocumentId
                            //Conditions
                            where a._IsDeleted == false
                            && b._IsDeleted == false
                             //   && b.Type == (string.IsNullOrWhiteSpace(type) ? b.Type : type)
                               && b.Grade == (string.IsNullOrWhiteSpace(grade) ? b.Grade : grade)
                              // && b.Type == "IN"
                                && a.Date.AddHours(offset).Date >= DateFrom.Date
                                && a.Date.AddHours(offset).Date <= DateTo.Date
                            select new InventoryWeavingMovementDetailViewModel
                            {
                                Construction = b.Construction,
                                Grade = b.Grade,
                                QuantityPiece = b.QuantityPiece,
                                Quantity = b.Quantity,
                            }); 

                var data = query.Skip((page - 1) * size).Take(size);


               Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
               Query = QueryHelper<InventoryWeavingMovement>.Order(Query, OrderDictionary);

               return new ListResult<InventoryWeavingMovementDetailViewModel>(data.ToList(), page, size, data.Count());
           } */

        public Tuple<List<InventoryWeavingInOutViewModel>, int> ReadReportGrade(string grade, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query1 = GetReportQuery(grade, dateTo, offset);

            var Query =Query1.Take(50);

            
            Pageable<InventoryWeavingInOutViewModel> pageable = new Pageable<InventoryWeavingInOutViewModel>(Query, page - 1, size);
            List<InventoryWeavingInOutViewModel> Data = pageable.Data.ToList<InventoryWeavingInOutViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        /*   public List<InventoryWeavingItemDetailViewModel> GetGrade(string grade, DateTimeOffset? dateTo, int offset)
           {
               IQueryable<InventoryWeavingMovement> query = this.DbSetMovement;
              DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;

               if (grade == null && dateTo == null)
               {

                   query = query.OrderByDescending(s => s._LastModifiedUtc).Where(s => s._IsDeleted.Equals(false));
               }
               else if (grade != null && dateTo == null)
               {
                   query = query.OrderByDescending(s => s._LastModifiedUtc).Where(s => s.Grade.Contains(grade) && s._IsDeleted.Equals(false));

               }
               else {
                   query = query.OrderByDescending(s => s._LastModifiedUtc).Where(s => s.Grade.Contains(grade) && s._IsDeleted.Equals(false) && s._CreatedUtc.Date == dateTo);
               }

               // DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
               //DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;
               var data = query.GroupBy(s => new {
                   s.Grade,
                   s.Piece,
                   s.Type,
                   s.MaterialName,
                   s.WovenType,
                   s.Width,
                   s.Yarn1,
                   s.Yarn2,
                   s.YarnOrigin1,
                   s.YarnOrigin2,
                   s.YarnType1,
                   s.YarnType2
               }).Select(s => new InventoryWeavingInOutViewModel()
               {
                   Construction = s.FirstOrDefault().Construction,
                   Grade = s.Key.Grade,

                   Type = s.Key.Type,
                   Qty = s.Sum(d => d.Quantity),
                   QtyPiece = s.Sum(d => d.QuantityPiece),

                   //QtyOut = 0,
                   //QtyPieceOut = 0

               });


               var result = data.GroupBy(s => new
               {
                   s.Grade,
                   s.MaterialName,
                   s.WovenType,
                   s.Width,
                   s.Yarn1,
                   s.Yarn2,
                   s.YarnOrigin1,
                   s.YarnOrigin2,
                   s.YarnType1,
                   s.YarnType2
               }).Select(s => new InventoryWeavingInOutViewModel()
               {

                   Construction = s.FirstOrDefault().Construction,

                   Grade = s.Key.Grade,
                   // Piece = item.Key.Piece == "1" ? "BESAR" : item.Key.Piece == "2" ? "KECIL" : "POTONGAN",

                   Qty = s.FirstOrDefault(d => d.Type == "OUT") != null ? s.FirstOrDefault(d => d.Type == "IN").Qty - s.FirstOrDefault(d => d.Type == "OUT").Qty :
                                    s.FirstOrDefault(d => d.Type == "IN").Qty,
                   QtyPiece = s.FirstOrDefault(d => d.Type == "OUT") != null ? s.FirstOrDefault(d => d.Type == "IN").QtyPiece - s.FirstOrDefault(d => d.Type == "OUT").QtyPiece :
                                    s.FirstOrDefault(d => d.Type == "IN").QtyPiece
                   //Quantity = item.FirstOrDefault().QtyOut== null ? item.FirstOrDefault().QtyIn : item.FirstOrDefault().QtyIn - item.FirstOrDefault().QtyOut
               }).Where(x => x.Qty > 0 && x.QtyPiece > 0).ToList();




               return result;
           } */

        public MemoryStream GenerateExcel(string grade,  DateTime? dateTo, int offset)
        {
            
            var query = GetReportQuery(grade, dateTo, offset);
            //query = query.OrderByDescending(b => b._LastModifiedUtc);
            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Konstruksi", DataType = typeof(String) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Grade", DataType = typeof(String) });
           // dt.Columns.Add(new DataColumn() { ColumnName = "Piece", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Meter", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(String) });
            if (query.ToArray().Count() == 0)
                dt.Rows.Add("", "", "", 0, ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in query)
                {
                    index++;
                    dt.Rows.Add(index, item.Construction, item.Grade, item.QtyPiece, item.Qty);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Inventory Weaving") }, true);
        }

        public IQueryable<InventoryWeavingInOutViewModel> GetReportQuery(string grade, DateTime? dateTo, int offset)
        {
            IQueryable<InventoryWeavingMovement> query = this.DbSetMovement;
            
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var DateFrom = query.Where( c => c._IsDeleted == false).OrderBy(x => x._CreatedUtc).Select( d => new { d._CreatedUtc.Date }).Take(1);

            if (grade == null && dateTo == null)
            {

                query = query.OrderByDescending(s => s._LastModifiedUtc).Where(s => s._IsDeleted.Equals(false));
            }
            else if (grade != null && dateTo == null)
            {
                query = query.OrderByDescending(s => s._LastModifiedUtc).Where(s => s.Grade.Contains(grade) && s._IsDeleted.Equals(false));

            }
            else if (grade == null && dateTo != null)
            {
                query = query.OrderByDescending(s => s._LastModifiedUtc).Where(s => s._IsDeleted.Equals(false) && s._CreatedUtc.Date >= DateFrom.FirstOrDefault().Date && s._CreatedUtc.Date <= dateTo);
            }

            else
            {
                query = query.OrderByDescending(s => s._LastModifiedUtc).Where(s => s.Grade.Contains(grade) && s._IsDeleted.Equals(false) && s._CreatedUtc.Date >= DateFrom.FirstOrDefault().Date && s._CreatedUtc.Date <= dateTo);
            }
           

            var data = query.GroupBy(s => new {
                s.Grade,
               
                s.Type,
                s.MaterialName,
                s.WovenType,
                s.Width,
                s.Yarn1,
                s.Yarn2,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2
            }).Select(s => new InventoryWeavingInOutViewModel()
            {
                Construction = s.FirstOrDefault().Construction,
                Grade = s.Key.Grade,

                Type = s.Key.Type,
                Qty = s.Sum(d => d.Quantity),
                QtyPiece = s.Sum(d => d.QuantityPiece),
                MaterialName = s.Key.MaterialName,
                WovenType = s.Key.WovenType,
                Width = s.Key.Width,
                Yarn1 = s.Key.Yarn1,
                Yarn2 = s.Key.Yarn2,
                YarnOrigin1 = s.Key.YarnOrigin1,
                YarnOrigin2 = s.Key.YarnOrigin2,
                YarnType1 = s.Key.YarnType1,
                YarnType2 = s.Key.YarnType2

                //QtyOut = 0,
                //QtyPieceOut = 0

            });

            var result = data.GroupBy(s => new
            {
                s.Grade,
                s.MaterialName,
                s.WovenType,
                s.Width,
                s.Yarn1,
                s.Yarn2,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2
            }).Select(s => new InventoryWeavingInOutViewModel()
            {

                Construction = s.FirstOrDefault().Construction,

                Grade = s.Key.Grade,
                // Piece = item.Key.Piece == "1" ? "BESAR" : item.Key.Piece == "2" ? "KECIL" : "POTONGAN",

                Qty = s.FirstOrDefault(d => d.Type == "OUT") != null ? s.FirstOrDefault(d => d.Type == "IN").Qty - s.FirstOrDefault(d => d.Type == "OUT").Qty :
                          s.FirstOrDefault(d => d.Type == "OUT") == null ? s.FirstOrDefault(d => d.Type == "IN").Qty :0,
                QtyPiece = s.FirstOrDefault(d => d.Type == "OUT") != null ? s.FirstOrDefault(d => d.Type == "IN").QtyPiece - s.FirstOrDefault(d => d.Type == "OUT").QtyPiece :
                             s.FirstOrDefault(d => d.Type == "OUT") == null ? s.FirstOrDefault(d => d.Type == "IN").QtyPiece:0
                //Quantity = item.FirstOrDefault().QtyOut== null ? item.FirstOrDefault().QtyIn : item.FirstOrDefault().QtyIn - item.FirstOrDefault().QtyOut
            }).Where(x => x.Qty > 0 && x.QtyPiece > 0).ToList();
            
            return result.AsQueryable(); 
        }



        //RINCIAN PEMASUKAN
        public Tuple<List<InventoryWeavingInOutViewModel>, int> ReadReportRecap(string bonType, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query2 = GetReport(bonType, dateFrom, dateTo, offset);

            var Query3 = Query2.Take(50);


            Pageable<InventoryWeavingInOutViewModel> pageable = new Pageable<InventoryWeavingInOutViewModel>(Query2, page - 1, size);
            List<InventoryWeavingInOutViewModel> data = pageable.Data.ToList<InventoryWeavingInOutViewModel>();
            int totalData = pageable.TotalCount;

            return Tuple.Create(data, totalData);
        }

        public MemoryStream GenerateExcelRecap(string bonType,  DateTime? dateFrom, DateTime? dateTo, int offset)
        {

            var Query = GetReport(bonType, dateFrom, dateTo, offset);
            //query = query.OrderByDescending(b => b._LastModifiedUtc);
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Konstruksi", DataType = typeof(String) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Kg", DataType = typeof(String) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Ball", DataType = typeof(String) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Jml. Piece", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Jml. Meter", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Ket", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                dt.Rows.Add("", "", "", "", 0, 0, ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    dt.Rows.Add(index, item.Construction, "", "", item.QtyPiece, item.Qty);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Inventory Weaving") }, true);
        }

        public List<InventoryWeavingInOutViewModel> GetReport(string bonType, DateTime? dateFrom, DateTime? dateTo, int offset)
        {

            IQueryable<InventoryWeavingMovement> Query = this.DbSetMovement;

            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var result = (from a in DbContext.InventoryWeavingDocuments
                          join b in DbContext.InventoryWeavingMovements on a.Id equals b.InventoryWeavingDocumentId
                          where a._IsDeleted == false
                             && b._IsDeleted == false
                             && a.BonType == (string.IsNullOrWhiteSpace(bonType) ? a.BonType : bonType)
                               && a.Date.AddHours(offset).Date >= DateFrom.Date
                              && a.Date.AddHours(offset).Date <= DateTo.Date
                          orderby a.Date, a._CreatedUtc ascending
                          select new InventoryWeavingInOutViewModel
                          {
                              Construction = b.Construction,
                              Type = b.Type,
                              MaterialName = b.MaterialName,
                              WovenType = b.WovenType,
                              Yarn1 = b.Yarn1,
                              Yarn2 = b.Yarn2,
                              YarnOrigin1 = b.YarnOrigin1,
                              YarnOrigin2 = b.YarnOrigin2,
                              YarnType1 = b.YarnType1,
                              YarnType2 = b.YarnType2,
                              Qty = b.Quantity,
                              QtyPiece = b.QuantityPiece


                          });

            var data1 = result.GroupBy(s => new
            {
                s.Grade,

                s.Type,
                s.MaterialName,
                s.WovenType,
                s.Width,
                s.Yarn1,
                s.Yarn2,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2
            }).Select(s => new InventoryWeavingInOutViewModel()
            {
                Construction = s.FirstOrDefault().Construction,
                Grade = s.Key.Grade,

                Type = s.Key.Type,
                Qty = s.Sum(d => d.Qty),
                QtyPiece = s.Sum(d => d.QtyPiece),
                MaterialName = s.Key.MaterialName,
                WovenType = s.Key.WovenType,
                Width = s.Key.Width,
                Yarn1 = s.Key.Yarn1,
                Yarn2 = s.Key.Yarn2,
                YarnOrigin1 = s.Key.YarnOrigin1,
                YarnOrigin2 = s.Key.YarnOrigin2,
                YarnType1 = s.Key.YarnType1,
                YarnType2 = s.Key.YarnType2



            });
                         
                        var data = data1.GroupBy(s => new
                         {
                             s.Grade,
                             s.MaterialName,
                             s.WovenType,
                             s.Width,
                             s.Yarn1,
                             s.Yarn2,
                             s.YarnOrigin1,
                             s.YarnOrigin2,
                             s.YarnType1,
                             s.YarnType2
                         }).Select(s => new InventoryWeavingInOutViewModel()
                         {

                             Construction = s.FirstOrDefault().Construction,
                             Qty = s.FirstOrDefault(d => d.Type == "OUT") != null ? s.FirstOrDefault(d => d.Type == "IN").Qty - s.FirstOrDefault(d => d.Type == "OUT").Qty :
                                          s.FirstOrDefault(d => d.Type == "OUT") == null ? s.FirstOrDefault(d => d.Type == "IN").Qty : 0,
                             QtyPiece = s.FirstOrDefault(d => d.Type == "OUT") != null ? s.FirstOrDefault(d => d.Type == "IN").QtyPiece - s.FirstOrDefault(d => d.Type == "OUT").QtyPiece :
                                             s.FirstOrDefault(d => d.Type == "OUT") == null ? s.FirstOrDefault(d => d.Type == "IN").QtyPiece : 0
                         }).Where(x => x.Qty > 0 && x.QtyPiece > 0).ToList();
                        
            return data.ToList();
            

        } 
    }
}
