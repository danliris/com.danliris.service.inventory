using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models
{
    public class FpRegradingResultDocsDetails : StandardEntity, IValidatableObject
    {
        public int FpReturProInvDocsId { get; set; } // header Id as foreign key
        //public string SupplierId { get; set; } // header Id supplier as foreign key
        public string Code { get; set; }
        public string ProductName { get; set; }
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public double Quantity { get; set; }
        public double Length { get; set; }
        //public double LengthBeforeReGrade { get; set; }
        public string Remark { get; set; }
        public string Grade { get; set; }
        //public string GradeBefore { get; set; }
        public string Retur { get; set; }
        public virtual FpRegradingResultDocs FpReturProInvDocs { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
