using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving
{
    public class InventoryWeavingDocumentOutService : IInventoryWeavingDocumentOutService
    {
        private string USER_AGENT = "Service";
        private const string UserAgent = "inventory-service";
        protected DbSet<InventoryWeavingDocumentItem> DbSetItem;
        protected DbSet<InventoryWeavingMovement> DbSetMovement;
        protected DbSet<InventoryWeavingDocument> DbSetDoc;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;


        public InventoryWeavingDocumentOutService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSetItem = dbContext.Set<InventoryWeavingDocumentItem>();
            DbSetMovement = dbContext.Set<InventoryWeavingMovement>();
            DbSetDoc = dbContext.Set<InventoryWeavingDocument>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
            //IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public ListResult<InventoryWeavingDocument> Read(int page, int size, string order, string keyword, string filter)
        {
            var query = this.DbSetDoc.Where(s => s.Type == "OUT");


            List<string> SearchAttributes = new List<string>()
            {
                "BonNo"
            };

            query = QueryHelper<InventoryWeavingDocument>.Search(query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<InventoryWeavingDocument>.Filter(query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<InventoryWeavingDocument>.Order(query, OrderDictionary);
            var data = query.Skip((page - 1) * size).Take(size).Select(s => new InventoryWeavingDocument()
            {
                Id = s.Id,

                Date = s.Date,
                BonNo = s.BonNo,
                BonType = s.BonType,
                StorageCode = s.StorageCode,
                StorageId = s.StorageId,
                StorageName = s.StorageName,
                Type = s.Type,
                _LastModifiedUtc = s._LastModifiedUtc,
            });

            return new ListResult<InventoryWeavingDocument>(data.ToList(), page, size, data.Count());
        }

        public ReadResponse<InventoryWeavingDocumentItem> GetDistinctMaterial(int page, int size, string filter, string order, string keyword)
        {
            IQueryable<InventoryWeavingDocumentItem> Query = this.DbSetItem;


            List<string> SearchAttributes = new List<string>()
                {
                    "Construction"
                };
            Query = QueryHelper<InventoryWeavingDocumentItem>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<InventoryWeavingDocumentItem>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<InventoryWeavingDocumentItem>.Order(Query, OrderDictionary);

            List<string> SelectedFields = new List<string>()
            {
                "Id", "Construction", 
            };

            var Data = Query.GroupBy(d => d.Construction)
                .Select(s => s.First())
                .Skip((page - 1) * size).Take(size)
                .OrderBy(s => s.Construction)
                .Select(s => new InventoryWeavingDocumentItem
                {
                    Id = s.Id,
                    Construction = s.Construction,
                });


            #region OrderBy

            //Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            //Query = QueryHelper<InventoryWeavingDocumentItem>.Order(Query, OrderDictionary);
            #endregion OrderBy

            #region Paging

            Pageable<InventoryWeavingDocumentItem> pageable = new Pageable<InventoryWeavingDocumentItem>(Data, page - 1, size);
            List<InventoryWeavingDocumentItem> data = pageable.Data.ToList<InventoryWeavingDocumentItem>();
            int totalData = pageable.TotalCount;

            #endregion Paging

            return new ReadResponse<InventoryWeavingDocumentItem>(data, totalData, OrderDictionary, new List<string>());
        }

        public List<InventoryWeavingItemDetailViewModel> GetMaterialItemList(string material)
        {
            IQueryable<InventoryWeavingMovement> query = this.DbSetMovement;

           if (material == null)
            {

                query = query.OrderByDescending(s => s._LastModifiedUtc).Where(s => s._IsDeleted.Equals(false));
            }
            else
            {
                query = query.OrderByDescending(s => s._LastModifiedUtc).Where(s => s.Construction.Contains(material) && s._IsDeleted.Equals(false));

            }

           var data = query.GroupBy( s => new { s.Grade,  s.Type, s.MaterialName, s.WovenType, s.Width,
                      s.Yarn1, s.Yarn2, s.YarnOrigin1, s.YarnOrigin2, s.YarnType1, s.YarnType2}).Select( s => new InventoryWeavingInOutViewModel()
           {
               Construction = s.FirstOrDefault().Construction,
               Grade = s.Key.Grade,
               Piece = s.FirstOrDefault().Piece,

               MaterialName = s.Key.MaterialName,
               WovenType = s.Key.WovenType,
               Width = s.Key.Width,
               Yarn1 = s.Key.Yarn1,
               Yarn2 = s.Key.Yarn2,
               YarnOrigin1 = s.Key.YarnOrigin1,
               YarnOrigin2 = s.Key.YarnOrigin2,
               YarnType1 = s.Key.YarnType1,
               YarnType2 = s.Key.YarnType2,
               UomUnit = s.FirstOrDefault().UomUnit,
               Type = s.Key.Type,
               Qty = s.Sum( d => d.Quantity),
               QtyPiece = s.Sum(d => d.QuantityPiece),
               Barcode = s.FirstOrDefault().Barcode,
               ProductionOrderDate = s.FirstOrDefault().ProductionOrderDate,
               //QtyOut = 0,
               //QtyPieceOut = 0

           });

           // var dataOUT = query.GroupBy(s => new { s.Construction, s.Grade, s.Piece }).Where(x => x.FirstOrDefault().Type == "OUT").Select(s => new InventoryWeavingInOutViewModel()
           // {
           //     Construction = s.Key.Construction,
           //     Grade = s.Key.Grade,
           //     Piece = s.Key.Piece,
           //     Type = s.FirstOrDefault().Type,
           //     QtyIn = 0,
           //     QtyPieceIn =0,
           //     QtyOut = s.Sum(d => d.Quantity),
           //     QtyPieceOut = s.Sum( d=> d.QuantityPiece)
           //});


          

            var result = data.GroupBy(s => new {
                s.MaterialName,
                s.WovenType,
                s.Width,
                s.Yarn1,
                s.Yarn2,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2,
            }).Select(s => new InventoryWeavingItemDetailViewModel()
            {

                Construction = s.FirstOrDefault().Construction,
                ListItems = s.GroupBy(x => new { x.Grade }).Select(item => new ItemListDetailViewModel()
                {
                    Grade = item.Key.Grade,
                    //Piece = item.FirstOrDefault().Piece == "1" ? "BESAR": item.FirstOrDefault().Piece == "2" ? "KECIL" : "POTONGAN",

                    MaterialName = item.FirstOrDefault().MaterialName,
                    WovenType = item.FirstOrDefault().WovenType,
                    Width = item.FirstOrDefault().Width,
                    Yarn1 = item.FirstOrDefault().Yarn1,
                    Yarn2  = item.FirstOrDefault().Yarn2,
                    YarnOrigin1 = item.FirstOrDefault().YarnOrigin1,
                    YarnOrigin2 = item.FirstOrDefault().YarnOrigin2,
                    YarnType1 = item.FirstOrDefault().YarnType1,
                    YarnType2 = item.FirstOrDefault().YarnType2,
                    //Width = item.FirstOrDefault().Width,
                    UomUnit = item.FirstOrDefault().UomUnit,
                    Barcode = item.FirstOrDefault().Barcode,
                    ProductionOrderDate = item.FirstOrDefault().ProductionOrderDate,

                    //Quantity = 0,
                    //QuantityPiece = 0,
                    Quantity = item.FirstOrDefault(d => d.Type == "OUT") != null ? item.FirstOrDefault(d => d.Type == "IN").Qty - item.FirstOrDefault(d => d.Type == "OUT").Qty :

                                item.FirstOrDefault(d => d.Type == "OUT") == null ? item.FirstOrDefault(d => d.Type == "IN").Qty : 0,
                    QuantityPiece = item.FirstOrDefault(d => d.Type == "OUT") != null ? item.FirstOrDefault(d => d.Type == "IN").QtyPiece - item.FirstOrDefault(d => d.Type == "OUT").QtyPiece :
                                item.FirstOrDefault(d => d.Type == "OUT") == null ? item.FirstOrDefault(d => d.Type == "IN").QtyPiece : 0
                    //Quantity = item.FirstOrDefault().QtyOut== null ? item.FirstOrDefault().QtyIn : item.FirstOrDefault().QtyIn - item.FirstOrDefault().QtyOut
                }).Take(75).Where( x => x.Quantity >0 && x.QuantityPiece > 0).ToList()
            });

            //var data = query.GroupBy(s => new { s.Construction }).Select(s => new InventoryWeavingItemDetailViewModel()
            //{
            //    Construction = s.Key.Construction,
            //    ListItems = s.GroupBy(x => new { x.Grade, x.Piece}).Select(item => new ItemListDetailViewModel()
            //    {
            //        Grade = item.Key.Grade,
            //        Piece = item.Key.Piece,
            //        //Quantity = item.First().Quantity,
            //        Quantity = (item.FirstOrDefault().Type == "OUT") == null ? item.FirstOrDefault(d => d.Type == "IN").Quantity
            //                    : 
            //                    item.FirstOrDefault(d => d.Type == "OUT").Quantity

            //        //Quantity = (item.FirstOrDefault().Type == "OUT") == null ? item.FirstOrDefault(d => d.Type == "IN").Quantity 
            //        //            : item.FirstOrDefault(d => d.Type == "IN").Quantity -
            //        //            item.FirstOrDefault(d => d.Type == "OUT").Quantity

            //    }).ToList()

            //});

            return result.ToList();
        }

        public async Task<InventoryWeavingDocument> MapToModel(InventoryWeavingDocumentOutViewModel data)
        {
            List<InventoryWeavingDocumentItem> DocsItems = new List<InventoryWeavingDocumentItem>();
            foreach (var i in data.items) {
                i.ListItems = i.ListItems.Where(s => s.IsSave).ToList();
                foreach (var d in i.ListItems) {
                    DocsItems.Add( new InventoryWeavingDocumentItem
                    {
                        Id = d.Id,
                        Active = d.Active,
                        _CreatedBy = d._CreatedBy,
                        _CreatedUtc = d._CreatedUtc,
                        _CreatedAgent = d._CreatedAgent,
                        _LastModifiedBy = d._LastModifiedBy,
                        _LastModifiedUtc = d._LastModifiedUtc,
                        _LastModifiedAgent = d._LastModifiedAgent,
                        _IsDeleted = d._IsDeleted,
                        ProductOrderName = i.ProductOrderNo,
                        ReferenceNo = i.ReferenceNo,
                        Construction = i.Construction,

                        Grade = d.Grade,
                        //Piece = d.Piece == "BESAR" ? "1" : d.Piece == "KECIL"? "2" : "3",
                        MaterialName = d.MaterialName,
                        WovenType = d.WovenType,
                        Width = d.Width,
                        Yarn1 = d.Yarn1,
                        Yarn2 = d.Yarn2,
                        YarnType1 = d.YarnType1,
                        YarnType2 = d.YarnType2,
                        YarnOrigin1 = d.YarnOrigin1,
                        YarnOrigin2 = d.YarnOrigin2,

                        UomId = 35,
                        UomUnit = "MTR",
                        Quantity = d.Qty,
                        QuantityPiece = d.QtyPiece,
                        ProductRemark = d.ProductRemark,
                        Barcode = d.Barcode,
                        ProductionOrderDate = d.ProductionOrderDate,
                        //InventoryWeavingDocumentId = d.InventoryWeavingDocumentId
                    });


                }
            }

            InventoryWeavingDocument model = new InventoryWeavingDocument
            {
                BonNo = data.bonNo,
                BonType = data.bonType,
                Date = data.date,
                StorageId = 105,
                StorageCode = "DNWDX2GZ",
                StorageName = "WEAVING 2 (EX. WEAVING 3) / WEAVING",
                Remark = data.remark,
                Type = "OUT",
                Items = DocsItems
               
            };
            return model;

        }


        public async Task Create(InventoryWeavingDocument model)
        {
            //var bonCheck = this.DbSetDoc.FirstOrDefault(s => s.Date.Date == model.Date.Date && s.BonType == model.BonType && s.Type == "OUT");

           // var bonCheck = this.DbContext.InventoryWeavingDocuments.Where(s => s.Date.Date == model.Date.Date && s.BonType == model.BonType && s.Type == "OUT").FirstOrDefault();

           // if (bonCheck == null)
            //{
                model.BonNo = GenerateBon(model.BonType, model.Date);
                model.FlagForCreate(IdentityService.Username, UserAgent);
                model.FlagForUpdate(IdentityService.Username, UserAgent);

                foreach (var item in model.Items)
                {
                    item.FlagForCreate(IdentityService.Username, UserAgent);
                    item.FlagForUpdate(IdentityService.Username, UserAgent);
                }

                DbSetDoc.Add(model);

                var result = await DbContext.SaveChangesAsync();

                foreach (var item in model.Items)
                {
                    InventoryWeavingMovement movement = new InventoryWeavingMovement
                    {
                        ProductOrderName = item.ProductOrderName,
                        BonNo = model.BonNo,
                        ReferenceNo = item.ReferenceNo,
                        Construction = item.Construction,
                        Grade = item.Grade,
                       //Piece = item.Piece,
                        MaterialName = item.MaterialName,
                        WovenType = item.WovenType,
                        Width = item.Width,
                        Yarn1 = item.Yarn1,
                        Yarn2 = item.Yarn2,
                        YarnType1 = item.YarnType1,
                        YarnType2 = item.YarnType2,
                        YarnOrigin1 = item.YarnOrigin1,
                        YarnOrigin2 = item.YarnOrigin2,
                        UomId = item.UomId,
                        UomUnit = item.UomUnit,
                        Quantity = item.Quantity,
                        QuantityPiece = item.QuantityPiece,
                        ProductRemark = item.ProductRemark,
                        Type = model.Type,
                        InventoryWeavingDocumentId = model.Id,
                        InventoryWeavingDocumentItemId = item.Id,
                        Barcode = item.Barcode,
                        ProductionOrderDate = item.ProductionOrderDate,

                    };

                    movement.FlagForCreate(IdentityService.Username, UserAgent);
                    movement.FlagForUpdate(IdentityService.Username, UserAgent);
                    DbSetMovement.Add(movement);
                }

                var result2 = await DbContext.SaveChangesAsync();

            //}
            //else
            //{



            //    foreach (var i in model.Items)
            //    {
            //        InventoryWeavingDocumentItem DocItem = new InventoryWeavingDocumentItem()
            //        {
            //            ProductOrderName = i.ProductOrderName,
            //            ReferenceNo = i.ReferenceNo,
            //            Construction = i.Construction,

            //            Grade = i.Grade,
            //            Piece = i.Piece,
            //            MaterialName = i.MaterialName,
            //            WovenType = i.WovenType,
            //            Width = i.Width,
            //            Yarn1 = i.Yarn1,
            //            Yarn2 = i.Yarn2,
            //            YarnType1 = i.YarnType1,
            //            YarnType2 = i.YarnType2,
            //            YarnOrigin1 = i.YarnOrigin1,
            //            YarnOrigin2 = i.YarnOrigin2,

            //            UomId = 35,
            //            UomUnit = "MTR",
            //            Quantity = i.Quantity,
            //            QuantityPiece = i.QuantityPiece,
            //            ProductRemark = i.ProductRemark,
            //            InventoryWeavingDocumentId = bonCheck.Id

            //        };

            //        DocItem.FlagForCreate(IdentityService.Username, UserAgent);
            //        DocItem.FlagForUpdate(IdentityService.Username, UserAgent);
            //        DbSetItem.Add(DocItem);
            //    }
            //    var result = await DbContext.SaveChangesAsync();
            //    //DbSetItem.Add(model.Items);

            //    foreach (var item in model.Items)
            //    {
            //        InventoryWeavingMovement movement = new InventoryWeavingMovement
            //        {
            //            ProductOrderName = item.ProductOrderName,
            //            BonNo = model.BonNo,
            //            ReferenceNo = item.ReferenceNo,
            //            Construction = item.Construction,
            //            Grade = item.Grade,
            //            Piece = item.Piece,
            //            MaterialName = item.MaterialName,
            //            WovenType = item.WovenType,
            //            Width = item.Width,
            //            Yarn1 = item.Yarn1,
            //            Yarn2 = item.Yarn2,
            //            YarnType1 = item.YarnType1,
            //            YarnType2 = item.YarnType2,
            //            YarnOrigin1 = item.YarnOrigin1,
            //            YarnOrigin2 = item.YarnOrigin2,
            //            UomId = item.UomId,
            //            UomUnit = item.UomUnit,
            //            Quantity = item.Quantity,
            //            QuantityPiece = item.QuantityPiece,
            //            ProductRemark = item.ProductRemark,
            //            Type = model.Type,
            //            InventoryWeavingDocumentId = bonCheck.Id,
            //            InventoryWeavingDocumentItemId = item.Id
            //        };

            //        movement.FlagForCreate(IdentityService.Username, UserAgent);
            //        movement.FlagForUpdate(IdentityService.Username, UserAgent);
            //        DbSetMovement.Add(movement);
            //    }

            //    var result2 = await DbContext.SaveChangesAsync();
            //}


        }

        private string GenerateBon(string from, DateTimeOffset date)
        {
            
            
            var type = from == "PACKING" ? "PC" : from == "FINISHING" ? "FN" : from == "PRINTING" ? "PR" 
                      :from == "PRODUKSI" ? "PR"  : from == "KOTOR" ? "KR" : from == "INSPECTING WEAVING" ? "IW" : "LL";

            var totalData = DbSetDoc.Count(s => s.BonNo.Substring(3,2) == type && s._CreatedUtc.Year == date.Date.Year)+1;

           
            return string.Format("{0}.{1}.{2}.{3}","GD", type, date.ToString("yy"), totalData.ToString().PadLeft(4, '0'));
            
        }

        public InventoryWeavingDocumentDetailViewModel ReadById(int id)
        {
            var data = this.DbSetDoc.Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                 .Include(p => p.Items).FirstOrDefault();


            if (data == null)
                return null;

            InventoryWeavingDocumentDetailViewModel DocsItems = MapToViewModelById(data);


            return DocsItems;
        }

        private InventoryWeavingDocumentDetailViewModel MapToViewModelById(InventoryWeavingDocument model)
        {
            var vm = new InventoryWeavingDocumentDetailViewModel()
            {
                Active = model.Active,
                Id = model.Id,
                _CreatedAgent = model._CreatedAgent,
                _CreatedBy = model._CreatedBy,
                _CreatedUtc = model._CreatedUtc,
                _IsDeleted = model._IsDeleted,
                _LastModifiedAgent = model._LastModifiedAgent,
                _LastModifiedBy = model._LastModifiedBy,
                _LastModifiedUtc = model._LastModifiedUtc,
                BonNo = model.BonNo,
                Date = model.Date,
                BonType = model.BonType,
                StorageId = model.StorageId,
                StorageCode = model.StorageCode,
                StorageName = model.StorageName,
                Type = model.Type,

                Detail = model.Items.GroupBy(x => x.ReferenceNo).Select(item => new InventoryWeavingItemDetailViewModel()
                {
                
                    //Construction = item.First().Construction,
                    ReferenceNo = item.First().ReferenceNo,
                  

                    ListItems = item.Where( s=> s._IsDeleted.Equals(false)).Select(s => new ItemListDetailViewModel()
                    {
                        Active = s.Active,
                        _CreatedAgent = s._CreatedAgent,
                        _CreatedBy = s._CreatedBy,
                        _CreatedUtc = s._CreatedUtc,

                        Id = s.Id,
                        _IsDeleted = s._IsDeleted,
                        _LastModifiedAgent = s._LastModifiedAgent,
                        _LastModifiedBy = s._LastModifiedBy,
                        _LastModifiedUtc = s._LastModifiedUtc,
                        Construction = s.Construction,
                        Grade = s.Grade,
                        //Piece = s.Piece == "1" ? "BESAR" : s.Piece == "2"? "KECIL": "POTONGAN",
                        MaterialName = s.MaterialName,
                        WovenType = s.WovenType,
                        Yarn1 = s.Yarn1,
                        Yarn2 = s.Yarn2,
                        YarnType1 = s.YarnType1,
                        YarnType2 = s.YarnType2,
                        YarnOrigin1 = s.YarnOrigin1,
                        YarnOrigin2 = s.YarnOrigin2,
                        YarnOrigin = s.YarnOrigin1 + " / " + s.YarnOrigin2,
                        Width = s.Width,
                        UomUnit = s.UomUnit,
                        Quantity = s.Quantity,
                        QuantityPiece = s.QuantityPiece,
                        ProductRemark = s.ProductRemark,
                        Barcode = s.Barcode,
                        ProductionOrderDate = s.ProductionOrderDate,
                    }).ToList()
                }).ToList()
            };
            return vm;
        }
       

        public MemoryStream DownloadCSVOut( DateTime dateFrom, DateTime dateTo, int offset, string bonType)
        {
            var data = GetQuery(dateFrom, dateTo, offset, bonType);
            
            using (MemoryStream stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream))
                {

                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        //foreach (var item in CsvHeader)
                        //{
                        //    csvWriter.WriteField(item);
                        //}
                        //csvWriter.NextRecord();

                        //InventoryWeavingDocumentCsvOutViewModel result = new InventoryWeavingDocumentCsvOutViewModel();


                       
                       
                        csvWriter.WriteRecords(data);
                        
                        
                        
                    }
                }
                return stream;
            }
        }

        /* Download Csv */
        public List<string> CsvHeader { get; } = new List<string>()
        {
           // "BonNo", "Benang", "Anyaman", "Lusi", "Pakan", "Lebar", "JL", "JP", "AL", "AP", "SP", "Grade", "Piece", "Qty", "QtyPiece"

            "BonNo","Tanggal","Benang","Anyaman","Lusi","Pakan","Lebar","JL","JP","AL","AP","Grade","Piece","Qty","QtyPiece","Barcode","ProductionOrderDate"
        };

        public IQueryable<InventoryWeavingDocumentCsvOutViewModel> GetQuery(DateTime? dateFrom, DateTime? dateTo, int offset, string bonType)
        {
            //int offset = 7;
            DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
            DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;
            var query = (from a in DbSetDoc 
                        join b in DbSetItem on a.Id equals b.InventoryWeavingDocumentId
                        where a._IsDeleted == false
                            && b._IsDeleted == false
                            && a.BonType == (string.IsNullOrWhiteSpace(bonType) ? a.BonType : bonType)
                            && a.Type == "OUT"
                              && a.Date.ToOffset(new TimeSpan(offset, 0, 0)).Date >= DateFrom.Date
                             && a.Date.ToOffset(new TimeSpan(offset, 0, 0)).Date <= DateTo.Date
                        orderby a.Date, a._CreatedUtc ascending
                        select new InventoryWeavingDocumentCsvOutViewModel
                        {
                            BonNo = a.BonNo,
                            Tanggal = a.Date.ToString("MM/dd/yyyy"),
                            //Construction = b.Construction,
                            Benang = b.MaterialName.Trim(),
                            Anyaman = b.WovenType.Trim(),
                            
                            Lusi = b.Yarn1.Trim(),
                            Pakan = b.Yarn2.Trim(),
                            Lebar = b.Width.Trim(),
                            JP = b.YarnType1.Trim(),
                            JL = b.YarnType2.Trim(),
                            
                            AL = b.YarnOrigin1.Trim(),
                            AP = b.YarnOrigin2.Trim(),
                            Grade = b.Grade.Trim(),
                            //Piece = b.Piece,
                            Qty = b.Quantity,
                            QtyPiece = b.QuantityPiece,
                            Barcode = b.Barcode,
                            ProductionOrderDate = b.ProductionOrderDate,
                        });


            return query;

        }

        public IQueryable<InventoryWeavingOutReportViewModel> GetQueryReport(string bonType, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offset)
        {
            DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
            DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;

            var query1 = (from a in DbContext.InventoryWeavingDocuments
                         join b in DbContext.InventoryWeavingDocumentItems on a.Id equals b.InventoryWeavingDocumentId
                         where a._IsDeleted == false
                            && b._IsDeleted == false
                            && a.BonType == (string.IsNullOrWhiteSpace(bonType) ? a.BonType : bonType)
                            && a.Type == "OUT"
                              && a.Date.AddHours(offset).Date >= DateFrom.Date
                             && a.Date.AddHours(offset).Date <= DateTo.Date
                         orderby a.Date, a._CreatedUtc ascending
                         select new InventoryWeavingOutReportViewModel
                         {
                             Id = a.Id,
                             Date = a.Date,
                             BonNo = a.BonNo,
                             Construction = b.Construction,
                             Grade = b.Grade,
                             //Piece = b.Piece,
                             Quantity = b.Quantity,
                             QuantityPiece = b.QuantityPiece,
                             Remark = b.ProductRemark,
                             Barcode = b.Barcode,
                             ProductionOrderDate = b.ProductionOrderDate,
                         })
                         ;

            var queryGroup = query1.GroupBy(x => new { x.BonNo }).Select(s => new InventoryWeavingOutReportViewModel {
                Id = s.FirstOrDefault().Id,
                BonNo = s.Key.BonNo,
                QuantityTot = s.Sum( x=> x.Quantity),
                QuantityPieceTot = s.Sum( x=> x.QuantityPiece),

            });

            var query = ( from a in query1
                          join b in queryGroup on a.Id equals b.Id
                          select new InventoryWeavingOutReportViewModel {
                              Id = a.Id,
                              Date = a.Date,
                              BonNo = a.BonNo,
                              Construction = a.Construction,
                              Grade = a.Grade,
                              //Piece = a.Piece,
                              Quantity = a.Quantity,
                              QuantityPiece = a.QuantityPiece,
                              QuantityTot = b.QuantityTot,
                              QuantityPieceTot = b.QuantityPieceTot,
                              Remark = a.Remark,
                              Barcode = a.Barcode,
                              ProductionOrderDate = a.ProductionOrderDate,
                          }


                );
            return query.AsQueryable();
        }

        public Tuple<List<InventoryWeavingOutReportViewModel>, int> GetReport(string bonType, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetQueryReport(bonType,  dateFrom, dateTo, offset);
            //Query.OrderBy(x => x.BillNo).ThenBy(x => x.CorDate).ThenBy(x => x.CorrNo).ThenBy(x => x.CorrType).ThenBy(x => x.CurrCode).ThenBy(x => x.DoDate).ThenBy(x => x.DoNo).ThenBy(x => x.IncomeTaxDate).ThenBy(x => x.INDate).ThenBy(x => x.INNo).ThenBy(x => x.InvoDate).ThenBy(x => x.InvoiceNo).ThenBy(x => x.NPH).ThenBy(x => x.NPN).ThenBy(x => x.PayBill).ThenBy(x => x.PaymentDueDate).ThenBy(x => x.PaymentMethod).ThenBy(x => x.PriceTot).ThenBy(x => x.SuppCode).ThenBy(x => x.SuppName).ThenBy(x => x.VatDate);
            //Query.OrderBy(x =>x.InvoiceNo).ThenBy(x => x.BillNo).ThenBy(x => x.CorDate).ThenBy(x => x.CorrNo).ThenBy(x => x.CorrType).ThenBy(x => x.CurrCode).ThenBy(x => x.DoDate).ThenBy(x => x.DoNo).ThenBy(x => x.IncomeTaxDate).ThenBy(x => x.INDate).ThenBy(x => x.INNo).ThenBy(x => x.InvoDate).ThenBy(x => x.NPH).ThenBy(x => x.NPN).ThenBy(x => x.PayBill).ThenBy(x => x.PaymentDueDate).ThenBy(x => x.PaymentMethod).ThenBy(x => x.PriceTot).ThenBy(x => x.SuppCode).ThenBy(x => x.SuppName).ThenBy(x => x.VatDate);

            //Console.WriteLine(Query);
            var b = Query.ToArray();
            var q = Query.ToList();
            var index = 0;
            foreach (InventoryWeavingOutReportViewModel a in q)
            {
                InventoryWeavingOutReportViewModel dup = Array.Find(b, o => o.Id == a.Id && o.BonNo == a.BonNo );
                if (dup != null)
                {
                    if (dup.Number == 0)
                    {
                        index++;
                        dup.Number = index;
                    }
                }
                a.Number = dup.Number;
            }

            Query = q.AsQueryable();
            Pageable<InventoryWeavingOutReportViewModel> pageable = new Pageable<InventoryWeavingOutReportViewModel>(Query, page - 1, size);
            List<InventoryWeavingOutReportViewModel> Data = pageable.Data.ToList<InventoryWeavingOutReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }


        public MemoryStream GenerateExcelReceiptReport(string bonType, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offset)
        {
            var Query = GetQueryReport(bonType, dateFrom, dateTo, offset);


            var r = Query.ToArray();
            var q = Query.ToList();
            var index1 = 0;
            foreach (InventoryWeavingOutReportViewModel f in q)
            {
                InventoryWeavingOutReportViewModel dup = Array.Find(r, o => o.Id == f.Id && o.BonNo == f.BonNo);
                if (dup != null)
                {
                    if (dup.Number == 0)
                    {
                        index1++;
                        dup.Number = index1;
                    }
                }
                f.Number = dup.Number;
            }

            Query = q.AsQueryable();

            DataTable result = new DataTable();
            var headers = new string[] { "No", "Nomor Bon", "Konstruksi", "Grade", "Piece", "Jumlah Meter", "Jumlah Piece", "Total", "Total1", "Keterangan", "Barcode", "Tanggal Produksi" };
            var subheaders = new string[] { "Meter", "Piece" };
            for (int i = 0; i < 5; i++)
            {
                result.Columns.Add(new DataColumn() { ColumnName = headers[i], DataType = typeof(string) });
            }

            result.Columns.Add(new DataColumn() { ColumnName = headers[5], DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[6], DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[7], DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[8], DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[9], DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[10], DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[11], DataType = typeof(String) });

            var index = 1;
            double TotalQuantity = 0;
            double TotalQuantityPiece = 0;

            foreach (var item in Query)
            {
                TotalQuantity+= item.Quantity;
                TotalQuantityPiece += item.QuantityPiece;
                string tglProduksi = item.ProductionOrderDate == null ? "-" : item.ProductionOrderDate.ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                //result.Rows.Add(index++, item.ProductCode, item.RO, item.PlanPo, item.NoArticle, item.ProductName, item.Information, item.Buyer,

                //    item.BeginningBalanceQty, item.BeginningBalanceUom, item.ReceiptQty, item.ReceiptCorrectionQty, item.ReceiptUom,
                //    NumberFormat(item.ExpendQty),
                //    item.ExpandUom, item.EndingBalanceQty, item.EndingUom, item.From);


                result.Rows.Add(item.Number, item.BonNo, item.Construction, item.Grade, item.Piece, item.Quantity, item.QuantityPiece,

                    item.QuantityTot, item.QuantityPieceTot, item.Remark, "", "");

            }

            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Data");

            var col = (char)('A' + (result.Columns.Count - 1));
            //string tglawal = new DateTimeOffset(dateFrom.Value.Date).ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            //string tglakhir = new DateTimeOffset(dateTo.Value.Date).ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            sheet.Cells[$"A1:{col}1"].Value = string.Format("BON PENGANTAR GREY");
            sheet.Cells[$"A1:{col}1"].Merge = true;
            sheet.Cells[$"A1:{col}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A1:{col}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A1:{col}1"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Value = string.Format("FM.W.00.GG.15.003");
            sheet.Cells[$"A2:{col}2"].Merge = true;
            sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A3:{col}3"].Value = string.Format("Untuk Bagian : {0}", bonType);
            sheet.Cells[$"A3:{col}3"].Merge = true;
            sheet.Cells[$"A3:{col}3"].Style.Font.Bold = true;
            sheet.Cells[$"A3:{col}3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A3:{col}3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


            sheet.Cells["A7"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light16);
            sheet.Cells["H5"].Value = headers[7];
            sheet.Cells["H5:I5"].Merge = true;

            Dictionary<string, int> counts = new Dictionary<string, int>();
            // var docNo = Query.ToArray();
            int value;
            foreach (var c in Query)
            {
                
                if (counts.TryGetValue(c.BonNo, out value))
                {
                    counts[c.BonNo]++;
                }
                else
                {
                    counts[c.BonNo] = 1;
                }

               
            }

            int num = 7;
            foreach (KeyValuePair<string, int> b in counts)
            {

                sheet.Cells["A" + num + ":A" + (num + b.Value - 1)].Merge = true;
                sheet.Cells["A" + num + ":A" + (num + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                sheet.Cells["B" + num + ":B" + (num + b.Value - 1)].Merge = true;
                sheet.Cells["B" + num + ":B" + (num + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                sheet.Cells["C" + num + ":C" + (num + b.Value - 1)].Merge = true;
                sheet.Cells["C" + num + ":C" + (num + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                sheet.Cells["H" + num + ":H" + (num + b.Value - 1)].Merge = true;
                sheet.Cells["H" + num + ":H" + (num + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                sheet.Cells["I" + num + ":I" + (num + b.Value - 1)].Merge = true;
                sheet.Cells["I" + num + ":I" + (num + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;


                num += b.Value;
            }

            //foreach (KeyValuePair<string, int> b in counts)
            //{

            //    sheet.Cells["B" + num + ":B" + (num + b.Value - 1)].Merge = true;
            //    sheet.Cells["B" + num + ":B" + (num + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
            //    num += b.Value;
            //}

            //foreach (KeyValuePair<string, int> b in counts)
            //{

            //    sheet.Cells["C" + num + ":C" + (num + b.Value - 1)].Merge = true;
            //    sheet.Cells["C" + num + ":C" + (num + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
            //    num += b.Value;
            //}

            //foreach (KeyValuePair<string, int> b in counts)
            //{

            //    sheet.Cells["H" + num + ":H" + (num + b.Value - 1)].Merge = true;
            //    sheet.Cells["H" + num + ":H" + (num + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
            //    num += b.Value;
            //}

            //foreach (KeyValuePair<string, int> b in counts)
            //{

            //    sheet.Cells["I" + num + ":I" + (num + b.Value - 1)].Merge = true;
            //    sheet.Cells["I" + num + ":I" + (num + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
            //    num += b.Value;
            //}


            foreach (var i in Enumerable.Range(0, 7))
            {
                col = (char)('A' + i);
                sheet.Cells[$"{col}5"].Value = headers[i];
                sheet.Cells[$"{col}5:{col}6"].Merge = true;
            }

            for (var i = 0; i < 2; i++)
            {
                col = (char)('H' + i);
                sheet.Cells[$"{col}6"].Value = subheaders[i];

            }

            foreach (var i in Enumerable.Range(0, 1))
            {
                col = (char)('J' + i);
                sheet.Cells[$"{col}5"].Value = headers[i + 9];
                sheet.Cells[$"{col}5:{col}6"].Merge = true;
            }

            sheet.Cells["A5:J6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells["A5:J6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells["A5:J6"].Style.Font.Bold = true;
            var widths = new int[] { 10, 20, 30, 10, 20, 15, 15, 15, 15,20};
            foreach (var i in Enumerable.Range(0, headers.Length))
            {
                sheet.Column(i + 1).Width = widths[i];
            }

            sheet.Column(5).Hidden = true;

            var a = Query.Count();
            var date = a + 2;
            var dateNow = DateTime.Now.Date;
            var ttd = a + 3;
            var ttd1 = a + 4;

            //sheet.Cells[$"A{5}:J{5 + a + 3}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            sheet.Cells[$"A{4}:J{5 + a + 2}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"A{5}:K{5 + a + 2}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;


            sheet.Cells[$"A{7 + a}"].Value = "T O T A L  . . . . . . . . . . . . . . .";
            sheet.Cells[$"A{7 + a}:D{7 + a}"].Merge = true;
            sheet.Cells[$"A{7 + a}:D{7 + a}"].Style.Font.Bold = true;
            sheet.Cells[$"A{7 + a}:D{7 + a}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A{7 + a}:D{7 + a}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"F{7 + a}"].Value = TotalQuantity;
            sheet.Cells[$"G{7 + a}"].Value = TotalQuantityPiece;

            sheet.Cells[$"G{7 + date}"].Value = "Sukoharjo :" + dateNow.ToString("dd/MM/yyyy");
            sheet.Cells[$"G{7 + date}:H{7 + date}"].Merge = true;
            sheet.Cells[$"G{7 + date}:H{7 + date}"].Style.Font.Bold = true;
            sheet.Cells[$"G{7 + date}:H{7 + date}"].Style.HorizontalAlignment= ExcelHorizontalAlignment.Center;
            sheet.Cells[$"G{7 + date}:H{7 + date}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


            sheet.Cells[$"G{7 + ttd}"].Value = "Diserahkan Oleh : ";
            sheet.Cells[$"G{7 + ttd}:H{7 + ttd}"].Merge = true;
            sheet.Cells[$"G{7 + ttd}:H{7 + ttd}"].Style.Font.Bold = true;
            sheet.Cells[$"G{7 + ttd}:H{7 + ttd}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"G{7 + ttd}:H{7 + ttd}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            sheet.Cells[$"B{7 + ttd}"].Value = "Diterima Oleh : ";
            //sheet.Cells[$"B{7 + ttd}"].Merge = true;
            sheet.Cells[$"B{7 + ttd}"].Style.Font.Bold = true;
            sheet.Cells[$"B{7 + ttd}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"B{7 + ttd}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


            sheet.Cells[$"G{7 + ttd1}"].Value = "(....................................) ";
            sheet.Cells[$"G{7 + ttd1}:H{7 + ttd1 + 3}"].Merge = true;
            sheet.Cells[$"G{7 + ttd1}:H{7 + ttd1 + 3}"].Style.Font.Bold = true;
            sheet.Cells[$"G{7 + ttd1}:H{7 + ttd1 + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"G{7 + ttd1}:H{7 + ttd1 + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;


            sheet.Cells[$"B{7 + ttd1}"].Value = "(...........................) ";
            sheet.Cells[$"B{7 + ttd1}:B{7 + ttd1 + 3}"].Merge = true;
            sheet.Cells[$"B{7 + ttd1}:B{7 + ttd1 + 3}"].Style.Font.Bold = true;
            sheet.Cells[$"B{7 + ttd1}:B{7 + ttd1 + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"B{7 + ttd1}:B{7 + ttd1 + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

            //sheet.Cells[$"K{7 + a}"].Value = CorrQtyTotal;
            //sheet.Cells[$"M{7 + a}"].Value = ExpendQtyTotal;
            //sheet.Cells[$"O{7 + a}"].Value = EndingQtyTotal;

            //sheet.Cells[$"K1338"].Value = ReceiptQtyTotal;


            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;


        }

        /* Upload Csv */

        public sealed class WeavingInventoryOutMap : CsvHelper.Configuration.ClassMap<InventoryWeavingUploadCsvOutViewModel>
        {
            public WeavingInventoryOutMap()
            {
                Map(p => p.ReferenceNo).Index(0);
                Map(p => p.DestinationArea).Index(1);
                Map(p => p.MaterialName).Index(2);
                Map(p => p.WovenType).Index(3);
                Map(p => p.Yarn1).Index(4);
                Map(p => p.Yarn2).Index(5);
                Map(p => p.Width).Index(6);
                Map(p => p.YarnType1).Index(7);
                Map(p => p.YarnType2).Index(8);
                Map(p => p.YarnOrigin1).Index(9);
                Map(p => p.YarnOrigin2).Index(10);
                Map(p => p.ProductionOrderNo).Index(11);
                Map(p => p.Grade).Index(12);
                Map(p => p.Piece).Index(13).TypeConverter<StringConverter>();
                Map(p => p.QtyPiece).Index(14).TypeConverter<StringConverter>();
                Map(p => p.Qty).Index(15).TypeConverter<StringConverter>();
                //Map(p => p.Barcode).Index(14);
                //Map(p => p.ProductionOrderDate).Index(15);
            }
        }

        public List<string> CsvHeaderUpload { get; } = new List<string>()
        {  
            
            //With Barcode 
           //"Konstruksi","Benang","Anyaman","Lusi","Pakan","Lebar","JL","JP","AL","AP","Grade","Piece","Qty","QtyPiece","Barcode","ProductionOrderDate"


           //without Barcode
           "nota","nm_tujuan","benang","type","lusi","pakan","lebar","jlusi","jpakan","alusi","apakan","sp","grade","jenis","piece","meter"

            //"BonNo","Tanggal","Benang","Anyaman","Lusi","Pakan","Lebar","JL","JP","AL","AP","Grade","Piece","Qty","QtyPiece","Barcode","ProductionOrderDate"
        };

        public Tuple<bool, List<object>> UploadValidate (ref List<InventoryWeavingUploadCsvOutViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;

            foreach (InventoryWeavingUploadCsvOutViewModel productVM in Data)
            {
                ErrorMessage = "";


                if (string.IsNullOrWhiteSpace(productVM.ReferenceNo))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "NOTA TIDAK BOLEH KOSONG, ");
                }
                if (string.IsNullOrWhiteSpace(productVM.DestinationArea))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "TUJUAN TIDAK BOLEH KOSONG, ");

                }

                if (string.IsNullOrWhiteSpace(productVM.MaterialName))
                {

                    ErrorMessage = string.Concat(ErrorMessage, "BENANG TIDAK BOLEH KOSONG, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.WovenType))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "ANYAMAN TIDAK BOLEH KOSONG, ");

                }

                if (string.IsNullOrWhiteSpace(productVM.Yarn1))
                {

                    ErrorMessage = string.Concat(ErrorMessage, "LUSI TIDAK BOLEH KOSONG, ");

                }
                
                if (string.IsNullOrWhiteSpace(productVM.Yarn2))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "PAKAN TIDAK BOLEH KOSONG, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.Width))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "LEBAR TIDAK BOLEH KOSONG, ");

                }

                if (string.IsNullOrWhiteSpace(productVM.YarnType1))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "JENIS LUSI TIDAK BOLEH KOSONG, ");

                }

                if (string.IsNullOrWhiteSpace(productVM.YarnType2))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "JENIS PAKAN TIDAK BOLEH KOSONG, ");

                }

                if (string.IsNullOrWhiteSpace(productVM.YarnOrigin1))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "ASAL LUSI TIDAK BOLEH KOSONG, ");

                }

                if (string.IsNullOrWhiteSpace(productVM.YarnOrigin2))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "ASAL PAKAN TIDAK BOLEH KOSONG, ");

                }

                if (string.IsNullOrWhiteSpace(productVM.Grade))
                {

                    ErrorMessage = string.Concat(ErrorMessage, "GRADE TIDAK BOLEH KOSONG ");
                }

                if (string.IsNullOrWhiteSpace(productVM.Piece))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "JENIS PIECE TIDAK BOLEH KOSONG ");

                }

                double Qty = 0;
                if (string.IsNullOrWhiteSpace(productVM.Qty))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Qty tidak boleh kosong, ");
                }
                else if (!double.TryParse(productVM.Qty, out Qty))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Qty harus numerik, ");
                }
                else if (Qty < 0)
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Qty harus lebih besar dari 0, ");
                }
                else
                {
                    string[] QtySplit = productVM.Qty.Split('.');
                    if (QtySplit.Count().Equals(2) && QtySplit[1].Length > 2)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Qty maksimal memiliki 2 digit dibelakang koma, ");
                    }
                }

                double QtyPiece = 0;
                if (string.IsNullOrWhiteSpace(productVM.QtyPiece))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Quantity Piece tidak boleh kosong, ");
                }
                else if (!double.TryParse(productVM.QtyPiece, out QtyPiece))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Quantity Piece numerik, ");
                }
                else if (QtyPiece < 0)
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Quantity Piece harus lebih besar dari 0, ");
                }

                //if (string.IsNullOrWhiteSpace(productVM.Barcode))
                //{
                //    ErrorMessage = string.Concat(ErrorMessage, "Barcode tidak boleh kosong");
                //}

                
                if (string.IsNullOrEmpty(ErrorMessage))
                {

                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    productVM.Qty = Qty;
                    productVM.QtyPiece = QtyPiece;

                }
                else
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Nota", productVM.BonNo);
                    Error.Add("Tujuan", productVM.DestinationArea);
                    //Error.Add("Konstruksi", productVM.Construction);
                    Error.Add("Benang", productVM.MaterialName);
                    Error.Add("Anyaman", productVM.WovenType);
                    Error.Add("Lusi", productVM.Yarn1);
                    Error.Add("Pakan", productVM.Yarn2);
                    Error.Add("Lebar", productVM.Width);

                    Error.Add("JL", productVM.YarnType1);
                    Error.Add("JP", productVM.YarnType2);
                    Error.Add("AL", productVM.YarnOrigin1);
                    Error.Add("AP", productVM.YarnOrigin2);
                    Error.Add("SP", productVM.ProductionOrderNo);
                    Error.Add("Grade", productVM.Grade);
                    Error.Add("Jenis", productVM.Piece);
                    Error.Add("Jumlah Piece", productVM.QtyPiece);
                    Error.Add("Jumlah Meter", productVM.Qty);


                    ErrorList.Add(Error);
                }
            }
            if (ErrorList.Count > 0)
            {
                Valid = false;
            }

            return Tuple.Create(Valid, ErrorList);
        }

        public async Task UploadData(InventoryWeavingDocument data, string username)
        {
            foreach (var i in data.Items)
            {
                i.FlagForCreate(IdentityService.Username, UserAgent);
                i.FlagForUpdate(IdentityService.Username, UserAgent);
            }
            data.FlagForCreate(IdentityService.Username, UserAgent);
            data.FlagForUpdate(IdentityService.Username, UserAgent);
            DbSetDoc.Add(data);

            var result = await DbContext.SaveChangesAsync();
            foreach (var item in data.Items)
            {
                InventoryWeavingMovement movementModel = new InventoryWeavingMovement
                {
                    ProductOrderName = item.ProductOrderName,
                    BonNo = data.BonNo,
                    ReferenceNo = item.ReferenceNo,
                    Construction = item.Construction,
                    Grade = item.Grade,
                    MaterialName = item.MaterialName,
                    WovenType = item.WovenType,
                    Width = item.Width,
                    Yarn1 = item.Yarn1,
                    Yarn2 = item.Yarn2,
                    YarnType1 = item.YarnType1,
                    YarnType2 = item.YarnType2,
                    YarnOrigin1 = item.YarnOrigin1,
                    YarnOrigin2 = item.YarnOrigin2,
                    UomId = item.UomId,
                    UomUnit = item.UomUnit,
                    Quantity = item.Quantity,
                    QuantityPiece = item.QuantityPiece,
                    ProductRemark = item.ProductRemark,
                    Type = data.Type,
                    InventoryWeavingDocumentId = data.Id,
                    InventoryWeavingDocumentItemId = item.Id,
                    Barcode = item.Barcode,
                    ProductionOrderDate = item.ProductionOrderDate,
                    DestinationArea = item.DestinationArea

                };

                movementModel.FlagForCreate(IdentityService.Username, UserAgent);
                movementModel.FlagForUpdate(IdentityService.Username, UserAgent);
                DbSetMovement.Add(movementModel);
            }
            var result2 = await DbContext.SaveChangesAsync();
        }
        

        public async Task<InventoryWeavingDocumentOutUploadViewModel> MapToViewModel(List<InventoryWeavingUploadCsvOutViewModel> csv, DateTimeOffset date)
        {
            List<InventoryWeavingDocumentOutItemViewModel> DocsOutItems = new List<InventoryWeavingDocumentOutItemViewModel>();
            
            //foreach (var d in csv)
            //{
                //foreach (var k in DocsOutItems)
                //{
                    //var query = k.ListItems.Where(s => s.IsSave).ToList();
       

                    foreach (var i in csv)
                    {

                        var MaterialName = i.MaterialName.Trim();
                        var WovenType = i.WovenType.Trim();

                        var Yarn1 = i.Yarn1.Trim();
                        var Yarn2 = i.Yarn2.Trim();
                        var Width = i.Width.Trim();
                        var YarnType1 = i.YarnType1.Trim();
                        var YarnType2 = i.YarnType2.Trim();
                        var YarnOrigin1 = i.YarnOrigin1.Trim();
                        var YarnOrigin2 = i.YarnOrigin2.Trim();

                        var constructonC = MaterialName + " " + WovenType + " " + Yarn1 + " " + Yarn2 + " " + Width + " "
                                           + YarnType1 + " " + YarnType2 + " " + YarnOrigin1 + " " + YarnOrigin2;

                        DocsOutItems.Add(new InventoryWeavingDocumentOutItemViewModel()
                        {
                            ProductOrderNo = i.ProductionOrderNo,
                            ReferenceNo = i.ReferenceNo,
                            Construction = constructonC,

                            Grade = i.Grade,
                            MaterialName = i.MaterialName,
                            WovenType = i.WovenType,
                            Yarn1 = i.Yarn1,
                            Yarn2 = i.Yarn2,
                            YarnType1 = i.YarnType1,
                            YarnType2 = i.YarnType2,
                            YarnOrigin1 = i.YarnOrigin1,
                            YarnOrigin2 = i.YarnOrigin2,
                            Width = i.Width,
                            UomUnit = "MTR",
                            UomId = 35,
                            Piece = i.Piece,
                            Quantity = Convert.ToDouble(i.Qty),
                            QuantityPiece = Convert.ToDouble(i.QtyPiece),
                            Barcode = null,
                            ProductionOrderDate = DateTime.MinValue,
                            DestinationArea = i.DestinationArea,
                            Type = "OUT"
                            

                        });
                    }
            //}
            //}

            InventoryWeavingDocumentOutUploadViewModel sPKDocView = new InventoryWeavingDocumentOutUploadViewModel
            {
                bonNo = GenerateBonUpload(date),
                date = date,
                bonType ="KELUAR GUDANG" ,

                storageId = 105,
                storageCode = "DNWDX2GZ",
                storageName = "WEAVING 2 (EX. WEAVING 3) / WEAVING",
                remark = "",
                type = "OUT",

                itemsOut = DocsOutItems,
            };

            return sPKDocView;
        } 
        
        public async Task<InventoryWeavingDocument> MapToModelUpload(InventoryWeavingDocumentOutUploadViewModel data)
        {
            InventoryWeavingDocument model = new InventoryWeavingDocument
            {
                BonNo = data.bonNo,
                BonType = data.bonType,
                Date = data.date,
                StorageId = data.storageId,
                StorageCode = data.storageCode,
                StorageName = data.storageName,
                Remark = data.remark,
                Type = data.type,
                Items = data.itemsOut.Select(item => new InventoryWeavingDocumentItem()
                {
                    Id = item.Id,
                    Active = item.Active,
                    _CreatedUtc = item._CreatedUtc,
                    _CreatedBy = item._CreatedBy,
                    _CreatedAgent = item._CreatedAgent,
                    _LastModifiedUtc = item._LastModifiedUtc,
                    _LastModifiedBy = item._LastModifiedBy,
                    _LastModifiedAgent = item._LastModifiedAgent,
                    _IsDeleted = item._IsDeleted,
                    ProductOrderName = item.ProductOrderNo,
                    ReferenceNo = item.ReferenceNo,
                    Construction = item.Construction,
                    Grade = item.Grade,
                    // Piece = item.piece,
                    MaterialName = item.MaterialName,
                    WovenType = item.WovenType,
                    Width = item.Width,
                    Yarn1 = item.Yarn1,
                    Yarn2 = item.Yarn2,
                    YarnType1 = item.YarnType1,
                    YarnType2 = item.YarnType2,
                    YarnOrigin1 = item.YarnOrigin1,
                    YarnOrigin2 = item.YarnOrigin2,

                    UomId = item.UomId,
                    UomUnit = item.UomUnit,
                    Quantity = item.Quantity,
                    QuantityPiece = item.QuantityPiece,
                    ProductRemark = item.ProductRemark,
                    InventoryWeavingDocumentId = item.InventoryWeavingDocumentId,
                    Barcode = item.Barcode,
                    ProductionOrderDate = item.ProductionOrderDate,
                    DestinationArea = item.DestinationArea,
                    Type = item.Type

                }).ToList()
            };
            return model;
        }


        private string GenerateBonUpload(DateTimeOffset date)
        {
            var totalData = DbSetDoc.Count(s => s._CreatedUtc.Year == date.Date.Year) + 1;
            return string.Format("{0}.{1}.{2}", "GD", date.ToString("yy"), totalData.ToString().PadLeft(4, '0'));
        }

        public int checkCsv(List<InventoryWeavingDocumentOutItemViewModel> data)
        {
            var index = 0;
            var query = data.GroupBy(s => new { s.MaterialName,
                                                s.WovenType,
                                                s.Yarn1,
                                                s.Yarn2,
                                                s.YarnType1,
                                                s.YarnType2,
                                                s.YarnOrigin1,
                                                s.YarnOrigin2,
                                                s.Grade

                                    }).Select(x => new { x.FirstOrDefault().Construction });

            //InventoryWeavingDocumentOutUploadViewModel check = new InventoryWeavingDocumentOutUploadViewModel();
            //List<InventoryWeavingDocumentOutItemViewModel> checkItem = new List<InventoryWeavingDocumentOutItemViewModel>();
            foreach (var i in query)
            {
                var result = this.DbSetMovement.Where(s => s.Construction.Equals(i.Construction) && s.Type == "IN").Count();


                if (result == 0)
                {
                    index++;
                }
            }

            return index;
        }
    }
}