using Com.Danliris.Service.Inventory.Lib.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.Linq;
using Newtonsoft.Json;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialsRequestNoteServices;
using System.Net.Http;
using System.Net.Http.Headers;
using Com.Danliris.Service.Inventory.Lib.Facades.InventoryFacades;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.Interfaces.MaterialRequestNote;
using Com.Danliris.Service.Inventory.Lib.Services;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialsRequestNoteViewModel
{
    public class MaterialsRequestNoteViewModel : BasicViewModel, IValidatableObject
    {
        public int AutoIncrementNumber { get; set; }
        public string Code { get; set; }
        public UnitViewModel Unit { get; set; }
        public string RequestType { get; set; }
        public string Remark { get; set; }
        public bool IsDistributed { get; set; }
        public bool IsCompleted { get; set; }
        public List<MaterialsRequestNote_ItemViewModel> MaterialsRequestNote_Items { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Unit == null || Unit.Id == 0)
                yield return new ValidationResult("Unit harus diisi", new List<string> { "UnitId" });
            if (string.IsNullOrWhiteSpace(this.RequestType))
                yield return new ValidationResult("Jenis Request harus diisi", new List<string> { "RequestType" });

            int Count = 0;
            string materialsRequestNote_ItemsError = "[";

            if (this.MaterialsRequestNote_Items == null || this.MaterialsRequestNote_Items.Count.Equals(0))
                yield return new ValidationResult("Tabel dibawah harus diisi", new List<string> { "MaterialsRequestNote_Items" });
            else
            {
                foreach (MaterialsRequestNote_ItemViewModel materialsRequestNote_Item in this.MaterialsRequestNote_Items)
                {
                    materialsRequestNote_ItemsError += "{";
                    if (materialsRequestNote_Item.Product == null || materialsRequestNote_Item.Product.Id == 0)
                    {
                        Count++;
                        materialsRequestNote_ItemsError += "ProductId: 'Barang harus diisi', ";
                    }
                    else if (materialsRequestNote_Item.Product.Name != null || string.IsNullOrWhiteSpace(materialsRequestNote_Item.Product.Name))
                    {
                        string inventorySummaryURI = "inventory/inventory-summaries?order=%7B%7D&page=1&size=1000000000&";

                        IMaterialsRequestNote Service = (IMaterialsRequestNote)validationContext.GetService(typeof(IMaterialsRequestNote));
                        //IdentityService identity = (IdentityService)validationContext.GetService(typeof(IdentityService));
                        //HttpClient httpClient = new HttpClient();
                        //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", identity.Token);

                        List<string> products = new List<string>();
                        foreach (MaterialsRequestNote_ItemViewModel item in this.MaterialsRequestNote_Items)
                        {

                            products.Add(item.Product.Code);
                        }

                        var storageName = this.Unit.Name.Equals("PRINTING") ? "Gudang Greige Printing" : "Gudang Greige Finishing";

                        Dictionary<string, object> filter = new Dictionary<string, object> { { "StorageName", storageName }, { "UomUnit", "MTR" } };//, { "ProductCode", new Dictionary<string, object> { { "$in", products.ToArray() } } } };
                        InventorySummaryFacade InventorySummaryFacade = (InventorySummaryFacade)validationContext.GetService(typeof(InventorySummaryFacade));
                        List<string> inventorySummaries = InventorySummaryFacade.GetProductCodeForMaterialRequestNote(storageName);

                        //var response = httpClient.GetAsync($@"{APIEndpoint.Inventory}{inventorySummaryURI}filter=" + JsonConvert.SerializeObject(filter)).Result.Content.ReadAsStringAsync();
                        //Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);

                        //var json = result.Single(p => p.Key.Equals("data")).Value;
                        //List<InventorySummaryViewModel> inventorySummaries = JsonConvert.DeserializeObject<List<InventorySummaryViewModel>>(json.ToString());

                        if (!(inventorySummaries.Count.Equals(0)))
                        {
                            var inventorySummary = inventorySummaries.FirstOrDefault(p => products.Contains(p));
                            if (inventorySummary == null)
                            {
                                Count++;
                                materialsRequestNote_ItemsError += "ProductId: 'Barang tidak ada pada inventori summeries', ";
                            }
                        }
                        else
                        {
                            Count++;
                            materialsRequestNote_ItemsError += "ProductId: 'Barang tidak ada pada inventori summeries', ";
                        }
                    }
                    if (string.IsNullOrWhiteSpace(materialsRequestNote_Item.Grade))
                    {
                        Count++;
                        materialsRequestNote_ItemsError += "Grade: 'Grade harus diisi', ";
                    }
                    if (materialsRequestNote_Item.Length == null)
                    {
                        Count++;
                        materialsRequestNote_ItemsError += "Length: 'Panjang harus diisi', ";
                    }
                    else if (materialsRequestNote_Item.Length <= 0)
                    {
                        Count++;
                        materialsRequestNote_ItemsError += "Length: 'Panjang harus lebih besar dari 0', ";
                    }

                    if (!string.Equals(this.RequestType.ToUpper(), "TEST") && !string.Equals(this.RequestType.ToUpper(), "PEMBELIAN"))
                    {
                        if (materialsRequestNote_Item.ProductionOrder != null && materialsRequestNote_Item.Length > (materialsRequestNote_Item.ProductionOrder.OrderQuantity * 1.05))
                        {
                            Count++;
                            materialsRequestNote_ItemsError += "Length: 'Panjang total tidak boleh lebih dari 105% toleransi', ";
                        }
                        if (materialsRequestNote_Item.ProductionOrder == null || materialsRequestNote_Item.ProductionOrder.Id == 0)
                        {
                            Count++;
                            materialsRequestNote_ItemsError += "ProductionOrderId: 'Surat Perintah Produksi harus diisi', ";
                        }
                        if (materialsRequestNote_Item.ProductionOrder != null)
                        {
                            int count = MaterialsRequestNote_Items
                                .Count(c => c.ProductionOrder.Id == materialsRequestNote_Item.ProductionOrder.Id);

                            if (count > 1)
                            {
                                Count++;
                                materialsRequestNote_ItemsError += "ProductionOrderId: 'Surat Perintah Produksi tidak boleh duplikat', ";
                            }
                        }
                    }
                    materialsRequestNote_ItemsError += "}, ";
                }
            }
            materialsRequestNote_ItemsError += "]";
            if (Count > 0)
            {
                yield return new ValidationResult(materialsRequestNote_ItemsError, new List<string> { "MaterialsRequestNote_Items" });
            }
        }
    }
}
