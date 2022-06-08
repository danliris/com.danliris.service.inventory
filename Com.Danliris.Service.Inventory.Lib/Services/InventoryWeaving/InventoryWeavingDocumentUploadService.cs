using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using CsvHelper.TypeConversion;
using System.Threading.Tasks;
using Com.Moonlay.Models;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Newtonsoft.Json;
using Com.Moonlay.NetCore.Lib;
using System.IO;
using System.Data;
using System.Globalization;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving
{
    public class InventoryWeavingDocumentUploadService : IInventoryWeavingDocumentUploadService
    {
        private string USER_AGENT = "inventory-service";
        private const string UserAgent = "inventory-service";
        protected DbSet<InventoryWeavingDocument> DbSet;
        protected DbSet<InventoryWeavingMovement> DbSet2;
        protected DbSet<InventoryWeavingDocumentItem> DbSet3;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;
        private readonly IInventoryWeavingMovementService movementService;

        public InventoryWeavingDocumentUploadService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<InventoryWeavingDocument>();
            DbSet2 = dbContext.Set<InventoryWeavingMovement>();
            DbSet3 = dbContext.Set<InventoryWeavingDocumentItem>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
            movementService = (IInventoryWeavingMovementService)serviceProvider.GetService(typeof(IInventoryWeavingMovementService));
            //movementService = serviceProvider.GetService<IInventoryWeavingMovementService>();
        }


        public async Task<int> Create(InventoryWeavingDocument model)
        {


            int Created = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    model.FlagForCreate(IdentityService.Username, USER_AGENT);
                    model.FlagForUpdate(IdentityService.Username, USER_AGENT);
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

        public ReadResponse<InventoryWeavingDocument> Read(int page, int size, string order, string keyword, string filter)
        {
            IQueryable<InventoryWeavingDocument> Query = this.DbSet;


            List<string> SearchAttributes = new List<string>()
                {
                    "BonNo","Type"
                };
            Query = QueryHelper<InventoryWeavingDocument>.Search(Query, SearchAttributes, keyword);

            List<string> SelectedFields = new List<string>()
            {
                "Id", "date", "bonNo", "bonType",  "storageCode", "storageId", "storageName", "type", "_LastModifiedUtc"
            };

            Query = Query.Where(s => s.Type == "IN")
                .Select(s => new InventoryWeavingDocument
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

                }).OrderByDescending(x => x.Date);


            #region OrderBy

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            //Query = QueryHelper<InventoryWeavingDocument>.Order(Query, OrderDictionary);
            #endregion OrderBy

            #region Paging

            Pageable<InventoryWeavingDocument> pageable = new Pageable<InventoryWeavingDocument>(Query, page - 1, size);
            List<InventoryWeavingDocument> Data = pageable.Data.ToList<InventoryWeavingDocument>();
            int TotalData = pageable.TotalCount;

            #endregion Paging
            //Data = Data.OrderByDescending(x => x.Date).ToList();

            return new ReadResponse<InventoryWeavingDocument>(Data, TotalData, OrderDictionary, SelectedFields);
        }
        public sealed class InventoryWeavingDocumentMap : CsvHelper.Configuration.ClassMap<InventoryWeavingDocumentCsvViewModel>
        {
            public InventoryWeavingDocumentMap()
            {
                Map(p => p.ReferenceNo).Index(0);
                //Map(p => p.Date).Index(1);
                Map(p => p.MaterialName).Index(1);
                Map(p => p.WovenType).Index(2);
                Map(p => p.Yarn1).Index(3);
                Map(p => p.Yarn2).Index(4);
                Map(p => p.Width).Index(5);
                Map(p => p.YarnType1).Index(6);
                Map(p => p.YarnType2).Index(7);
                Map(p => p.YarnOrigin1).Index(8);
                Map(p => p.YarnOrigin2).Index(9);
                Map(p => p.ProductionOrderNo).Index(10);
                Map(p => p.Grade).Index(11);
                Map(p => p.Piece).Index(12).TypeConverter<StringConverter>();
                Map(p => p.QtyPiece).Index(13).TypeConverter<StringConverter>();
                Map(p => p.Qty).Index(14).TypeConverter<StringConverter>();
                //Map(p => p.Barcode).Index(15);
                //Map(p => p.ProductionOrderDate).Index(16);


            }
        }

        public List<string> CsvHeader { get; } = new List<string>()
        {
           // "BonNo", "Benang", "Anyaman", "Lusi", "Pakan", "Lebar", "JL", "JP", "AL", "AP", "SP", "Grade", "Piece", "Qty", "QtyPiece"


           // "nota","benang","type","lusi","pakan","lebar","jlusi","jpakan","alusi","apakan","sp","grade","jenis","piece","meter","barcode","tgl"
           "nota","benang","type","lusi","pakan","lebar","jlusi","jpakan","alusi","apakan","sp","grade","jenis","piece","meter"

        };

        public Tuple<bool, List<object>> UploadValidate(ref List<InventoryWeavingDocumentCsvViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;

            foreach (InventoryWeavingDocumentCsvViewModel productVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(productVM.ReferenceNo))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "No Bon Tidak Boleh Kosong tidak boleh kosong, ");
                }

                //if (productVM.Date == null)
                //{
                //    ErrorMessage = string.Concat(ErrorMessage, "No Bon Tidak Boleh Kosong tidak boleh kosong, ");
                //}

                if (string.IsNullOrWhiteSpace(productVM.ProductionOrderNo))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "SP tidak boleh kosong, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.Grade))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Grade tidak boleh kosong, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.MaterialName))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Benang tidak boleh kosong, ");
                }


                if (string.IsNullOrWhiteSpace(productVM.Width))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Lebar tidak boleh kosong, ");
                }


                if (string.IsNullOrWhiteSpace(productVM.Yarn1))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Lusi tidak boleh kosong, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.Yarn2))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Pakan tidak boleh kosong, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.YarnType1))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Jenis Lusi tidak boleh kosong, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.YarnType2))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Jenis Pakan tidak boleh kosong, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.YarnOrigin1))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Asal Lusi tidak boleh kosong, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.YarnOrigin2))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Asal Pakan tidak boleh kosong, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.Piece))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Piece tidak boleh kosong, ");
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


                //if (string.IsNullOrWhiteSpace(productVM.ProductionOrderDate.ToString()))
                //{
                  //  ErrorMessage = string.Concat(ErrorMessage, "Tanggal produksi order tidak boleh kosong");
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

                    Error.Add("BonNo", productVM.ReferenceNo);
                   // Error.Add("Tanggal", productVM.Date);
                    Error.Add("Benang", productVM.MaterialName);
                    Error.Add("Lusi", productVM.Yarn1);
                    Error.Add("Pakan", productVM.Yarn2);
                    Error.Add("Lebar", productVM.Width);
                    Error.Add("JL", productVM.YarnType1);
                    Error.Add("JP", productVM.YarnType2);
                    Error.Add("AL", productVM.YarnOrigin1);
                    Error.Add("AP", productVM.YarnOrigin2);
                    Error.Add("List Error", ErrorMessage);
                    




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

            //IInventoryWeavingMovementService movement = ServiceProvider.GetService<IInventoryWeavingMovementService>();
            foreach (var i in data.Items)
            {
                MoonlayEntityExtension.FlagForCreate(i, username, USER_AGENT);
                MoonlayEntityExtension.FlagForUpdate(i, username, USER_AGENT);
            }
            MoonlayEntityExtension.FlagForCreate(data, username, USER_AGENT);
            MoonlayEntityExtension.FlagForUpdate(data, username, USER_AGENT);
            DbSet.Add(data);

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
                    Type = data.Type,
                    InventoryWeavingDocumentId = data.Id,
                    InventoryWeavingDocumentItemId = item.Id,
                    Barcode = item.Barcode,
                    ProductionOrderDate = item.ProductionOrderDate,
                    DestinationArea = item.DestinationArea


                    //await BulkInsert(data, username);
                };
                MoonlayEntityExtension.FlagForCreate(movementModel, username, USER_AGENT);
                MoonlayEntityExtension.FlagForUpdate(movementModel, username, USER_AGENT);
                DbSet2.Add(movementModel);
            }
            var result2 = await DbContext.SaveChangesAsync();
        }


        public async Task<InventoryWeavingDocumentViewModel> MapToViewModel(List<InventoryWeavingDocumentCsvViewModel> csv, DateTimeOffset date, string from)
        //public async Task<InventoryWeavingDocumentViewModel> MapToViewModel(List<InventoryWeavingDocumentCsvViewModel> csv, string from)
        {
            List<InventoryWeavingDocumentItemViewModel> DocsItems = new List<InventoryWeavingDocumentItemViewModel>();
            //var itemx = GetItem();

            foreach (var i in csv)
            {

                //var MaterialName =  i.MaterialName.TrimEnd().Replace(" ", String.Empty);
                var MaterialName = i.MaterialName.Trim();
                var WovenType = i.WovenType.Trim();

                var Yarn1 = i.Yarn1.Trim();
                var Yarn2 = i.Yarn2.Trim();
                var Width = i.Width.Trim();
                var YarnType1 = i.YarnType1.Trim();
                var YarnType2 = i.YarnType2.Trim();
                var YarnOrigin1 = i.YarnOrigin1.Trim();
                var YarnOrigin2 = i.YarnOrigin2.Trim();

                var constructonC = MaterialName +" "+ WovenType + " " + Yarn1 + " " + Yarn2 + " " + Width + " "
                                   + YarnType1 + " " + YarnType2 + " " + YarnOrigin1 + " " + YarnOrigin2;

                DocsItems.Add(new InventoryWeavingDocumentItemViewModel
                {
                    productOrderNo = i.ProductionOrderNo,
                    referenceNo = i.ReferenceNo,

                    construction = constructonC,
                    grade = i.Grade,

                   // piece = i.Piece,
                    materialName = MaterialName,
                    wovenType = WovenType,
                    width = Width,
                    yarn1 = Yarn1,
                    yarn2 = Yarn2,
                    yarnType1 = YarnType1,
                    yarnType2 = YarnType2,
                    yarnOrigin1 = YarnOrigin1,
                    yarnOrigin2 = YarnOrigin2,
                    uomId = 35,
                    uomUnit = "MTR",
                    quantity = Convert.ToDouble(i.Qty),
                    quantityPiece = Convert.ToDouble(i.QtyPiece),
                    barcode = null,
                    productionOrderDate = DateTime.MinValue,


                });


            }

            InventoryWeavingDocumentViewModel sPKDocsViews = new InventoryWeavingDocumentViewModel
            {
                bonNo = GenerateBon(from, date),
                date = date,
                bonType = from == "PRODUKSI" ? "PRODUKSI" : from == "RETUR PACKING" ? "PACKING"
                          : from == "RETUR FINISHING" ? "FINISHING" : from == "RETUR PRINTING" ? "PRINTING"
                          : from == "RECHEKING" ? "RECHEKING" : from == "DEVELOPMENT" ? "DEVELOPMENT":"LAIN-LAIN",
                storageId = 105,
                storageCode = "DNWDX2GZ",
                storageName = "WEAVING 2 (EX. WEAVING 3) / WEAVING",
                remark = "",
                type = "IN",

                items = DocsItems
            };

            return sPKDocsViews;
        }


        public async Task<InventoryWeavingDocument> MapToModel(InventoryWeavingDocumentViewModel data)
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
                Items = data.items.Select(item => new InventoryWeavingDocumentItem()
                {
                    Id = item.Id,
                    Active = item.Active,
                    _CreatedBy = item._CreatedBy,
                    _CreatedUtc = item._CreatedUtc,
                    _CreatedAgent = item._CreatedAgent,
                    _LastModifiedBy = item._LastModifiedBy,
                    _LastModifiedUtc = item._LastModifiedUtc,
                    _LastModifiedAgent = item._LastModifiedAgent,
                    _IsDeleted = item._IsDeleted,
                    ProductOrderName = item.productOrderNo,
                    ReferenceNo = item.referenceNo,
                    Construction = item.construction,
                    Grade = item.grade,
                   // Piece = item.piece,
                    MaterialName = item.materialName,
                    WovenType = item.wovenType,
                    Width = item.width,
                    Yarn1 = item.yarn1,
                    Yarn2 = item.yarn2,
                    YarnType1 = item.yarnType1,
                    YarnType2 = item.yarnType2,
                    YarnOrigin1 = item.yarnOrigin1,
                    YarnOrigin2 = item.yarnOrigin2,

                    UomId = item.uomId,
                    UomUnit = item.uomUnit,
                    Quantity = item.quantity,
                    QuantityPiece = item.quantityPiece,
                    ProductRemark = item.productRemark,
                    InventoryWeavingDocumentId = item.InventoryWeavingDocumentId,
                    Barcode = item.barcode,
                    ProductionOrderDate = item.productionOrderDate,
                    DestinationArea = data.bonType,
                    Type = "IN"
                    


                }).ToList()
            };
            return model;
 
        }

        public async Task<InventoryWeavingDocument> MapToModelUpdate(InventoryWeavingDocumentDetailViewModel data)
        {
            List<InventoryWeavingDocumentItem> DocsItems = new List<InventoryWeavingDocumentItem>();
            foreach (var i in data.Detail)
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
                        Piece = d.Piece == "BESAR" ? "1" : d.Piece == "KECIL" ? "2" : "3",
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
                        Quantity = d.Quantity,
                        QuantityPiece = d.QuantityPiece,
                        ProductRemark = d.ProductRemark,

                        //InventoryWeavingDocumentId = d.InventoryWeavingDocumentId
                    });


                }
            }

            InventoryWeavingDocument model = new InventoryWeavingDocument
            {
                BonNo = data.BonNo,
                BonType = data.BonType,
                Date = data.Date,
                StorageId = data.Id,
                StorageCode = data.StorageCode,
                StorageName = data.StorageName,
                Remark = "",
                Type = data.Type,
                Items = DocsItems

            };
            return model;

        }

        private string GenerateBon(string from, DateTimeOffset date)
        {


            var type = from == "PRODUKSI" ? "PR" : from == "RETUR PACKING" ? "PC" : from == "RETUR FINISHING" ? "RF"
                      : from == "RETUR PRINTING" ? "RP" : from == "RECHEKING" ? "RC" : from == "DEVELOPMENT" ? "DV" : "LL";

            var totalData = DbSet.Count(s => s.BonNo.Substring(0, 2) == type && s._CreatedUtc.Year == date.Date.Year) + 1;

            if (totalData == 0)
            {
                return string.Format("{0}.{1}.{2}", type, date.ToString("yy"), "0001");

            }
            else
            {
                return string.Format("{0}.{1}.{2}", type, date.ToString("yy"), totalData.ToString().PadLeft(4, '0'));
            }
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
                    ReferenceNo = item.Key,
                    Construction = item.First().Construction,
                    ProductOrderNo = item.First().ProductOrderName,
                    Year = item.First().ProductOrderName.Substring(item.First().ProductOrderName.Length - 4, 4),

                    ListItems = item.Select(s => new ItemListDetailViewModel()
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

                        Grade = s.Grade,
                        //Piece = s.Piece,
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
                        //DateTime.ParseExact(s.ProductionOrderDate.Date.ToShortDateString(), "yyyy/MM/dd hh:mm:ss", CultureInfo.CreateSpecificCulture("en-US")),
                        //DateTime.ParseExact(Convert.ToString(s.ProductionOrderDate.Date), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    }).ToList()
                }).ToList()
            };
            return vm;
        }
        public InventoryWeavingDocumentDetailViewModel ReadById(int id)
        {
            var data = this.DbSet.Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                 .Include(p => p.Items).FirstOrDefault();


            if (data == null)
                return null;

            InventoryWeavingDocumentDetailViewModel DocsItems = MapToViewModelById(data);


            return DocsItems;
        }

        public int checkNota(List<InventoryWeavingDocumentCsvViewModel>data)
        {
            var index = 0;

            var data1 = data.GroupBy(x => x.ReferenceNo).Select( s => new {s.FirstOrDefault().ReferenceNo });
            foreach (var i in data1)
            {
                var query = this.DbSet3.Where(d => d.ReferenceNo.Equals(i.ReferenceNo)).Count();

                if (query != 0)
                {
                    index++;
                }


            }

         


            return index;
        }

        public ListResult<InventoryWeavingItemViewModel> ReadInputWeaving(string bonType, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int page, int size, string order, int offset)
        {

            IQueryable<InventoryWeavingDocument> Query = this.DbSet;

            List<string> SelectedFields = new List<string>()
                   {
                      "date", "bonNo", "referenceNo", "construction", "grade", "piece", "quantity", "quantityPiece", "barcode", "productionOrderDate"
                   };

            //DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
            //DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;

            var query = GetDataMonitoring(bonType, dateFrom, dateTo, offset);

            //var query = (from a in DbContext.InventoryWeavingDocuments
            //             join b in DbContext.InventoryWeavingDocumentItems on a.Id equals b.InventoryWeavingDocumentId
            //             where a._IsDeleted == false
            //                && b._IsDeleted == false
            //                && a.BonType == (string.IsNullOrWhiteSpace(bonType) ? a.BonType : bonType)
            //                && a.Type == "IN"
            //                  && a.Date.AddHours(offset).Date >= DateFrom.Date
            //                 && a.Date.AddHours(offset).Date <= DateTo.Date
            //             orderby a.Date, a._CreatedUtc ascending
            //             select new InventoryWeavingItemViewModel
            //             {
            //                 Date = a.Date,
            //                 BonNo = a.BonNo,
            //                 Construction = b.Construction,
            //                 Grade = b.Grade,
            //                 Piece = b.Piece,
            //                 Quantity = b.Quantity,
            //                 QuantityPiece = b.QuantityPiece
            //             });
            
            var data = query.Skip((page - 1) * size).Take(size);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<InventoryWeavingDocument>.Order(Query, OrderDictionary);


            return new ListResult<InventoryWeavingItemViewModel>(data.ToList(), page, size, data.Count());

        }

        public IQueryable<InventoryWeavingItemViewModel>GetDataMonitoring(string bonType, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offset)
        {
            DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
            DateTimeOffset DateTo = dateTo == null ? DateTime.Now : (DateTimeOffset)dateTo;

            var query = (from a in DbContext.InventoryWeavingDocuments
                         join b in DbContext.InventoryWeavingDocumentItems on a.Id equals b.InventoryWeavingDocumentId
                         where a._IsDeleted == false
                            && b._IsDeleted == false
                            && a.BonType == (string.IsNullOrWhiteSpace(bonType) ? a.BonType : bonType)
                            && a.Type == "IN"
                              && a.Date.AddHours(offset).Date >= DateFrom.Date
                              && a.Date.AddHours(offset).Date <= DateTo.Date
                         orderby a.Date, a._CreatedUtc ascending
                         select new InventoryWeavingItemViewModel
                         {
                             Date = a.Date,
                             BonNo = a.BonNo,
                             ReferenceNo = b.ReferenceNo,
                             Construction = b.Construction,
                             Grade = b.Grade,
                             //Piece = b.Piece,
                             Quantity = b.Quantity,
                             QuantityPiece = b.QuantityPiece,
                             Barcode = b.Barcode,
                             ProductionOrderDate = b.ProductionOrderDate,
                         })
                         ;
            return query;
        }

        public MemoryStream GenerateExcel(string bonType, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offSet)
        {
            var query = GetDataMonitoring(bonType, dateFrom, dateTo, offSet);
            //var query = _repository.ReadAll()
            //    .Where(s => s.Area == INSPECTIONMATERIAL && s.DyeingPrintingAreaInputProductionOrders.Any(d => !d.HasOutputDocument));
            //var query = _repository.ReadAll().Where(s => s.Area == DyeingPrintingArea.INSPECTIONMATERIAL);
            //if (dateFrom.HasValue && dateTo.HasValue)
            //{
            //    query = query.Where(s => dateFrom.Value.Date <= s.Date.ToOffset(new TimeSpan(offSet, 0, 0)).Date &&
            //                s.Date.ToOffset(new TimeSpan(offSet, 0, 0)).Date <= dateTo.Value.Date);
            //}
            //else if (!dateFrom.HasValue && dateTo.HasValue)
            //{
            //    query = query.Where(s => s.Date.ToOffset(new TimeSpan(offSet, 0, 0)).Date <= dateTo.Value.Date);
            //}
            //else if (dateFrom.HasValue && !dateTo.HasValue)
            //{
            //    query = query.Where(s => dateFrom.Value.Date <= s.Date.ToOffset(new TimeSpan(offSet, 0, 0)).Date);
            //}




            //query = query.OrderBy(s => s.BonNo);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "Tanggal ", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "No Bon", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nota", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Construction", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Grade", DataType = typeof(string) });
            //dt.Columns.Add(new DataColumn() { ColumnName = "Piece", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Quantity", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Quantity Piece", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Barcode", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Order Produksi", DataType = typeof(string) });


            if (query.Count() == 0)
            {
                dt.Rows.Add("", "", "", "", "", "", "", "");
            }
            else
            {
                foreach (var model in query)
                {
                    string date = model.Date == null ? "-" : model.Date.ToOffset(new TimeSpan(offSet, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string tglProduksi = model.ProductionOrderDate == null ? "-" : model.ProductionOrderDate.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //var dateIn = model.Date.Equals(DateTimeOffset.MinValue) ? "" : model.Date.ToOffset(new TimeSpan(offSet, 0, 0)).Date.ToString("d");


                        model.QuantityPiece.ToString("N2", CultureInfo.InvariantCulture));

                    //foreach (var item in model.DyeingPrintingAreaInputProductionOrders.Where(d => !d.HasOutputDocument).OrderBy(s => s.ProductionOrderNo))
                    //foreach (var item in model.DyeingPrintingAreaInputProductionOrders.OrderBy(s => s.ProductionOrderNo))
                    //{
                    //    var dateIn = item.DateIn.Equals(DateTimeOffset.MinValue) ? "" : item.DateIn.ToOffset(new TimeSpan(offSet, 0, 0)).Date.ToString("d");

                    //    dt.Rows.Add(model.BonNo, item.ProductionOrderNo, dateIn, item.ProductionOrderOrderQuantity.ToString("N2", CultureInfo.InvariantCulture),
                    //        item.CartNo, item.Construction, item.Unit, item.Buyer, item.Color, item.Motif, item.UomUnit, item.InputQuantity.ToString("N2", CultureInfo.InvariantCulture));
                    //}
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Territory") }, true);
        }

        public async Task<InventoryWeavingDocument> GetExistingModel(int[] Id)
        {
            var query = (from a in DbSet
                         join b in DbSet3 on a.Id equals b.InventoryWeavingDocumentId
                         where
                         a._IsDeleted == false
                         && b._IsDeleted == false
                         && Id.Contains(b.Id)
                         select new InventoryWeavingDocument
                         {
                             BonNo = a.BonNo,
                             BonType = a.BonType,
                             Date = a.Date,
                             StorageId = a.StorageId,
                             StorageCode = a.StorageCode,
                             StorageName = a.StorageName,
                             Remark = a.Remark,
                             Type = a.Type,
                             Items = a.Items.Select(item => new InventoryWeavingDocumentItem()
                             {
                                 Id = item.Id,
                                 Active = item.Active,
                                 _CreatedBy = item._CreatedBy,
                                 _CreatedUtc = item._CreatedUtc,
                                 _CreatedAgent = item._CreatedAgent,
                                 _LastModifiedBy = item._LastModifiedBy,
                                 _LastModifiedUtc = item._LastModifiedUtc,
                                 _LastModifiedAgent = item._LastModifiedAgent,
                                 _IsDeleted = item._IsDeleted,
                                 ProductOrderName = item.ProductOrderName,
                                 ReferenceNo = item.ReferenceNo,
                                 Construction = item.Construction,
                                 Grade = item.Grade,
                                 Piece = item.Piece,
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

                                 InventoryWeavingDocumentId = item.InventoryWeavingDocumentId
                             }).ToList()

                         }

                         );
            return query.FirstOrDefault();
        }

        //public async Task<InventoryWeavingDocument> GetExistingModel( InventoryWeavingDocument data)
        //{
        //    List<InventoryWeavingDocumentItem> DocsItems = new List<InventoryWeavingDocumentItem>();
        //    foreach (var i in Id)
        //    {
        //        i.ListItems = i.ListItems.Where(s => s.IsSave).ToList();

        //        data = data.Items.Where(s => s.Id ==  );
        //        foreach (var d in i.ListItems)
        //        {
        //            DocsItems.Add(new InventoryWeavingDocumentItem
        //            {
        //                Id = d.Id,
        //                Active = d.Active,
        //                _CreatedBy = d._CreatedBy,
        //                _CreatedUtc = d._CreatedUtc,
        //                _CreatedAgent = d._CreatedAgent,
        //                _LastModifiedBy = d._LastModifiedBy,
        //                _LastModifiedUtc = d._LastModifiedUtc,
        //                _LastModifiedAgent = d._LastModifiedAgent,
        //                _IsDeleted = d._IsDeleted,
        //                ProductOrderName = i.ProductOrderNo,
        //                ReferenceNo = i.ReferenceNo,
        //                Construction = i.Construction,

        //                Grade = d.Grade,
        //                Piece = d.Piece == "BESAR" ? "1" : d.Piece == "KECIL" ? "2" : "3",
        //                MaterialName = d.MaterialName,
        //                WovenType = d.WovenType,
        //                Width = d.Width,
        //                Yarn1 = d.Yarn1,
        //                Yarn2 = d.Yarn2,
        //                YarnType1 = d.YarnType1,
        //                YarnType2 = d.YarnType2,
        //                YarnOrigin1 = d.YarnOrigin1,
        //                YarnOrigin2 = d.YarnOrigin2,

        //                UomId = 35,
        //                UomUnit = "MTR",
        //                Quantity = d.Qty,
        //                QuantityPiece = d.QtyPiece,
        //                ProductRemark = d.ProductRemark,
        //                //InventoryWeavingDocumentId = d.InventoryWeavingDocumentId
        //            });


        //        }
        //    }

        //    InventoryWeavingDocument model = new InventoryWeavingDocument
        //    {
        //        BonNo = data.BonNo,
        //        BonType = data.BonType,
        //        Date = data.Date,
        //        StorageId = data.Id,
        //        StorageCode = data.StorageCode,
        //        StorageName = data.StorageName,
        //        Remark = "",
        //        Type = data.Type,
        //        Items = DocsItems

        //    };
        //    return model;

        //}

        public async Task<int> UpdateAsync(int id, InventoryWeavingDocument model)
        {
            using (var transaction = DbContext.Database.CurrentTransaction ?? DbContext.Database.BeginTransaction())
            {
                try
                {
                    int Updated = 0;

                    int[] Id = model.Items.Select(i => i.Id).ToArray();
                    var existingModel = this.DbSet.Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false)).Include(p => p.Items).FirstOrDefault();
                    //var existingModel = GetExistingModel(Id);

                    //var modelExist = this.DbSet.Where(x => x.Items.Any(s => Id.Contains(s.Id))).ToList();
                    //var existingModel = this.DbSet.Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false)).Include(p => p.Items.Any( s=> Id.Contains(s.Id))).FirstOrDefault();

                    //existingModel = existingModel.Items.Any(s => Id.Contains(s.Id));


                    //foreach (var existingItem in existingModel.Detail)
                    //{
                    //    GarmentLeftoverWarehouseStock stockIn = GenerateStock(existingItem);
                    //    await StockService.StockIn(stockIn, model.ExpenditureNo, model.Id, existingItem.Id);
                    //}

                    foreach (var existingItem in existingModel.Items.Where(x => Id.Contains(x.Id))) 
                    {
                        var item = model.Items.FirstOrDefault(i => i.Id == existingItem.Id);
                        
                        if (item != null)
                        {
                            if (existingItem.Quantity != item.Quantity)
                            {
                                existingItem.Quantity = item.Quantity;
                            }

                            if (existingItem.QuantityPiece != item.QuantityPiece)
                            {
                                existingItem.QuantityPiece = item.QuantityPiece;
                            }
                            existingItem.FlagForUpdate(IdentityService.Username, UserAgent);
                        }
                        //else
                        //{
                        //    if (existingItem.Quantity != item.Quantity)
                        //    {
                        //        existingItem.Quantity = item.Quantity;
                        //    }

                        //    if (existingItem.QuantityPiece != item.QuantityPiece)
                        //    {
                        //        existingItem.QuantityPiece = item.QuantityPiece;
                        //    }
                        //    existingItem.FlagForUpdate(IdentityService.Username, UserAgent);
                        //}
                    }

                    foreach (var item in model.Items.Where(i => i.Id == 0))
                    {
                        item.FlagForCreate(IdentityService.Username, UserAgent);
                        item.FlagForUpdate(IdentityService.Username, UserAgent);
                        existingModel.Items.Add(item);
                    }

                    Updated = await DbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        InventoryWeavingMovement movement = GenerateMovement(item);
                        await movementService.UpdateAsync(movement);
                    }



                    transaction.Commit();

                    return Updated;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }


        private InventoryWeavingMovement GenerateMovement(InventoryWeavingDocumentItem item)
        {
            InventoryWeavingMovement movement = new InventoryWeavingMovement
            {
                Quantity = item.Quantity,
                QuantityPiece =item.QuantityPiece,
                InventoryWeavingDocumentItemId = item.Id
                

            };

            return movement;
        }

        public async Task<int> UpdateAsyncMovement(InventoryWeavingMovement model)
        {
           
                try
                {
                    int Updated = 0;

                    var existingStock = DbSet2.Where( x => x.InventoryWeavingDocumentItemId == model.InventoryWeavingDocumentItemId).FirstOrDefault();


                    if (existingStock.Quantity != model.Quantity)
                    {
                        existingStock.Quantity = model.Quantity;
                    }
                    if (existingStock.QuantityPiece != model.QuantityPiece)
                    {
                        existingStock.QuantityPiece = model.QuantityPiece;
                    }
                    //existingStock.Quantity -= model.Quantity;
                    existingStock.FlagForUpdate(IdentityService.Username, UserAgent);

                    Updated = await DbContext.SaveChangesAsync();



                    //transaction.Commit();

                    return Updated;
                }
                catch (Exception e)
                {
                    //transaction.Rollback();
                    throw e;
                }
            
        }

    }   
}
