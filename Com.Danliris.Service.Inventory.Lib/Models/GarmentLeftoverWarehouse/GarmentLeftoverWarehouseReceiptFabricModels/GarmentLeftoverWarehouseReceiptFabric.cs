using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricModels
{
    public class GarmentLeftoverWarehouseReceiptFabric : StandardEntity, IValidatableObject
    {
        public string ReceiptNoteNo { get; set; }

        public long UnitFromId { get; set; }
        public string UnitFromCode { get; set; }
        public string UnitFromName { get; set; }

        public long UENId { get; set; }
        public string UENNo { get; set; }

        public long StorageFromId { get; set; }
        public string StorageFromCode { get; set; }
        public string StorageFromName { get; set; }

        public DateTimeOffset ExpenditureDate { get; set; }
        public DateTimeOffset ReceiptDate { get; set; }

        public string Remark { get; set; }

        public virtual ICollection<GarmentLeftoverWarehouseReceiptFabricItem> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
