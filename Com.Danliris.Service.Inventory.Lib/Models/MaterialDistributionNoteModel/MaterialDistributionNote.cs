using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel
{
    public class MaterialDistributionNote : StandardEntity, IValidatableObject
    {
        public string No { get; set; }
        public string UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string Type { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDisposition { get; set; }
        public int AutoIncrementNumber { get; set; }

        public int UnitIntegerId { get; set; }

        public virtual ICollection<MaterialDistributionNoteItem> MaterialDistributionNoteItems { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
