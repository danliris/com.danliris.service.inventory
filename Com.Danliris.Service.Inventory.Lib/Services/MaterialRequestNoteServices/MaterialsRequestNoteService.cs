using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Com.Moonlay.NetCore.Lib;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Com.Danliris.Service.Inventory.Lib.Interfaces;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialsRequestNoteViewModel;
using Com.Moonlay.NetCore.Lib.Service;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryDocumentViewModel;

namespace Com.Danliris.Service.Inventory.Lib.Services.MaterialsRequestNoteServices
{
    public class MaterialsRequestNoteService : BasicService<InventoryDbContext, MaterialsRequestNote>, IMap<MaterialsRequestNote, MaterialsRequestNoteViewModel>
    {
        public MaterialsRequestNoteService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<MaterialsRequestNote>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<MaterialsRequestNote> Query = this.DbContext.MaterialsRequestNotes;

            List<string> SearchAttributes = new List<string>()
                {
                    "UnitName", "RequestType", "Code", "MaterialsRequestNote_Items.ProductionOrderNo"
                };
            Query = ConfigureSearch(Query, SearchAttributes, Keyword);

            List<string> SelectedFields = new List<string>()
                {
                    "Id", "Code", "Unit", "RequestType", "Remark", "MaterialsRequestNote_Items", "_LastModifiedUtc", "IsCompleted"
                };
            Query = Query
                .Select(mrn => new MaterialsRequestNote
                {
                    Id = mrn.Id,
                    Code = mrn.Code,
                    UnitId = mrn.UnitId,
                    UnitCode = mrn.UnitCode,
                    UnitName = mrn.UnitName,
                    IsDistributed = mrn.IsDistributed,
                    IsCompleted = mrn.IsCompleted,
                    RequestType = mrn.RequestType,
                    _LastModifiedUtc = mrn._LastModifiedUtc,
                    MaterialsRequestNote_Items = mrn.MaterialsRequestNote_Items.Select(p => new MaterialsRequestNote_Item { MaterialsRequestNoteId = p.MaterialsRequestNoteId, ProductionOrderNo = p.ProductionOrderNo }).Where(i => i.MaterialsRequestNoteId.Equals(mrn.Id)).ToList()
                });

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = ConfigureOrder(Query, OrderDictionary);

            Pageable<MaterialsRequestNote> pageable = new Pageable<MaterialsRequestNote>(Query, Page - 1, Size);
            List<MaterialsRequestNote> Data = pageable.Data.ToList<MaterialsRequestNote>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public async Task<MaterialsRequestNote> CustomCodeGenerator(MaterialsRequestNote Model)
        {
            Model.Type = string.Equals(Model.UnitName.ToUpper(), "PRINTING") ? "P" : "F";
            var lastData = await this.DbSet.Where(w => w.UnitName == Model.UnitName).OrderByDescending(o => o._CreatedUtc).FirstOrDefaultAsync();

            DateTime Now = DateTime.Now;
            string Year = Now.ToString("yy");
            string Month = Now.ToString("MM");

            if (lastData == null)
            {
                Model.AutoIncrementNumber = 1;
                string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                Model.Code = $"SPB{Model.Type}{Month}{Year}{Number}";
            }
            else
            {
                if (lastData._CreatedUtc.Year < Now.Year)
                {
                    Model.AutoIncrementNumber = 1;
                    string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                    Model.Code = $"SPB{Model.Type}{Month}{Year}{Number}";
                }
                else
                {
                    Model.AutoIncrementNumber = lastData.AutoIncrementNumber + 1;
                    string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                    Model.Code = $"SPB{Model.Type}{Month}{Year}{Number}";
                }
            }

            return Model;
        }

        public void UpdateIsRequestedProductionOrder(List<string> productionOrderIds, string context)
        {
            string productionOrderUri = "sales/production-orders/update/is-requested";

            var data = new
            {
                context = context,
                ids = productionOrderIds
            };

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var response = httpClient.PutAsync($"{APIEndpoint.Production}{productionOrderUri}", new StringContent(JsonConvert.SerializeObject(data).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            response.EnsureSuccessStatusCode();
        }

        public void UpdateIsCompletedProductionOrder(List<SppParams> contextAndIds)
        {
            string productionOrderUri = "sales/production-orders/update/is-completed";

            var data = new
            {
                contextAndIds = contextAndIds
            };

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var response = httpClient.PutAsync($"{APIEndpoint.Production}{productionOrderUri}", new StringContent(JsonConvert.SerializeObject(data).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            response.EnsureSuccessStatusCode();
        }

        public void UpdateDistributedQuantityProductionOrder(List<SppParams> contextAndIds)
        {
            string productionOrderUri = "sales/production-orders/update/distributed-quantity";

            var data = new
            {
                data = contextAndIds
            };

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var response = httpClient.PutAsync($"{APIEndpoint.Production}{productionOrderUri}", new StringContent(JsonConvert.SerializeObject(data).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            response.EnsureSuccessStatusCode();
        }

        //public void UpdateInventorySummary(List<InventorySummaryViewModel> item)
        //{
        //    string inventorySummary = "inventory/inventory-summary/update/all-summary";

        //    HttpClient httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        //    var response = httpClient.PutAsync($"{APIEndpoint.Inventory}{inventorySummary}", new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, General.JsonMediaType)).Result;
        //    response.EnsureSuccessStatusCode();
        //}

        public override async Task<int> CreateModel(MaterialsRequestNote Model)
        {
            int Created = 0;
            using (var transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<string> productionOrderIds = new List<string>();
                    Model = await this.CustomCodeGenerator(Model);
                    Created = await this.CreateAsync(Model);


                    foreach (MaterialsRequestNote_Item item in Model.MaterialsRequestNote_Items)
                    {
                        productionOrderIds.Add(item.ProductionOrderId);
                    }

                    if (Model.RequestType != "PEMBELIAN")
                    {
                        this.CreateInventoryDocument(Model, "OUT");
                    }

                    UpdateIsRequestedProductionOrder(productionOrderIds, "CREATE");
                    transaction.Commit();
                }
                catch (ServiceValidationExeption e)
                {
                    throw new ServiceValidationExeption(e.ValidationContext, e.ValidationResults);
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
            }
            return Created;
        }

        public override async Task<MaterialsRequestNote> ReadModelById(int id)
        {
            return await this.DbSet
                .Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                .Include(d => d.MaterialsRequestNote_Items)
                .FirstOrDefaultAsync();
        }

        public class SppParams
        {
            public string context { get; set; }
            public string id { get; set; }
            public double distributedQuantity { get; set; }
        }

        public async Task UpdateIsCompleted(int Id, MaterialsRequestNote Model)
        {
            {
                try
                {
                    int CountIsIncomplete = 0;
                    List<SppParams> contextAndIds = new List<SppParams>();
                    foreach (MaterialsRequestNote_Item item in Model.MaterialsRequestNote_Items)
                    {
                        SppParams sppParams = new SppParams();
                        if (!item.ProductionOrderIsCompleted)
                        {
                            CountIsIncomplete += 1;
                            sppParams.context = "INCOMPLETE";
                            sppParams.id = item.ProductionOrderId;
                        }
                        else
                        {
                            sppParams.context = "COMPLETE";
                            sppParams.id = item.ProductionOrderId;
                        }

                        contextAndIds.Add(sppParams);
                    }
                    UpdateIsCompletedProductionOrder(contextAndIds);

                    if (CountIsIncomplete == 0)
                    {
                        Model.IsCompleted = true;
                    }
                    else
                    {
                        Model.IsCompleted = false;
                    }

                    await UpdateModel(Id, Model);


                }
                catch (Exception e)
                {
                    Console.Write(e);
                }
            }

        }

        public void UpdateDistributedQuantity(int Id, MaterialsRequestNote Model)
        {
            {
                try
                {
                    List<SppParams> contextQuantityAndIds = new List<SppParams>();
                    foreach (MaterialsRequestNote_Item item in Model.MaterialsRequestNote_Items)
                    {
                        SppParams sppParams = new SppParams
                        {
                            id = item.ProductionOrderId,
                            distributedQuantity = item.DistributedLength
                        };

                        contextQuantityAndIds.Add(sppParams);
                    }
                    UpdateDistributedQuantityProductionOrder(contextQuantityAndIds);
                }
                catch (Exception)
                {
                }
            }
        }

        public override async Task<int> UpdateModel(int Id, MaterialsRequestNote Model)
        {

            MaterialsRequestNote_ItemService materialsRequestNote_ItemService = this.ServiceProvider.GetService<MaterialsRequestNote_ItemService>();
            materialsRequestNote_ItemService.Username = this.Username;

            int Updated = 0;
            using (var transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {

                    HashSet<object> materialsRequestNote_Items = new HashSet<object>(materialsRequestNote_ItemService.DbSet
                        .Where(w => w.MaterialsRequestNoteId.Equals(Id))
                        .AsNoTracking());
                    //.Select(s => new { Id = s.Id, ProductionOrderId = s.ProductionOrderId }));

                    Updated = await this.UpdateAsync(Id, Model);

                    List<string> productionOrderIds = new List<string>();
                    List<string> newProductionOrderIds = new List<string>();
                    List<MaterialsRequestNote_Item> itemForUpdateInventory = new List<MaterialsRequestNote_Item>();

                    foreach (dynamic materialsRequestNote_Item in materialsRequestNote_Items)
                    {
                        MaterialsRequestNote_Item model = Model.MaterialsRequestNote_Items.FirstOrDefault<dynamic>(prop => prop.Id.Equals(materialsRequestNote_Item.Id));
                        if (model == null)
                        {
                            await materialsRequestNote_ItemService.DeleteModel(materialsRequestNote_Item.Id);
                            productionOrderIds.Add(materialsRequestNote_Item.ProductionOrderId);
                            itemForUpdateInventory.Add(materialsRequestNote_Item);
                        }
                        else
                        {
                            double length = materialsRequestNote_Item.Length - model.Length;
                            materialsRequestNote_Item.Length = length;
                            itemForUpdateInventory.Add(materialsRequestNote_Item);
                            await materialsRequestNote_ItemService.UpdateModel(materialsRequestNote_Item.Id, model);
                        }
                    }

                    foreach (MaterialsRequestNote_Item materialsRequestNote_Item in Model.MaterialsRequestNote_Items)
                    {
                        if (materialsRequestNote_Item.Id.Equals(0))
                        {
                            await materialsRequestNote_ItemService.CreateModel(materialsRequestNote_Item);
                            newProductionOrderIds.Add(materialsRequestNote_Item.ProductionOrderId);
                            double length = materialsRequestNote_Item.Length * -1;
                            materialsRequestNote_Item.Length = length;
                            itemForUpdateInventory.Add(materialsRequestNote_Item);
                        }
                    }

                    if (Model.RequestType != "PEMBELIAN")
                    {
                        Model.MaterialsRequestNote_Items = itemForUpdateInventory;
                        this.CreateInventoryDocument(Model, "IN");
                    }

                    UpdateIsRequestedProductionOrder(productionOrderIds, "UPDATE");
                    UpdateIsRequestedProductionOrder(newProductionOrderIds, "CREATE");
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
            }

            return Updated;
        }

        public override async Task<int> DeleteModel(int Id)
        {
            MaterialsRequestNote_ItemService materialsRequestNote_ItemService = this.ServiceProvider.GetService<MaterialsRequestNote_ItemService>();

            int Deleted = 0;
            using (var transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    MaterialsRequestNote Model = await this.ReadModelById(Id);
                    Deleted = await this.DeleteAsync(Id);


                    HashSet<object> materialsRequestNote_Items = new HashSet<object>(materialsRequestNote_ItemService.DbSet
                    .Where(w => w.MaterialsRequestNoteId.Equals(Id))
                    .AsNoTracking());

                    materialsRequestNote_ItemService.Username = this.Username;

                    foreach (dynamic materialsRequestNote_Item in materialsRequestNote_Items)
                    {
                        await materialsRequestNote_ItemService.DeleteModel(materialsRequestNote_Item.Id);
                    }

                    List<string> productionOrderIds = new List<string>();

                    foreach (MaterialsRequestNote_Item item in Model.MaterialsRequestNote_Items)
                    {
                        productionOrderIds.Add(item.ProductionOrderId);

                    }

                    if (Model.RequestType != "PEMBELIAN")
                    {
                        this.CreateInventoryDocument(Model, "IN");
                    }


                    UpdateIsRequestedProductionOrder(productionOrderIds, "DELETE");
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }

            return Deleted;
        }

        public override void OnCreating(MaterialsRequestNote model)
        {
            if (model.MaterialsRequestNote_Items.Count > 0)
            {
                MaterialsRequestNote_ItemService materialsRequestNote_ItemService = this.ServiceProvider.GetService<MaterialsRequestNote_ItemService>();

                materialsRequestNote_ItemService.Username = this.Username;
                foreach (MaterialsRequestNote_Item materialsRequestNoteItem in model.MaterialsRequestNote_Items)
                {
                    materialsRequestNote_ItemService.OnCreating(materialsRequestNoteItem);
                }
            }

            base.OnCreating(model);
            model._CreatedAgent = "Service";
            model._CreatedBy = this.Username;
            model._LastModifiedAgent = "Service";
            model._LastModifiedBy = this.Username;
        }

        public override void OnUpdating(int id, MaterialsRequestNote model)
        {
            base.OnUpdating(id, model);
            model._LastModifiedAgent = "Service";
            model._LastModifiedBy = this.Username;
        }

        public override void OnDeleting(MaterialsRequestNote model)
        {
            base.OnDeleting(model);
            model._DeletedAgent = "Service";
            model._DeletedBy = this.Username;
        }

        public MaterialsRequestNoteViewModel MapToViewModel(MaterialsRequestNote model)
        {
            MaterialsRequestNoteViewModel viewModel = new MaterialsRequestNoteViewModel();

            PropertyCopier<MaterialsRequestNote, MaterialsRequestNoteViewModel>.Copy(model, viewModel);

            UnitViewModel Unit = new UnitViewModel()
            {
                _id = model.UnitId,
                code = model.UnitCode,
                name = model.UnitName
            };

            viewModel.Code = model.Code;
            viewModel.Unit = Unit;
            viewModel.RequestType = model.RequestType;
            viewModel.Remark = model.Remark;

            viewModel.MaterialsRequestNote_Items = new List<MaterialsRequestNote_ItemViewModel>();
            if (model.MaterialsRequestNote_Items != null)
            {
                foreach (MaterialsRequestNote_Item materialsRequestNote_Item in model.MaterialsRequestNote_Items)
                {
                    MaterialsRequestNote_ItemViewModel materialsRequestNote_ItemViewModel = new MaterialsRequestNote_ItemViewModel();
                    PropertyCopier<MaterialsRequestNote_Item, MaterialsRequestNote_ItemViewModel>.Copy(materialsRequestNote_Item, materialsRequestNote_ItemViewModel);

                    OrderTypeViewModel OrderType = new OrderTypeViewModel()
                    {
                        _id = materialsRequestNote_Item.OrderTypeId,
                        code = materialsRequestNote_Item.OrderTypeCode,
                        name = materialsRequestNote_Item.OrderTypeName
                    };

                    ProductionOrderViewModel ProductionOrder = new ProductionOrderViewModel()
                    {
                        _id = materialsRequestNote_Item.ProductionOrderId,
                        orderNo = materialsRequestNote_Item.ProductionOrderNo,
                        orderQuantity = materialsRequestNote_Item.OrderQuantity,
                        isCompleted = materialsRequestNote_Item.ProductionOrderIsCompleted,
                        distributedQuantity = materialsRequestNote_Item.DistributedLength,
                        orderType = OrderType
                    };
                    materialsRequestNote_ItemViewModel.ProductionOrder = ProductionOrder;

                    ProductViewModel Product = new ProductViewModel()
                    {
                        _id = materialsRequestNote_Item.ProductId,
                        code = materialsRequestNote_Item.ProductCode,
                        name = materialsRequestNote_Item.ProductName
                    };
                    materialsRequestNote_ItemViewModel.Product = Product;

                    materialsRequestNote_ItemViewModel.Length = materialsRequestNote_Item.Length;
                    materialsRequestNote_ItemViewModel.DistributedLength = materialsRequestNote_Item.DistributedLength;

                    viewModel.MaterialsRequestNote_Items.Add(materialsRequestNote_ItemViewModel);
                }
            }

            return viewModel;
        }

        public MaterialsRequestNote MapToModel(MaterialsRequestNoteViewModel viewModel)
        {
            MaterialsRequestNote model = new MaterialsRequestNote();

            PropertyCopier<MaterialsRequestNoteViewModel, MaterialsRequestNote>.Copy(viewModel, model);

            model.UnitId = viewModel.Unit._id;
            model.UnitCode = viewModel.Unit.code;
            model.UnitName = viewModel.Unit.name;
            model.RequestType = viewModel.RequestType;
            model.Remark = viewModel.Remark;

            model.MaterialsRequestNote_Items = new List<MaterialsRequestNote_Item>();

            foreach (MaterialsRequestNote_ItemViewModel materialsRequestNote_ItemViewModel in viewModel.MaterialsRequestNote_Items)
            {
                MaterialsRequestNote_Item materialsRequestNote_Item = new MaterialsRequestNote_Item();

                PropertyCopier<MaterialsRequestNote_ItemViewModel, MaterialsRequestNote_Item>.Copy(materialsRequestNote_ItemViewModel, materialsRequestNote_Item);

                if (!viewModel.RequestType.Equals("PEMBELIAN") && !viewModel.RequestType.Equals("TEST"))
                {

                    materialsRequestNote_Item.ProductionOrderId = materialsRequestNote_ItemViewModel.ProductionOrder._id;
                    materialsRequestNote_Item.ProductionOrderNo = materialsRequestNote_ItemViewModel.ProductionOrder.orderNo;
                    materialsRequestNote_Item.ProductionOrderIsCompleted = materialsRequestNote_ItemViewModel.ProductionOrder.isCompleted;
                    materialsRequestNote_Item.OrderQuantity = (double)materialsRequestNote_ItemViewModel.ProductionOrder.orderQuantity;
                    materialsRequestNote_Item.OrderTypeId = materialsRequestNote_ItemViewModel.ProductionOrder.orderType._id;
                    materialsRequestNote_Item.OrderTypeCode = materialsRequestNote_ItemViewModel.ProductionOrder.orderType.code;
                    materialsRequestNote_Item.OrderTypeName = materialsRequestNote_ItemViewModel.ProductionOrder.orderType.name;
                }

                materialsRequestNote_Item.ProductId = materialsRequestNote_ItemViewModel.Product._id;
                materialsRequestNote_Item.ProductCode = materialsRequestNote_ItemViewModel.Product.code;
                materialsRequestNote_Item.ProductName = materialsRequestNote_ItemViewModel.Product.name;
                materialsRequestNote_Item.Length = materialsRequestNote_ItemViewModel.Length != null ? (double)materialsRequestNote_ItemViewModel.Length : 0;
                materialsRequestNote_Item.DistributedLength = materialsRequestNote_ItemViewModel.DistributedLength != null ? (double)materialsRequestNote_ItemViewModel.DistributedLength : 0;

                model.MaterialsRequestNote_Items.Add(materialsRequestNote_Item);
            }

            return model;
        }

        public IQueryable<MaterialsRequestNoteReportViewModel> GetReportQuery(string materialsRequestNoteCode, string productionOrderId, string unitId, string productId, string status, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            bool IsCompleted = !string.IsNullOrWhiteSpace(status) && status.ToUpper().Equals("SUDAH COMPLETE") ? true : false;
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from a in DbContext.MaterialsRequestNotes
                         join b in DbContext.MaterialsRequestNote_Items on a.Id equals b.MaterialsRequestNoteId
                         where a._IsDeleted == false
                             && a.Code == (string.IsNullOrWhiteSpace(materialsRequestNoteCode) ? a.Code : materialsRequestNoteCode)
                             && a.UnitId == (string.IsNullOrWhiteSpace(unitId) ? a.UnitId : unitId)
                             && b.ProductionOrderId == (string.IsNullOrWhiteSpace(productionOrderId) ? b.ProductionOrderId : productionOrderId)
                             && b.ProductId == (string.IsNullOrWhiteSpace(productId) ? b.ProductId : productId)
                             && b.ProductionOrderIsCompleted == (string.IsNullOrWhiteSpace(status) ? b.ProductionOrderIsCompleted : IsCompleted)
                             && a._CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                             && a._CreatedUtc.AddHours(offset).Date <= DateTo.Date
                         select new MaterialsRequestNoteReportViewModel
                         {
                             Code = a.Code,
                             CreatedDate = a._CreatedUtc,
                             OrderNo = b.ProductionOrderNo,
                             ProductName = b.ProductName,
                             Grade = b.Grade,
                             OrderQuantity = b.OrderQuantity,
                             Length = b.Length,
                             DistributedLength = b.DistributedLength,
                             Status = b.ProductionOrderIsCompleted,
                             Remark = a.Remark,
                         });

            return Query;
        }

        public Tuple<List<MaterialsRequestNoteReportViewModel>, int> GetReport(string materialsRequestNoteCode, string productionOrderId, string unitId, string productId, string status, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(materialsRequestNoteCode, productionOrderId, unitId, productId, status, dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.CreatedDate);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<MaterialsRequestNoteReportViewModel> pageable = new Pageable<MaterialsRequestNoteReportViewModel>(Query, page - 1, size);
            List<MaterialsRequestNoteReportViewModel> Data = pageable.Data.ToList<MaterialsRequestNoteReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public void CreateInventoryDocument(MaterialsRequestNote Model, string Type)
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

            List<MaterialsRequestNote_Item> list = Model.MaterialsRequestNote_Items
            .GroupBy(m => new { m.ProductId, m.ProductCode, m.ProductName })
            .Select(s => new MaterialsRequestNote_Item
            {
                ProductId = s.First().ProductId,
                ProductCode = s.First().ProductCode,
                ProductName = s.First().ProductName,
                Length = s.Sum(d => d.Length)
            }).ToList();

            InventoryDocumentItemViewModel inventoryDocumentItem = new InventoryDocumentItemViewModel();
            foreach (MaterialsRequestNote_Item item in list)
            {
                inventoryDocumentItem.productId = item.ProductId;
                inventoryDocumentItem.productCode = item.ProductCode;
                inventoryDocumentItem.productName = item.ProductName;
                inventoryDocumentItem.quantity = item.Length;
                inventoryDocumentItem.uomId = uom["_id"].ToString();
                inventoryDocumentItem.uom = uom["unit"].ToString();
                inventoryDocumentItems.Add(inventoryDocumentItem);
            }

            InventoryDocumentViewModel inventoryDocument = new InventoryDocumentViewModel
            {
                date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                referenceNo = Model.Code,
                referenceType = "Surat Permintaan Barang",
                type = Type,
                storageId = storage["_id"].ToString(),
                storageCode = storage["code"].ToString(),
                storageName = storage["name"].ToString(),
                items = inventoryDocumentItems
            };

            var response = httpClient.PostAsync($"{APIEndpoint.Inventory}{inventoryDocumentURI}", new StringContent(JsonConvert.SerializeObject(inventoryDocument).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            response.EnsureSuccessStatusCode();
        }
    }
}
