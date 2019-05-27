using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel
{
    public class InventoryDocumentViewModel : BasicViewModel, IValidatableObject
    {
        public string no { get; set; }
        public string code { get; set; }
        public DateTimeOffset date { get; set; }
        public string referenceNo { get; set; }
        public string referenceType { get; set; }
        public string type { get; set; }
        public int storageId { get; set; }
        public string storageCode { get; set; }
        public string storageName { get; set; }
        public List<InventoryDocumentItemViewModel> items { get; set; }
        public string remark { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (storageId==0)
                yield return new ValidationResult("Gudang harus diisi", new List<string> { "storageId" });
            if (this.referenceNo == null || string.IsNullOrWhiteSpace(this.referenceNo))
                yield return new ValidationResult("No. referensi harus diisi", new List<string> { "referenceNo" });
            if (this.referenceType == null || string.IsNullOrWhiteSpace(this.referenceType))
                yield return new ValidationResult("No. referensi harus diisi", new List<string> { "referenceType" });
            if (this.date.Equals(DateTimeOffset.MinValue) || this.date == null)
            {
                yield return new ValidationResult("Tanggal Harus diisi", new List<string> { "date" });
            }
            int itemErrorCount = 0;

            if (this.items.Count.Equals(0))
            {
                yield return new ValidationResult("Tabel Item Harus diisi", new List<string> { "itemscount" });
            }
            else
            {
                string itemError = "[";

                foreach (InventoryDocumentItemViewModel item in items)
                {
                    itemError += "{";

                    if (item.productId==0)
                    {
                        itemErrorCount++;
                        itemError += "productId: 'Barang harus diisi', ";
                    }
                    //else
                    //{
                    //    var itemsExist = items.Where(i => i.productId != null && i.uomId != null && i.productId.Equals(item.productId) && i.uomId.Equals(item.uomId)).Count();
                    //    if (itemsExist > 1)
                    //    {
                    //        itemErrorCount++;
                    //        itemError += "productId: 'Barang dan Satuan sudah digunakan', ";
                    //    }
                    //}

                    if (type=="ADJ" && item.quantity == 0)
                    {
                        itemErrorCount++;
                        itemError += "quantity: 'Jumlah adjust tidak boleh = 0',";
                    }
                    if (type != "ADJ" && item.quantity <= 0)
                    {
                        itemErrorCount++;
                        itemError += "quantity: 'Jumlah harus lebih dari 0',";
                    }
                    if (item.uom == null || item.uomId==0)
                    {
                        itemErrorCount++;
                        itemError += "uomId: 'Satuan harus diisi', ";
                    }

                    itemError += "}, ";
                }

                itemError += "]";

                if (itemErrorCount > 0)
                    yield return new ValidationResult(itemError, new List<string> { "items" });
            }
        }
    }
}
