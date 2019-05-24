using Com.Danliris.Service.Inventory.Lib.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialsRequestNoteViewModel
{
    public class MaterialsRequestNote_ItemViewModel : BasicViewModel, IValidatableObject
    {
        public ProductionOrderViewModel ProductionOrder { get; set; }
        public ProductViewModel Product { get; set; }
        public double? Length { get; set; }
        public double? DistributedLength { get; set; }
        public string Grade { get; set; }
        public string Remark { get; set; }
        public string ProductionOrderId { get; set; }
        public string ProductionOrderNo { get; set; }
        public double? ProductionOrderQuantity { get; set; }
        public bool ProductionOrderIsCompleted { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
