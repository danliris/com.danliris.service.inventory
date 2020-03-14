using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricViewModels
{
    public class GarmentLeftoverWarehouseReceiptFabricViewModel : BasicViewModel, IValidatableObject
    {
        public string ReceiptNoteNo { get; set; }

        public UnitViewModel UnitFrom { get; set; }

        public long UENId { get; set; }
        public string UENNo { get; set; }

        public StorageViewModel StorageFrom { get; set; }

        public DateTimeOffset ExpenditureDate { get; set; }
        public DateTimeOffset ReceiptDate { get; set; }

        public string Remark { get; set; }

        public List<GarmentLeftoverWarehouseReceiptFabricItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UnitFrom == null)
            {
                yield return new ValidationResult("Unit Asal tidak boleh kosong", new List<string> { "UnitFrom" });
            }

            if (string.IsNullOrWhiteSpace(UENNo))
            {
                yield return new ValidationResult("No Bon Pengeluaran Unit tidak boleh kosong", new List<string> { "UENNo" });
            }

            if (ReceiptDate <= DateTimeOffset.MinValue)
            {
                yield return new ValidationResult("Tanggal Penerimaan tidak boleh kosong", new List<string> { "ReceiptDate" });
            }

            if (Items == null || Items.Count < 1)
            {
                yield return new ValidationResult("Items tidak boleh kosong", new List<string> { "ItemsCount" });
            }
            else
            {
                foreach (var item in Items)
                {
                    int errorCount = 0;

                    if (item.Product == null)
                    {

                    }

                    if (errorCount > 0)
                    {
                        yield return new ValidationResult("Validasi", new List<string> { "Items" });
                    }
                }
            }

            yield return new ValidationResult("Validasi", new List<string> { "Validasi" });
        }
    }
}
