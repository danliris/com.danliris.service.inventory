using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Interfaces;
using Com.Danliris.Service.Inventory.Lib.Models;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryDocumentViewModel;
using Com.Moonlay.NetCore.Lib;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;



namespace Com.Danliris.Service.Inventory.Lib.Services
{
    public class FpRegradingResultDocsService : BasicService<InventoryDbContext, FpRegradingResultDocs>, IMap<FpRegradingResultDocs, FpRegradingResultDocsViewModel>
    {
        public FpRegradingResultDocsService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public FpRegradingResultDocs MapToModel(FpRegradingResultDocsViewModel viewModel)
        {
            FpRegradingResultDocs model = new FpRegradingResultDocs();
            PropertyCopier<FpRegradingResultDocsViewModel, FpRegradingResultDocs>.Copy(viewModel, model);

            model.NoBon = viewModel.Bon.no;
            model.NoBonId = viewModel.Bon._id;
            model.UnitName = viewModel.Bon.unitName;
            model.SupplierId = viewModel.Supplier._id;
            model.SupplierName = viewModel.Supplier.name;
            model.SupplierCode = viewModel.Supplier.code;
            model.ProductId = viewModel.Product.Id;
            model.ProductCode = viewModel.Product.Code;
            model.ProductName = viewModel.Product.Name;
            model.Shift = viewModel.Shift;
            model.MachineName = viewModel.Machine.name;
            model.MachineId = viewModel.Machine._id;
            model.MachineCode = viewModel.Machine.code;
            model.Operator = viewModel.Operator;
            model.Remark = viewModel.Remark;

            model.Details = new List<FpRegradingResultDocsDetails>();

            foreach (FpRegradingResultDetailsDocsViewModel data in viewModel.Details)
            {
                FpRegradingResultDocsDetails detail = new FpRegradingResultDocsDetails();
                //detail.SupplierId = viewModel.Supplier._id;
                detail.ProductId = viewModel.Product.Id;
                detail.ProductCode = viewModel.Product.Code;
                detail.ProductName = viewModel.Product.Name;
                detail.Quantity = data.Quantity;
                detail.Length = data.Length;
                detail.Remark = data.Remark;
                detail.Grade = data.Grade;
                //detail.GradeBefore = data.GradeBefore;
                detail.Retur = data.Retur;
                //detail.LengthBeforeReGrade = data.LengthBeforeReGrade;
                model.Details.Add(detail);

            }

            return model;
        }

        public FpRegradingResultDocsViewModel MapToViewModel(FpRegradingResultDocs model)
        {
            FpRegradingResultDocsViewModel viewModel = new FpRegradingResultDocsViewModel();
            PropertyCopier<FpRegradingResultDocs, FpRegradingResultDocsViewModel>.Copy(model, viewModel);

            viewModel.Details = new List<FpRegradingResultDetailsDocsViewModel>();
            viewModel.Bon = new FpRegradingResultDocsViewModel.noBon();
            viewModel.Bon._id = model.NoBonId;
            viewModel.Bon.no = model.NoBon;
            viewModel.Bon.unitName = model.UnitName;

            viewModel.Supplier = new FpRegradingResultDocsViewModel.supplier();
            viewModel.Supplier.name = model.SupplierName;
            viewModel.Supplier._id = model.SupplierId;
            viewModel.Supplier.code = model.SupplierCode;
            viewModel._CreatedUtc = model._CreatedUtc;

            viewModel.Product = new FpRegradingResultDocsViewModel.product();
            viewModel.Product.Name = model.ProductName;
            viewModel.Product.Id = model.ProductId;
            viewModel.Product.Code = model.SupplierCode;

            viewModel.Shift = model.Shift;
            viewModel.Remark = model.Remark;
            viewModel.Operator = model.Operator;

            viewModel.Machine = new FpRegradingResultDocsViewModel.machine();
            viewModel.Machine.name = model.MachineName;
            viewModel.Machine.code = model.MachineCode;
            viewModel.Machine._id = model.MachineId;

            foreach (FpRegradingResultDocsDetails data in model.Details)
            {
                FpRegradingResultDetailsDocsViewModel detail = new FpRegradingResultDetailsDocsViewModel();
                detail.Product = new FpRegradingResultDetailsDocsViewModel.product();
                detail.Product.Id = data.ProductId;
                detail.Product.Name = data.ProductName;
                detail.Product.Length = data.Length;
                //detail.Product.Quantity = data.Quantity;
                //detail.Quantity = data.Quantity;
                detail.Remark = data.Remark;
                detail.Quantity = data.Quantity;
                detail.Length = data.Length;
                //detail.GradeBefore = data.GradeBefore;
                detail.Grade = data.Grade;
                detail.Retur = data.Retur;
                //detail.LengthBeforeReGrade = data.LengthBeforeReGrade;
                viewModel.Details.Add(detail);
            }
            return viewModel;
        }

        public override Tuple<List<FpRegradingResultDocs>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {

            IQueryable<FpRegradingResultDocs> Query = this.DbContext.fpRegradingResultDocs;

            List<string> SearchAttributes = new List<string>()
                {
                    "Code", "NoBon", "SupplierName","ProductName"
                };
            Query = ConfigureSearch(Query, SearchAttributes, Keyword);

            List<string> SelectedFields = new List<string>()
                {
                    "Id", "Code", "Date", "Bon", "Supplier", "Product", "Details", "Machine", "Remark", "Operator", "IsReturnedToPurchasing"
                };
            Query = Query
                .Select(o => new FpRegradingResultDocs
                {
                    Id = o.Id,
                    Code = o.Code,
                    Date = o.Date,
                    NoBon = o.NoBon,
                    NoBonId = o.NoBonId,
                    UnitName = o.UnitName,
                    SupplierId = o.SupplierId,
                    SupplierName = o.SupplierName,
                    SupplierCode = o.SupplierCode,
                    ProductId = o.ProductId,
                    ProductName = o.ProductName,
                    ProductCode = o.ProductCode,
                    Remark = o.Remark,
                    Operator = o.Operator,
                    MachineCode = o.MachineCode,
                    MachineId = o.MachineId,
                    MachineName = o.MachineName,
                    Shift = o.Shift,
                    _CreatedUtc = o._CreatedUtc,
                    _LastModifiedUtc=o._LastModifiedUtc,
                    IsReturnedToPurchasing=o.IsReturnedToPurchasing,
                    Details = o.Details.Select(p => new FpRegradingResultDocsDetails { FpReturProInvDocsId = p.FpReturProInvDocsId, ProductName = p.ProductName, ProductCode = p.ProductCode, ProductId = p.ProductId, Id = o.Id, Length = p.Length, Quantity = p.Quantity, Remark = p.Remark, Code = p.Code, Grade = p.Grade, Retur = p.Retur }).Where(i => i.FpReturProInvDocsId.Equals(o.Id)).ToList()
                });

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = ConfigureOrder(Query, OrderDictionary);

            Pageable<FpRegradingResultDocs> pageable = new Pageable<FpRegradingResultDocs>(Query, Page - 1, Size);
            List<FpRegradingResultDocs> Data = pageable.Data.ToList<FpRegradingResultDocs>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }
        public override async Task<FpRegradingResultDocs> ReadModelById(int id)
        {
            return await this.DbSet
                .Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                .Include(d => d.Details)
                .FirstOrDefaultAsync();
        }

        public void CreateInventoryDocument(FpRegradingResultDocs Model, string Type)
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
            InventoryDocumentItemViewModel inventoryDocumentItem = new InventoryDocumentItemViewModel();
            double TotalLength = 0;
            foreach (FpRegradingResultDocsDetails o in Model.Details)
            {
                TotalLength += o.Length;
                //inventoryDocumentItem = new InventoryDocumentItemViewModel
                //{
                //productId = o.ProductId,
                //productCode = o.ProductCode,
                //productName = o.ProductName,
                //quantity = o.Length,
                //uomId = uom["_id"].ToString(),
                //uom = uom["unit"].ToString()
                //};
                //inventoryDocumentItems.Add(inventoryDocumentItem);
            }

            inventoryDocumentItem.productId = Model.ProductId;
            inventoryDocumentItem.productCode = Model.ProductCode;
            inventoryDocumentItem.productName = Model.ProductName;
            inventoryDocumentItem.quantity = TotalLength;
            inventoryDocumentItem.uomId = uom["_id"].ToString();
            inventoryDocumentItem.uom = uom["unit"].ToString();

            inventoryDocumentItems.Add(inventoryDocumentItem);

            InventoryDocumentViewModel inventoryDocument = new InventoryDocumentViewModel
            {
                date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                referenceNo = Model.Code,
                referenceType = "Bon Hasil Re-grading",
                type = Type,
                storageId = storage["_id"].ToString(),
                storageCode = storage["code"].ToString(),
                storageName = storage["name"].ToString(),
                items = inventoryDocumentItems
            };

            var response = httpClient.PostAsync($"{APIEndpoint.Inventory}{inventoryDocumentURI}", new StringContent(JsonConvert.SerializeObject(inventoryDocument).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            response.EnsureSuccessStatusCode();
        }

        public async Task<FpRegradingResultDocs> CustomCodeGenerator(FpRegradingResultDocs Model)
        {
            var unit = string.Equals(Model.UnitName.ToUpper(), "PRINTING") ? "PR" : "FS";
            var lastData = await this.DbSet.Where(w => w.UnitName == Model.UnitName).OrderByDescending(o => o._CreatedUtc).FirstOrDefaultAsync();

            DateTime Now = DateTime.Now;
            string Year = Now.ToString("yy");
            //string Month = Now.ToString("MM");

            if (lastData == null)
            {
                Model.AutoIncrementNumber = 1;
                string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                Model.Code = $"KB{unit}{Year}{Number}";
            }
            else
            {
                if (lastData._CreatedUtc.Year < Now.Year)
                {
                    Model.AutoIncrementNumber = 1;
                    string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                    Model.Code = $"KB{unit}{Year}{Number}";
                }
                else
                {
                    Model.AutoIncrementNumber = lastData.AutoIncrementNumber + 1;
                    string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
                    Model.Code = $"KB{unit}{Year}{Number}";
                }
            }

            return Model;
        }

        public override async Task<int> CreateModel(FpRegradingResultDocs Model)
        {
            int Created = 0;
            using (var transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    Model = await this.CustomCodeGenerator(Model);
                    Created = await this.CreateAsync(Model);
                    CreateInventoryDocument(Model, "IN");

                    transaction.Commit();
                }
                catch (ServiceValidationExeption e)
                {
                    throw new ServiceValidationExeption(e.ValidationContext, e.ValidationResults);
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }
            return Created;
        }

        public override void OnCreating(FpRegradingResultDocs model)
        {
            //do
            //{
            //    model.Code = CodeGenerator.GenerateCode();
            //}
            //while (this.DbSet.Any(d => d.Code.Equals(model.Code)));
            try
            {
                base.OnCreating(model);
                model._CreatedAgent = "Service";
                model._CreatedBy = this.Username;
                model._LastModifiedAgent = "Service";
                model._LastModifiedBy = this.Username;

                FpRegradingResultDetailsDocsService fpReturProInvDocsDetailsService = ServiceProvider.GetService<FpRegradingResultDetailsDocsService>();
                fpReturProInvDocsDetailsService.Username = this.Username;
                foreach (FpRegradingResultDocsDetails data in model.Details)
                {

                    fpReturProInvDocsDetailsService.OnCreating(data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }

        }

        public override async Task<int> DeleteModel(int Id)
        {
            int Count = 0;

            using (var Transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    FpRegradingResultDocs fpReturProInvDocs = await ReadModelById(Id);
                    Count = this.Delete(Id);


                    FpRegradingResultDetailsDocsService fpReturProInvDocsDetailsService = ServiceProvider.GetService<FpRegradingResultDetailsDocsService>();
                    fpReturProInvDocsDetailsService.Username = this.Username;


                    HashSet<int> fpReturProInvDocsDetails = new HashSet<int>(this.DbContext.fpRegradingResultDocsDetails.Where(p => p.FpReturProInvDocsId.Equals(Id)).Select(p => p.Id));

                    foreach (int detail in fpReturProInvDocsDetails)
                    {
                        await fpReturProInvDocsDetailsService.DeleteAsync(detail);
                    }

                    CreateInventoryDocument(fpReturProInvDocs, "OUT");

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

        public override void OnUpdating(int id, FpRegradingResultDocs model)
        {
            base.OnUpdating(id, model);
            model._LastModifiedAgent = "Service";
            model._LastModifiedBy = this.Username;
        }

        public override void OnDeleting(FpRegradingResultDocs model)
        {
            base.OnDeleting(model);
            model._DeletedAgent = "Service";
            model._DeletedBy = this.Username;
        }

        public Tuple<List<FpRegradingResultDocs>, int, Dictionary<string, string>, List<string>> ReadNo(string Keyword = null, string Filter = "{}", int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null)
        {
            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);

            IQueryable<FpRegradingResultDocs> Query = this.DbContext.fpRegradingResultDocs.Where(p => p._IsDeleted == false && p.IsReturnedToPurchasing == false && p.UnitName == FilterDictionary["UnitName"] && p.SupplierId == FilterDictionary["SupplierId"]);

            List<string> SearchAttributes = new List<string>()
            {
                "Code"
            };
            Query = ConfigureSearch(Query, SearchAttributes, Keyword);

            List<string> SelectedFields = new List<string>()
            {
                "Id", "Code"
            };

            Query = Query
                .Select(o => new FpRegradingResultDocs
                {
                    Id = o.Id,
                    Code = o.Code,
                    _LastModifiedUtc = o._LastModifiedUtc,
                    UnitName = o.UnitName,
                    SupplierId = o.SupplierId,
                    Details = new List<FpRegradingResultDocsDetails>()
                });

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = ConfigureOrder(Query, OrderDictionary);

            Pageable<FpRegradingResultDocs> pageable = new Pageable<FpRegradingResultDocs>(Query, Page - 1, Size);
            List<FpRegradingResultDocs> Data = pageable.Data.ToList<FpRegradingResultDocs>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public void UpdateIsReturnedToPurchasing(int fPRegradingResultDocsId, bool flag)
        {
            FpRegradingResultDocs model = new FpRegradingResultDocs { Id = fPRegradingResultDocsId, IsReturnedToPurchasing = flag };
 
            DbContext.fpRegradingResultDocs.Attach(model);
            DbContext.Entry(model).Property(x => x.IsReturnedToPurchasing).IsModified = true;
            DbContext.SaveChanges();
        }
    }
}
