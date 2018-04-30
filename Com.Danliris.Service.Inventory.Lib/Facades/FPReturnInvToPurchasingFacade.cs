using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Interfaces;
using Com.Danliris.Service.Inventory.Lib.Models;
using Com.Danliris.Service.Inventory.Lib.Models.FPReturnInvToPurchasingModel;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.FPReturnInvToPurchasingService;
using Com.Danliris.Service.Inventory.Lib.ViewModels.FPReturnInvToPurchasingViewModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryDocumentViewModel;
using Com.Moonlay.NetCore.Lib;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Facades
{
    public class FPReturnInvToPurchasingFacade : ICreateable<FPReturnInvToPurchasing>, IReadable, IReadByIdable<FPReturnInvToPurchasing>, IDeleteable
    {
        private readonly FPReturnInvToPurchasingService fpReturnInvToPurchasingService;
        private readonly FPReturnInvToPurchasingDetailService fpReturnInvToPurchasingDetailService;
        private readonly FpRegradingResultDocsService fpRegradingResultDocsService;
        public readonly IServiceProvider serviceProvider;

        public FPReturnInvToPurchasingFacade(IServiceProvider serviceProvider, FPReturnInvToPurchasingService fpReturnInvToPurchasingService, FPReturnInvToPurchasingDetailService fpReturnInvToPurchasingDetailService, FpRegradingResultDocsService fpRegradingResultDocsService)
        {
            this.serviceProvider = serviceProvider;
            this.fpReturnInvToPurchasingService = fpReturnInvToPurchasingService;
            this.fpReturnInvToPurchasingDetailService = fpReturnInvToPurchasingDetailService;
            this.fpRegradingResultDocsService = fpRegradingResultDocsService;
        }

        public async Task<int> Create(FPReturnInvToPurchasing model)
        {
            int Created = 0;

            using (var transaction = this.fpReturnInvToPurchasingService.DbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (FPReturnInvToPurchasingDetail detail in model.FPReturnInvToPurchasingDetails)
                    {
                        fpRegradingResultDocsService.UpdateIsReturnedToPurchasing(detail.FPRegradingResultDocsId, true);
                        this.fpReturnInvToPurchasingDetailService.OnCreating(detail);
                    }

                    model = await this.NoGenerator(model);
                    Created = await this.fpReturnInvToPurchasingService.CreateAsync(model);
                    CreateInventoryDocument(model, "OUT");

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
                    throw new Exception("Insert Error");
                }
            }

            return Created;
        }

        /*
        public async Task<int> Update(int id, FPReturnInvToPurchasing model)
        {
            int Updated = 0;

            using (var transaction = this.fpReturnInvToPurchasingService.DbContext.Database.BeginTransaction())
            {
                try
                {
                    HashSet<int> fpReturnInvToPurchasingDetails = new HashSet<int>(fpReturnInvToPurchasingDetailService.DbSet
                        .Where(w => w.FPReturnInvToPurchasingId.Equals(id))
                        .Select(s => s.Id));
                    Updated = await this.fpReturnInvToPurchasingService.UpdateAsync(id, model);

                    Updated = await this.fpReturnInvToPurchasingService.CreateAsync(model);
                    CreateInventoryDocument(model, "OUT");

                    foreach (int detailId in fpReturnInvToPurchasingDetails)
                    {
                        FPReturnInvToPurchasingDetail detailModel = model.FPReturnInvToPurchasingDetails.FirstOrDefault(prop => prop.Id.Equals(detailId));
                        if (model == null)
                        {
                            fpReturnInvToPurchasingDetailService.Delete(detailId);
                        }
                        else
                        {
                            fpReturnInvToPurchasingDetailService.Update(detailId, detailModel);
                        }
                    }

                    foreach (FPReturnInvToPurchasingDetail detailModel in model.FPReturnInvToPurchasingDetails)
                    {
                        if (detailModel.Id.Equals(0))
                        {
                            fpReturnInvToPurchasingDetailService.Create(detailModel);
                        }
                    }

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
                    throw new Exception("Update Error");
                }
            }

            return Updated;
        }
        */

        public Tuple<List<object>, int, Dictionary<string, string>> Read(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            #region Query

            IQueryable<FPReturnInvToPurchasing> Query = this.fpReturnInvToPurchasingService.DbSet;

            Query = Query
                .Select(s => new FPReturnInvToPurchasing
                {
                    Id = s.Id,
                    No = s.No,
                    UnitName = s.UnitName,
                    SupplierName = s.SupplierName,
                    _CreatedUtc = s._CreatedUtc,
                    _LastModifiedUtc = s._LastModifiedUtc,
                    FPReturnInvToPurchasingDetails = s.FPReturnInvToPurchasingDetails.Select(p => new FPReturnInvToPurchasingDetail { FPReturnInvToPurchasingId = p.FPReturnInvToPurchasingId, Quantity = p.Quantity, Length = p.Length, NecessaryLength = p.NecessaryLength }).Where(i => i.FPReturnInvToPurchasingId.Equals(s.Id)).ToList()
                });

            #endregion Query

            #region Search

            List<string> searchAttributes = new List<string>()
            {
                "No"
            };

            if (keyword != null)
            {
                Query = Query.Where(General.BuildSearch(searchAttributes), keyword);
            }

            #endregion Search

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

            Pageable<FPReturnInvToPurchasing> pageable = new Pageable<FPReturnInvToPurchasing>(Query, page - 1, size);
            List<FPReturnInvToPurchasing> Data = pageable.Data.ToList<FPReturnInvToPurchasing>();
            int TotalData = pageable.TotalCount;

            #endregion Paging

            List<object> list = new List<object>();
            list.AddRange(
                Data
                    .GroupBy(d => new { d.Id, d.No, d.UnitName, d._CreatedUtc, d.SupplierName })
                    .Select(s => new
                    {
                        Id = s.First().Id,
                        No = s.First().No,
                        UnitName = s.First().UnitName,
                        TotalQuantity = s.Sum(d => d.FPReturnInvToPurchasingDetails.Sum(p => p.Quantity)),
                        TotalLength = s.Sum(d => d.FPReturnInvToPurchasingDetails.Sum(p => p.Length)),
                        TotalNecessaryLength = s.Sum(d => d.FPReturnInvToPurchasingDetails.Sum(p => p.NecessaryLength)),
                        SupplierName = s.First().SupplierName,
                        _CreatedUtc = s.First()._CreatedUtc
                    }).ToList()
            );

            return Tuple.Create(list, TotalData, OrderDictionary);
        }

        public async Task<FPReturnInvToPurchasing> ReadById(int id)
        {
            return await this.fpReturnInvToPurchasingService.DbSet
                .Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                .Include(d => d.FPReturnInvToPurchasingDetails)
                .FirstOrDefaultAsync();
        }

        public async Task<int> Delete(int id)
        {
            int Count = 0;

            if (!this.fpReturnInvToPurchasingService.IsExists(id))
            {
                return 0;
            }

            using (var Transaction = this.fpReturnInvToPurchasingService.DbContext.Database.BeginTransaction())
            {
                try
                {
                    FPReturnInvToPurchasing fpReturnInvToPurchasing = await this.ReadById(id);
                    Count = await this.fpReturnInvToPurchasingService.DeleteAsync(id);

                    HashSet<object> fpReturnInvToPurchasingDetails = new HashSet<object>(this.fpReturnInvToPurchasingDetailService.DbSet.Where(p => p.FPReturnInvToPurchasingId.Equals(id)).Select(p => new { Id = p.Id, FPRegradingResultDocsId = p.FPRegradingResultDocsId }));
                    foreach (dynamic detail in fpReturnInvToPurchasingDetails)
                    {
                        fpRegradingResultDocsService.UpdateIsReturnedToPurchasing(detail.FPRegradingResultDocsId, false);
                        await this.fpReturnInvToPurchasingDetailService.DeleteAsync(detail.Id);
                    }

                    CreateInventoryDocument(fpReturnInvToPurchasing, "IN");

                    Transaction.Commit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    Transaction.Rollback();
                    throw new Exception("Update Error");
                }
            }

            return Count;
        }

        public void CreateInventoryDocument(FPReturnInvToPurchasing model, string Type)
        {
            string inventoryDocumentURI = "inventory/inventory-documents";
            string storageURI = "master/storages";
            string uomURI = "master/uoms";

            HttpClientService httpClient = (HttpClientService)this.serviceProvider.GetService(typeof(HttpClientService));

            #region UOM

            Dictionary<string, object> filterUOM = new Dictionary<string, object> { { "unit", "MTR" } };
            var responseUOM = httpClient.GetAsync($@"{APIEndpoint.Core}{uomURI}?filter=" + JsonConvert.SerializeObject(filterUOM)).Result.Content.ReadAsStringAsync();
            Dictionary<string, object> resultUOM = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseUOM.Result);
            var jsonUOM = resultUOM.Single(p => p.Key.Equals("data")).Value;
            Dictionary<string, object> uom = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonUOM.ToString())[0];

            #endregion UOM

            #region Storage

            var storageName = model.UnitName.Equals("PRINTING") ? "Gudang Greige Printing" : "Gudang Greige Finishing";
            Dictionary<string, object> filterStorage = new Dictionary<string, object> { { "name", storageName } };
            var responseStorage = httpClient.GetAsync($@"{APIEndpoint.Core}{storageURI}?filter=" + JsonConvert.SerializeObject(filterStorage)).Result.Content.ReadAsStringAsync();
            Dictionary<string, object> resultStorage = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseStorage.Result);
            var jsonStorage = resultStorage.Single(p => p.Key.Equals("data")).Value;
            Dictionary<string, object> storage = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonStorage.ToString())[0];

            #endregion Storage

            #region Inventory Document

            List<InventoryDocumentItemViewModel> inventoryDocumentItems = new List<InventoryDocumentItemViewModel>();

            foreach (FPReturnInvToPurchasingDetail detail in model.FPReturnInvToPurchasingDetails)
            {
                InventoryDocumentItemViewModel inventoryDocumentItem = new InventoryDocumentItemViewModel();

                inventoryDocumentItem.productId = detail.ProductId;
                inventoryDocumentItem.productCode = detail.ProductCode;
                inventoryDocumentItem.productName = detail.ProductName;
                inventoryDocumentItem.quantity = detail.Length;
                inventoryDocumentItem.uomId = uom["_id"].ToString();
                inventoryDocumentItem.uom = uom["unit"].ToString();

                inventoryDocumentItems.Add(inventoryDocumentItem);
            }

            InventoryDocumentViewModel inventoryDocument = new InventoryDocumentViewModel
            {
                date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                referenceNo = model.No,
                referenceType = "Bon Retur Barang - Pembelian",
                type = Type,
                storageId = storage["_id"].ToString(),
                storageCode = storage["code"].ToString(),
                storageName = storage["name"].ToString(),
                items = inventoryDocumentItems
            };

            var response = httpClient.PostAsync($"{APIEndpoint.Inventory}{inventoryDocumentURI}", new StringContent(JsonConvert.SerializeObject(inventoryDocument).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            response.EnsureSuccessStatusCode();

            #endregion Inventory Document
        }

        public async Task<FPReturnInvToPurchasing> NoGenerator(FPReturnInvToPurchasing Model)
        {
            var unit = string.Equals(Model.UnitName.ToUpper(), "PRINTING") ? "PR" : "FS";
            var lastData = await this.fpReturnInvToPurchasingService.DbSet.Where(w => w.UnitName == Model.UnitName).OrderByDescending(o => o._CreatedUtc).FirstOrDefaultAsync();

            DateTime Now = DateTime.Now;
            string Year = Now.ToString("yy");

            if (lastData == null)
            {
                Model.AutoIncrementNumber = 1;
                string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                Model.No = $"BL{unit}{Year}{Number}";
            }
            else
            {
                if (lastData._CreatedUtc.Year < Now.Year)
                {
                    Model.AutoIncrementNumber = 1;
                    string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                    Model.No = $"BL{unit}{Year}{Number}";
                }
                else
                {
                    Model.AutoIncrementNumber = lastData.AutoIncrementNumber + 1;
                    string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                    Model.No = $"BL{unit}{Year}{Number}";
                }
            }

            return Model;
        }
    }
}
