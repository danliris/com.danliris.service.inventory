using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel
{
    public class MaterialsRequestNote_Item : StandardEntity, IValidatableObject
    {
        public int MaterialsRequestNoteId { get; set; }
        public virtual MaterialsRequestNote MaterialsRequestNote { get; set; }
        public string Code { get; set; }
        public string ProductionOrderId { get; set; }
        public string ProductionOrderNo { get; set; }
        public bool ProductionOrderIsCompleted { get; set; }
        public double OrderQuantity { get; set; }
        public string OrderTypeId { get; set; }
        public string OrderTypeCode { get; set; }
        public string OrderTypeName { get; set; }
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Grade { get; set; }
        public double DistributedLength { get; set; }
        public double Length { get; set; }
        public string Remark { get; set; }

        public int ProductIntegerId { get; set; }
        public long ProductionOrderLongId { get; set; }
        public int OrderTypeIntegerId { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
