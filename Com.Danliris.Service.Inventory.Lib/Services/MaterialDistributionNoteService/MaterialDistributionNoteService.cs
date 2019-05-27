using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Interfaces;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialDistributionNoteViewModel;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialsRequestNoteServices;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Com.Moonlay.NetCore.Lib.Service;
using System.Net.Http;
using System.Net.Http.Headers;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryDocumentViewModel;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using System.Linq.Dynamic.Core;

namespace Com.Danliris.Service.Inventory.Lib.Services.MaterialDistributionNoteService
{
    public class MaterialDistributionNoteService : BasicService<InventoryDbContext, MaterialDistributionNote>, IMap<MaterialDistributionNote, MaterialDistributionNoteViewModel>
    {
        public MaterialDistributionNoteService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<MaterialDistributionNote>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<MaterialDistributionNote> Query = this.DbContext.MaterialDistributionNotes;

            List<string> SearchAttributes = new List<string>()
            {
                "No"
            };

            Query = ConfigureSearch(Query, SearchAttributes, Keyword);

            List<string> SelectedFields = new List<string>()
            {
                "Id", "No", "_CreatedUtc", "Type", "IsDisposition", "IsApproved"
            };

            Query = Query
                .Select(mdn => new MaterialDistributionNote
                {
                    Id = mdn.Id,
                    No = mdn.No,
                    _CreatedUtc = mdn._CreatedUtc,
                    Type = mdn.Type,
                    IsDisposition = mdn.IsDisposition,
                    IsApproved = mdn.IsApproved,
                    _LastModifiedUtc = mdn._LastModifiedUtc
                });

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = ConfigureOrder(Query, OrderDictionary);

            Pageable<MaterialDistributionNote> pageable = new Pageable<MaterialDistributionNote>(Query, Page - 1, Size);
            List<MaterialDistributionNote> Data = pageable.Data.ToList<MaterialDistributionNote>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public MaterialDistributionNote MapToModel(MaterialDistributionNoteViewModel viewModel)
        {
            MaterialDistributionNote model = new MaterialDistributionNote();
            PropertyCopier<MaterialDistributionNoteViewModel, MaterialDistributionNote>.Copy(viewModel, model);

            model.UnitIntegerId = viewModel.Unit.Id;
            model.UnitCode = viewModel.Unit.Code;
            model.UnitName = viewModel.Unit.Name;

            model.MaterialDistributionNoteItems = new List<MaterialDistributionNoteItem>();
            foreach (MaterialDistributionNoteItemViewModel mdni in viewModel.MaterialDistributionNoteItems)
            {
                MaterialDistributionNoteItem materialDistributionNoteItem = new MaterialDistributionNoteItem();
                PropertyCopier<MaterialDistributionNoteItemViewModel, MaterialDistributionNoteItem>.Copy(mdni, materialDistributionNoteItem);

                materialDistributionNoteItem.MaterialDistributionNoteDetails = new List<MaterialDistributionNoteDetail>();
                foreach (MaterialDistributionNoteDetailViewModel mdnd in mdni.MaterialDistributionNoteDetails)
                {
                    MaterialDistributionNoteDetail materialDistributionNoteDetail = new MaterialDistributionNoteDetail();
                    PropertyCopier<MaterialDistributionNoteDetailViewModel, MaterialDistributionNoteDetail>.Copy(mdnd, materialDistributionNoteDetail);

                    materialDistributionNoteDetail.ProductionOrderLongId = mdnd.ProductionOrder.Id;
                    materialDistributionNoteDetail.ProductionOrderNo = mdnd.ProductionOrder.OrderNo;
                    materialDistributionNoteDetail.ProductionOrderIsCompleted = mdnd.ProductionOrder.IsCompleted;
                    materialDistributionNoteDetail.DistributedLength = (double)mdnd.DistributedLength;

                    materialDistributionNoteDetail.ProductIntegerId = mdnd.Product.Id;
                    materialDistributionNoteDetail.ProductCode = mdnd.Product.Code;
                    materialDistributionNoteDetail.ProductName = mdnd.Product.Name;

                    materialDistributionNoteDetail.SupplierIntegerId = mdnd.Supplier.Id;
                    materialDistributionNoteDetail.SupplierCode = mdnd.Supplier.Code;
                    materialDistributionNoteDetail.SupplierName = mdnd.Supplier.Name;

                    materialDistributionNoteItem.MaterialDistributionNoteDetails.Add(materialDistributionNoteDetail);
                }

                model.MaterialDistributionNoteItems.Add(materialDistributionNoteItem);
            }

            return model;
        }

        public MaterialDistributionNoteViewModel MapToViewModel(MaterialDistributionNote model)
        {
            MaterialDistributionNoteViewModel viewModel = new MaterialDistributionNoteViewModel();
            PropertyCopier<MaterialDistributionNote, MaterialDistributionNoteViewModel>.Copy(model, viewModel);

            UnitViewModel Unit = new UnitViewModel()
            {
                Id = model.UnitIntegerId,
                Code = model.UnitCode,
                Name = model.UnitName
            };

            viewModel.Unit = Unit;

            viewModel.MaterialDistributionNoteItems = new List<MaterialDistributionNoteItemViewModel>();
            if (model.MaterialDistributionNoteItems != null)
            {
                foreach (MaterialDistributionNoteItem mdni in model.MaterialDistributionNoteItems)
                {
                    MaterialDistributionNoteItemViewModel materialDistributionNoteItemViewModel = new MaterialDistributionNoteItemViewModel();
                    PropertyCopier<MaterialDistributionNoteItem, MaterialDistributionNoteItemViewModel>.Copy(mdni, materialDistributionNoteItemViewModel);

                    materialDistributionNoteItemViewModel.MaterialDistributionNoteDetails = new List<MaterialDistributionNoteDetailViewModel>();
                    foreach (MaterialDistributionNoteDetail mdnd in mdni.MaterialDistributionNoteDetails)
                    {
                        MaterialDistributionNoteDetailViewModel materialDistributionNoteDetailViewModel = new MaterialDistributionNoteDetailViewModel();
                        PropertyCopier<MaterialDistributionNoteDetail, MaterialDistributionNoteDetailViewModel>.Copy(mdnd, materialDistributionNoteDetailViewModel);

                        ProductionOrderSQLViewModel productionOrder = new ProductionOrderSQLViewModel
                        {
                            Id = mdnd.ProductionOrderLongId,
                            OrderNo = mdnd.ProductionOrderNo,
                            IsCompleted = mdnd.ProductionOrderIsCompleted,
                        };

                        ProductSQLViewModel product = new ProductSQLViewModel
                        {
                            Id = mdnd.ProductIntegerId,
                            Code = mdnd.ProductCode,
                            Name = mdnd.ProductName
                        };

                        SupplierSQLViewModel supplier = new SupplierSQLViewModel
                        {
                            Id = mdnd.SupplierIntegerId,
                            Code = mdnd.SupplierCode,
                            Name = mdnd.SupplierName
                        };

                        materialDistributionNoteDetailViewModel.ProductionOrder = productionOrder;
                        materialDistributionNoteDetailViewModel.Product = product;
                        materialDistributionNoteDetailViewModel.Supplier = supplier;

                        materialDistributionNoteItemViewModel.MaterialDistributionNoteDetails.Add(materialDistributionNoteDetailViewModel);
                    }

                    viewModel.MaterialDistributionNoteItems.Add(materialDistributionNoteItemViewModel);
                }
            }

            return viewModel;
        }

        public override async Task<MaterialDistributionNote> ReadModelById(int id)
        {
            var Data = await this.DbSet
                .Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                .Include(d => d.MaterialDistributionNoteItems)
                    .ThenInclude(d => d.MaterialDistributionNoteDetails)
                .FirstOrDefaultAsync();

            if (Data != null)
            {
                List<int> DetailsId = new List<int>();

                foreach (MaterialDistributionNoteItem item in Data.MaterialDistributionNoteItems)
                {
                    foreach (MaterialDistributionNoteDetail detail in item.MaterialDistributionNoteDetails)
                    {
                        DetailsId.Add(detail.MaterialsRequestNoteItemId);
                    }
                }

                var RequestNoteItems = this.DbContext.MaterialsRequestNote_Items.Select(p => new { Id = p.Id, IsCompleted = p.ProductionOrderIsCompleted }).Where(p => DetailsId.Contains(p.Id));

                foreach (MaterialDistributionNoteItem item in Data.MaterialDistributionNoteItems)
                {
                    foreach (MaterialDistributionNoteDetail detail in item.MaterialDistributionNoteDetails)
                    {
                        var RequestNoteItem = RequestNoteItems.Single(p => p.Id == detail.MaterialsRequestNoteItemId);
                        detail.IsCompleted = RequestNoteItem.IsCompleted;
                    }
                }
            }

            return Data;
        }

        public void CreateInventoryDocument(MaterialDistributionNote Model, string Type)
        {
            string inventoryDocumentURI = "inventory/inventory-documents";
            string storageURI = "master/storages";
            string uomURI = "master/uoms";

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            /* Get UOM */
            Dictionary<string, object> filterUOM = new Dictionary<string, object> { { "unit", "MTR" } };
            var responseUOM = httpClient.GetAsync($@"{APIEndpoint.Core}{uomURI}?filter=" + JsonConvert.SerializeObject(filterUOM)).Result.Content.ReadAsStringAsync();
            Dictionary<string, object> resultUOM = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseUOM.Result);
            var jsonUOM = resultUOM.Single(p => p.Key.Equals("data")).Value;
            Dictionary<string, object> uom = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonUOM.ToString())[0];

            /* Get Storage */
            var storageName = Model.UnitName.Equals("PRINTING") ? "Gudang Greige Printing" : "Gudang Greige Finishing";
            Dictionary<string, object> filterStorage = new Dictionary<string, object> { { "name", storageName } };
            var responseStorage = httpClient.GetAsync($@"{APIEndpoint.Core}{storageURI}?filter=" + JsonConvert.SerializeObject(filterStorage)).Result.Content.ReadAsStringAsync();
            Dictionary<string, object> resultStorage = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseStorage.Result);
            var jsonStorage = resultStorage.Single(p => p.Key.Equals("data")).Value;
            Dictionary<string, object> storage = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonStorage.ToString())[0];

            /* Create Inventory Document */
            List<InventoryDocumentItemViewModel> inventoryDocumentItems = new List<InventoryDocumentItemViewModel>();
            List<MaterialDistributionNoteDetail> mdnds = new List<MaterialDistributionNoteDetail>();

            foreach (MaterialDistributionNoteItem mdni in Model.MaterialDistributionNoteItems)
            {
                mdnds.AddRange(mdni.MaterialDistributionNoteDetails);
            }

            //List<MaterialDistributionNoteDetail> list = mdnds
            //        .GroupBy(m => new { m.ProductId, m.ProductCode, m.ProductName })
            //        .Select(s => new MaterialDistributionNoteDetail
            //        {
            //            ProductId = s.First().ProductId,
            //            ProductCode = s.First().ProductCode,
            //            ProductName = s.First().ProductName,
            //            ReceivedLength = s.Sum(d => d.ReceivedLength),
            //            MaterialRequestNoteItemLength = s.Sum(d => d.MaterialRequestNoteItemLength)
            //        }).ToList();

            foreach (MaterialDistributionNoteDetail mdnd in mdnds)
            {
                InventoryDocumentItemViewModel inventoryDocumentItem = new InventoryDocumentItemViewModel
                {
                    productId = mdnd.ProductId,
                    productCode = mdnd.ProductCode,
                    productName = mdnd.ProductName,
                    quantity = mdnd.ReceivedLength,
                    stockPlanning = Model.Type != "RE-GRADING" ? (mdnd.DistributedLength == 0 ? mdnd.MaterialRequestNoteItemLength - mdnd.ReceivedLength : mdnd.ReceivedLength * -1) : mdnd.ReceivedLength * -1,
                    uomId = uom["_id"].ToString(),
                    uom = uom["unit"].ToString()
                };

                inventoryDocumentItems.Add(inventoryDocumentItem);
            }

            List<InventoryDocumentItemViewModel> list = inventoryDocumentItems
                    .GroupBy(m => new { m.productId, m.productCode, m.productName })
                    .Select(s => new InventoryDocumentItemViewModel
                    {
                        productId = s.First().productId,
                        productCode = s.First().productCode,
                        productName = s.First().productName,
                        quantity = s.Sum(d => d.quantity),
                        stockPlanning = s.Sum(d => d.stockPlanning),
                        uom = s.First().uom,
                        uomId = s.First().uomId
                    }).ToList();

            InventoryDocumentViewModel inventoryDocument = new InventoryDocumentViewModel
            {
                date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                referenceNo = Model.No,
                referenceType = "Bon Pengantar Greige",
                type = Type,
                storageId = storage["_id"].ToString(),
                storageCode = storage["code"].ToString(),
                storageName = storage["name"].ToString(),
                items = list
            };

            var response = httpClient.PostAsync($"{APIEndpoint.Inventory}{inventoryDocumentURI}", new StringContent(JsonConvert.SerializeObject(inventoryDocument).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            response.EnsureSuccessStatusCode();
        }

        public async Task<MaterialDistributionNote> CustomCodeGenerator(MaterialDistributionNote Model)
        {
            var Type = string.Equals(Model.UnitName.ToUpper(), "PRINTING") ? "PR" : "FS";
            var lastData = await this.DbSet.Where(w => w.UnitName == Model.UnitName).OrderByDescending(o => o._CreatedUtc).FirstOrDefaultAsync();

            DateTime Now = DateTime.Now;
            string Year = Now.ToString("yy");

            if (lastData == null)
            {
                Model.AutoIncrementNumber = 1;
                string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                Model.No = $"P{Type}{Year}{Number}";
            }
            else
            {
                if (lastData._CreatedUtc.Year < Now.Year)
                {
                    Model.AutoIncrementNumber = 1;
                    string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                    Model.No = $"P{Type}{Year}{Number}";
                }
                else
                {
                    Model.AutoIncrementNumber = lastData.AutoIncrementNumber + 1;
                    string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                    Model.No = $"P{Type}{Year}{Number}";
                }
            }

            return Model;
        }

        public override async Task<int> CreateModel(MaterialDistributionNote Model)
        {
            int Created = 0;
            using (var transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    Model = await this.CustomCodeGenerator(Model);
                    Created = await this.CreateAsync(Model);

                    MaterialsRequestNoteService materialsRequestNoteService = ServiceProvider.GetService<MaterialsRequestNoteService>();
                    MaterialsRequestNote_ItemService materialsRequestNote_ItemService = ServiceProvider.GetService<MaterialsRequestNote_ItemService>();
                    materialsRequestNoteService.Username = Username;
                    materialsRequestNoteService.Token = Token;
                    materialsRequestNote_ItemService.Username = Username;

                    List<InventorySummaryViewModel> data = new List<InventorySummaryViewModel>();


                    foreach (MaterialDistributionNoteItem materialDistributionNoteItem in Model.MaterialDistributionNoteItems)
                    {


                        //inventory summary data
                        //if (!(Model.Type.ToUpper().Equals("RE - GRADING")))
                        //{
                        //    foreach (MaterialDistributionNoteDetail materialDistributionNoteDetail in materialDistributionNoteItem.MaterialDistributionNoteDetails)
                        //    {
                        //        var Distributed = DbContext.MaterialsRequestNotes.Single(p => p.Id == materialDistributionNoteItem.MaterialRequestNoteId);
                        //        //InventorySummaryViewModel InventorySummary = new InventorySummaryViewModel();
                        //        //InventorySummary.quantity = Distributed.IsDistributed ? materialDistributionNoteDetail.ReceivedLength * -1 : materialDistributionNoteDetail.ReceivedLength;
                        //        //InventorySummary.productCode = materialDistributionNoteDetail.ProductCode;
                        //        //InventorySummary.productId = materialDistributionNoteDetail.ProductId;
                        //        //InventorySummary.productName = materialDistributionNoteDetail.ProductName;
                        //        //InventorySummary.storageName = Model.UnitName;
                        //        //InventorySummary.uom = "MTR";

                        //        materialDistributionNoteDetail.ReceivedLength = Distributed.IsDistributed ? materialDistributionNoteDetail.ReceivedLength * -1 : materialDistributionNoteDetail.MaterialRequestNoteItemLength - materialDistributionNoteDetail.ReceivedLength;

                        //        data.Add(InventorySummary);
                        //    }
                        //}

                        MaterialsRequestNote materialsRequestNote = await materialsRequestNoteService.ReadModelById(materialDistributionNoteItem.MaterialRequestNoteId);
                        materialsRequestNote.IsDistributed = true;

                        if (Model.Type.ToUpper().Equals("PRODUKSI"))
                        {
                            foreach (MaterialDistributionNoteDetail materialDistributionNoteDetail in materialDistributionNoteItem.MaterialDistributionNoteDetails)
                            {
                                materialsRequestNote.MaterialsRequestNote_Items.Where(w => w.ProductionOrderId.Equals(materialDistributionNoteDetail.ProductionOrderId)).Select(s => { s.DistributedLength += materialDistributionNoteDetail.ReceivedLength; return s; }).ToList();
                            }
                            materialsRequestNoteService.UpdateDistributedQuantity(materialsRequestNote.Id, materialsRequestNote);

                            foreach (MaterialsRequestNote_Item materialRequestNoteItem in materialsRequestNote.MaterialsRequestNote_Items)
                            {
                                materialsRequestNote_ItemService.OnUpdating(materialRequestNoteItem.Id, materialRequestNoteItem);
                                materialsRequestNote_ItemService.DbSet.Update(materialRequestNoteItem);
                            }
                        }

                        materialsRequestNoteService.OnUpdating(materialsRequestNote.Id, materialsRequestNote);
                        materialsRequestNoteService.DbSet.Update(materialsRequestNote);

                    }

                    DbContext.SaveChanges();

                    CreateInventoryDocument(Model, "OUT");

                    //this.UpdateInventorySummary(data);

                    transaction.Commit();
                }
                catch (ServiceValidationExeption e)
                {
                    transaction.Rollback();
                    throw new ServiceValidationExeption(e.ValidationContext, e.ValidationResults);
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new ServiceValidationExeption(null, null);
                }
            }
            return Created;
        }

        public bool UpdateIsApprove(List<int> Ids)
        {
            bool IsSuccessful = false;

            using (var Transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var mdn = this.DbSet.Where(m => Ids.Contains(m.Id)).ToList();
                    mdn.ForEach(m => { m.IsApproved = true; m._LastModifiedUtc = DateTime.UtcNow; m._LastModifiedAgent = "Service"; m._LastModifiedBy = this.Username; });
                    this.DbContext.SaveChanges();

                    IsSuccessful = true;
                    Transaction.Commit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    Transaction.Rollback();
                    throw;
                }
            }

            return IsSuccessful;
        }

        //public async Task<int> UpdateIsCompleted(int Id, MaterialDistributionNote Model)
        //{
        //    int IsSucceed = 0;

        //    using (var Transaction = this.DbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            IsSucceed = await UpdateModel(Id, Model);
        //            MaterialDistributionNoteItemService materialDistributionNoteItemService = ServiceProvider.GetService<MaterialDistributionNoteItemService>();
        //            MaterialDistributionNoteDetailService materialDistributionNoteDetailService = ServiceProvider.GetService<MaterialDistributionNoteDetailService>();
        //            MaterialsRequestNoteService materialsRequestNoteService = ServiceProvider.GetService<MaterialsRequestNoteService>();

        //            materialDistributionNoteItemService.Username = this.Username;
        //            materialDistributionNoteDetailService.Username = this.Username;
        //            materialsRequestNoteService.Username = Username;
        //            materialsRequestNoteService.Token = Token;

        //            foreach (MaterialDistributionNoteItem materialDistributionNoteItem in Model.MaterialDistributionNoteItems)
        //            {
        //                MaterialsRequestNote materialsRequestNote = await materialsRequestNoteService.ReadModelById(materialDistributionNoteItem.MaterialRequestNoteId);
        //                await materialDistributionNoteItemService.UpdateModel(materialDistributionNoteItem.Id, materialDistributionNoteItem);

        //                foreach (MaterialDistributionNoteDetail materialDistributionNoteDetail in materialDistributionNoteItem.MaterialDistributionNoteDetails)
        //                {
        //                    await materialDistributionNoteDetailService.UpdateModel(materialDistributionNoteDetail.Id, materialDistributionNoteDetail);
        //                    if (materialDistributionNoteDetail.IsCompleted)
        //                    {
        //                        materialsRequestNote.MaterialsRequestNote_Items.Where(w => w.ProductionOrderId.Equals(materialDistributionNoteDetail.ProductionOrderId)).Select(s => { s.ProductionOrderIsCompleted = true; return s; }).ToList();
        //                    }
        //                }

        //                await materialsRequestNoteService.UpdateIsComplete(materialsRequestNote.Id, materialsRequestNote);

        //            }

        //            Transaction.Commit();
        //        }
        //        catch (Exception)
        //        {
        //            Transaction.Rollback();
        //        }
        //    }

        //    return IsSucceed;
        //}

        public override async Task<int> DeleteModel(int Id)
        {
            int Count = 0;

            using (var Transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    MaterialDistributionNote materialDistributionNote = await ReadModelById(Id);
                    Count = this.Delete(Id);

                    MaterialDistributionNoteItemService materialDistributionNoteItemService = ServiceProvider.GetService<MaterialDistributionNoteItemService>();
                    MaterialDistributionNoteDetailService materialDistributionNoteDetailService = ServiceProvider.GetService<MaterialDistributionNoteDetailService>();
                    materialDistributionNoteItemService.Username = this.Username;
                    materialDistributionNoteDetailService.Username = this.Username;

                    HashSet<int> MaterialDistributionNoteItems = new HashSet<int>(this.DbContext.MaterialDistributionNoteItems.Where(p => p.MaterialDistributionNoteId.Equals(Id)).Select(p => p.Id));

                    List<InventorySummaryViewModel> data = new List<InventorySummaryViewModel>();

                    foreach (int item in MaterialDistributionNoteItems)
                    {
                        HashSet<int> MaterialDistributionNoteDetails = new HashSet<int>(this.DbContext.MaterialDistributionNoteDetails.Where(p => p.MaterialDistributionNoteItemId.Equals(item)).Select(p => p.Id));

                        foreach (int detail in MaterialDistributionNoteDetails)
                        {
                            await materialDistributionNoteDetailService.DeleteAsync(detail);
                        }

                        await materialDistributionNoteItemService.DeleteAsync(item);
                    }

                    MaterialsRequestNoteService materialsRequestNoteService = ServiceProvider.GetService<MaterialsRequestNoteService>();
                    MaterialsRequestNote_ItemService materialsRequestNote_ItemService = ServiceProvider.GetService<MaterialsRequestNote_ItemService>();
                    materialsRequestNoteService.Username = Username;
                    materialsRequestNoteService.Token = Token;
                    materialsRequestNote_ItemService.Username = Username;

                    foreach (MaterialDistributionNoteItem materialDistributionNoteItem in materialDistributionNote.MaterialDistributionNoteItems)
                    {
                        MaterialsRequestNote materialsRequestNote = await materialsRequestNoteService.ReadModelById(materialDistributionNoteItem.MaterialRequestNoteId);
                        materialsRequestNote.IsDistributed = true;

                        //inventory summary data

                        if (materialDistributionNote.Type.ToUpper().Equals("PRODUKSI"))
                        {
                            foreach (MaterialDistributionNoteDetail materialDistributionNoteDetail in materialDistributionNoteItem.MaterialDistributionNoteDetails)
                            {
                                materialsRequestNote.MaterialsRequestNote_Items.Where(w => w.ProductionOrderId.Equals(materialDistributionNoteDetail.ProductionOrderId)).Select(s => { s.DistributedLength -= materialDistributionNoteDetail.ReceivedLength; return s; }).ToList();


                            }
                            materialsRequestNoteService.UpdateDistributedQuantity(materialsRequestNote.Id, materialsRequestNote);

                            foreach (MaterialsRequestNote_Item materialRequestNoteItem in materialsRequestNote.MaterialsRequestNote_Items)
                            {
                                materialsRequestNote_ItemService.OnUpdating(materialRequestNoteItem.Id, materialRequestNoteItem);
                                materialsRequestNote_ItemService.DbSet.Update(materialRequestNoteItem);
                            }
                        }

                        materialsRequestNoteService.OnUpdating(materialsRequestNote.Id, materialsRequestNote);
                        materialsRequestNoteService.DbSet.Update(materialsRequestNote);
                    }

                    DbContext.SaveChanges();

                    CreateInventoryDocument(materialDistributionNote, "IN");
                    Transaction.Commit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    Transaction.Rollback();
                    throw;
                }
            }

            return Count;
        }

        public override void OnCreating(MaterialDistributionNote model)
        {
            base.OnCreating(model);

            model._CreatedAgent = "Service";
            model._CreatedBy = this.Username;
            model._LastModifiedAgent = "Service";
            model._LastModifiedBy = this.Username;

            MaterialDistributionNoteItemService materialDistributionNoteItemService = ServiceProvider.GetService<MaterialDistributionNoteItemService>();
            materialDistributionNoteItemService.Username = this.Username;

            foreach (MaterialDistributionNoteItem mdni in model.MaterialDistributionNoteItems)
            {
                materialDistributionNoteItemService.OnCreating(mdni);
            }
        }

        public override void OnUpdating(int id, MaterialDistributionNote model)
        {
            base.OnUpdating(id, model);
            model._LastModifiedAgent = "Service";
            model._LastModifiedBy = this.Username;
        }

        public override void OnDeleting(MaterialDistributionNote model)
        {
            base.OnDeleting(model);
            model._DeletedAgent = "Service";
            model._DeletedBy = this.Username;
        }

        public IQueryable<MaterialDistributionNoteReportViewModel> GetReportQuery(string unitId, string type, DateTime date, int offset)
        {
            var Query = (from a in DbContext.MaterialDistributionNotes
                         join b in DbContext.MaterialDistributionNoteItems on a.Id equals b.MaterialDistributionNoteId
                         join c in DbContext.MaterialDistributionNoteDetails on b.Id equals c.MaterialDistributionNoteItemId
                         where a._IsDeleted == false
                             && a.UnitId == (string.IsNullOrWhiteSpace(unitId) ? a.UnitId : unitId)
                             && a.Type == (string.IsNullOrWhiteSpace(type) ? a.Type : type)
                             && a._CreatedUtc.AddHours(offset).Date == date.Date
                         select new MaterialDistributionNoteReportViewModel
                         {
                             _LastModifiedUtc = a._LastModifiedUtc,
                             No = a.No,
                             Type = a.Type,
                             MaterialRequestNoteNo = b.MaterialRequestNoteCode,
                             ProductionOrderNo = c.ProductionOrderNo,
                             ProductName = c.ProductName,
                             Grade = c.Grade,
                             Quantity = c.Quantity,
                             Length = c.ReceivedLength,
                             SupplierName = c.SupplierName,
                             IsDisposition = a.IsDisposition,
                             IsApproved = a.IsApproved
                         });

            return Query;
        }

        public Tuple<List<MaterialDistributionNoteReportViewModel>, int> GetReport(string unitId, string type, DateTime date, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(unitId, type, date, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
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

            Pageable<MaterialDistributionNoteReportViewModel> pageable = new Pageable<MaterialDistributionNoteReportViewModel>(Query, page - 1, size);
            List<MaterialDistributionNoteReportViewModel> Data = pageable.Data.ToList<MaterialDistributionNoteReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public List<MaterialDistributionNoteReportViewModel> GetPdfReport(string unitId, string unitName, string type, DateTime date, int offset)
        {
            var Data = GetReportQuery(unitId, type, date, offset).ToList();

            return Data;
        }

    }
}