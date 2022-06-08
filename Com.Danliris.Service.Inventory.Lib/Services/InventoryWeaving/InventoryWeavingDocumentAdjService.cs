using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using System.Linq;
using Newtonsoft.Json;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using System.Threading.Tasks;
using Com.Moonlay.Models;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving
{
    public class InventoryWeavingDocumentAdjService : IInventoryWeavingDocumentAdjService
    {
        private string USER_AGENT = "Service";
        private const string UserAgent = "inventory-service";
        protected DbSet<InventoryWeavingDocumentItem> DbSetItem;
        protected DbSet<InventoryWeavingMovement> DbSetMovement;
        protected DbSet<InventoryWeavingDocument> DbSetDoc;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;

        public InventoryWeavingDocumentAdjService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
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
            var query = this.DbSetDoc.Where(s => s.Type == "ADJ_OUT" || s.Type == "ADJ_IN");


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
                Qty = s.Sum(d => d.Quantity),
                QtyPiece = s.Sum(d => d.QuantityPiece),

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




            var result = data.GroupBy(s => new
            {
                s.MaterialName,
                s.WovenType,
                s.Width,
                s.Yarn1,
                s.Yarn2,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2
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
                    Yarn2 = item.FirstOrDefault().Yarn2,
                    YarnOrigin1 = item.FirstOrDefault().YarnOrigin1,
                    YarnOrigin2 = item.FirstOrDefault().YarnOrigin2,
                    YarnType1 = item.FirstOrDefault().YarnType1,
                    YarnType2 = item.FirstOrDefault().YarnType2,
                    UomUnit = item.FirstOrDefault().UomUnit,

                    //Quantity = 0,
                    //QuantityPiece = 0,
                    //Quantity = item.FirstOrDefault(d => d.Type == "OUT") != null ? item.FirstOrDefault(d => d.Type == "IN").Qty - item.FirstOrDefault(d => d.Type == "OUT").Qty :

                    //            item.FirstOrDefault(d => d.Type == "OUT") == null ? item.FirstOrDefault(d => d.Type == "IN").Qty : 0,
                    //QuantityPiece = item.FirstOrDefault(d => d.Type == "OUT") != null ? item.FirstOrDefault(d => d.Type == "IN").QtyPiece - item.FirstOrDefault(d => d.Type == "OUT").QtyPiece :
                    //            item.FirstOrDefault(d => d.Type == "OUT") == null ? item.FirstOrDefault(d => d.Type == "IN").QtyPiece : 0


                    Quantity = item.FirstOrDefault(d => d.Type == "OUT") != null && item.FirstOrDefault(d => d.Type == "ADJ_OUT") != null && item.FirstOrDefault(d => d.Type == "ADJ_IN") != null ?
                               (item.FirstOrDefault(d => d.Type == "IN").Qty + item.FirstOrDefault(d => d.Type == "ADJ_IN").Qty) -
                               (item.FirstOrDefault(d => d.Type == "OUT").Qty + item.FirstOrDefault(d => d.Type == "ADJ_OUT").Qty) : //full

                               item.FirstOrDefault(d => d.Type == "OUT") != null && item.FirstOrDefault(d => d.Type == "ADJ_OUT") == null && item.FirstOrDefault(d => d.Type == "ADJ_IN") != null ?
                               (item.FirstOrDefault(d => d.Type == "IN").Qty + item.FirstOrDefault(d => d.Type == "ADJ_IN").Qty) -
                               item.FirstOrDefault(d => d.Type == "OUT").Qty : // ADJ OUT null

                               item.FirstOrDefault(d => d.Type == "OUT") != null && item.FirstOrDefault(d => d.Type == "ADJ_OUT") != null && item.FirstOrDefault(d => d.Type == "ADJ_IN") == null ?
                               item.FirstOrDefault(d => d.Type == "IN").Qty -
                               (item.FirstOrDefault(d => d.Type == "OUT").Qty + item.FirstOrDefault(d => d.Type == "ADJ_OUT").Qty) : // ADJ_IN null

                               item.FirstOrDefault(d => d.Type == "OUT") != null && item.FirstOrDefault(d => d.Type == "ADJ_OUT") == null && item.FirstOrDefault(d => d.Type == "ADJ_IN") == null ?
                               item.FirstOrDefault(d => d.Type == "IN").Qty -
                               item.FirstOrDefault(d => d.Type == "OUT").Qty : //ADJ IN & ADJ OUT null

                               item.FirstOrDefault(d => d.Type == "OUT") == null && item.FirstOrDefault(d => d.Type == "ADJ_OUT") == null && item.FirstOrDefault(d => d.Type == "ADJ_IN") == null ?
                               item.FirstOrDefault(d => d.Type == "IN").Qty : 0, // ALL null)

                    //QuantityPiece = 1
                    QuantityPiece = item.FirstOrDefault(d => d.Type == "OUT") != null && item.FirstOrDefault(d => d.Type == "ADJ_OUT") != null && item.FirstOrDefault(d => d.Type == "ADJ_IN") != null ?
                                    (item.FirstOrDefault(d => d.Type == "IN").QtyPiece + item.FirstOrDefault(d => d.Type == "ADJ_IN").QtyPiece) -
                                    (item.FirstOrDefault(d => d.Type == "OUT").QtyPiece + item.FirstOrDefault(d => d.Type == "ADJ_OUT").QtyPiece) : //full

                                    item.FirstOrDefault(d => d.Type == "OUT") != null && item.FirstOrDefault(d => d.Type == "ADJ_OUT") == null && item.FirstOrDefault(d => d.Type == "ADJ_IN") != null ?
                                    (item.FirstOrDefault(d => d.Type == "IN").QtyPiece + item.FirstOrDefault(d => d.Type == "ADJ_IN").QtyPiece) -
                                    item.FirstOrDefault(d => d.Type == "OUT").QtyPiece : // ADJ OUT null

                                    item.FirstOrDefault(d => d.Type == "OUT") != null && item.FirstOrDefault(d => d.Type == "ADJ_OUT") != null && item.FirstOrDefault(d => d.Type == "ADJ_IN") == null ?
                                    item.FirstOrDefault(d => d.Type == "IN").QtyPiece -
                                    (item.FirstOrDefault(d => d.Type == "OUT").QtyPiece + item.FirstOrDefault(d => d.Type == "ADJ_OUT").QtyPiece) : // ADJ IN

                                    item.FirstOrDefault(d => d.Type == "OUT") != null && item.FirstOrDefault(d => d.Type == "ADJ_OUT") == null && item.FirstOrDefault(d => d.Type == "ADJ_IN") == null ?
                                    item.FirstOrDefault(d => d.Type == "IN").QtyPiece -
                                    item.FirstOrDefault(d => d.Type == "OUT").QtyPiece : //ADJ IN & ADJ OUT null

                                    item.FirstOrDefault(d => d.Type == "OUT") == null && item.FirstOrDefault(d => d.Type == "ADJ_OUT") == null && item.FirstOrDefault(d => d.Type == "ADJ_IN") == null ?
                                    item.FirstOrDefault(d => d.Type == "IN").QtyPiece : 0 // ALL null







                }).Take(75).Where(x => x.Quantity > 0 || x.QuantityPiece > 0).ToList()
            }).Where( x => x.ListItems.Count > 0 ); 

            return result.ToList();
        }

        public async Task<InventoryWeavingDocument> MapToModel(InventoryWeavingDocumentOutViewModel data)
        {
            List<InventoryWeavingDocumentItem> DocsItems = new List<InventoryWeavingDocumentItem>();
            foreach (var i in data.items)
            {
                i.ListItems = i.ListItems.Where(s => s.IsSave).ToList();
                foreach (var d in i.ListItems)
                {
                    DocsItems.Add(new InventoryWeavingDocumentItem
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
                Type = data.bonType == "ADJ MASUK" ? "ADJ_IN":"ADJ_OUT",
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
                    InventoryWeavingDocumentItemId = item.Id
                };

                movement.FlagForCreate(IdentityService.Username, UserAgent);
                movement.FlagForUpdate(IdentityService.Username, UserAgent);
                DbSetMovement.Add(movement);
            }

            var result2 = await DbContext.SaveChangesAsync();

        }

        private string GenerateBon(string from, DateTimeOffset date)
        {


            var type = from == "ADJ IN" ? "ADJ.IN" : "ADJ.OUT";

            var totalData = DbSetDoc.Count(s => s.BonNo.Substring(3, 2) == type && s._CreatedUtc.Year == date.Date.Year) + 1;


            return string.Format("{0}.{1}.{2}", type, date.ToString("yy"), totalData.ToString().PadLeft(4, '0'));

        }

    }
}
