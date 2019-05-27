using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Moonlay.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel
{
    public class MaterialsRequestNote : StandardEntity, IValidatableObject
    {
        public string Code { get; set; }
        public string UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string Remark { get; set; }
        public string RequestType { get; set; }
        public string Type { get; set; }
        public bool IsDistributed { get; set; }
        public bool IsCompleted { get; set; }
        public int AutoIncrementNumber { get; set; }

        public int UnitIntegerId { get; set; }

        public virtual ICollection<MaterialsRequestNote_Item> MaterialsRequestNote_Items { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
