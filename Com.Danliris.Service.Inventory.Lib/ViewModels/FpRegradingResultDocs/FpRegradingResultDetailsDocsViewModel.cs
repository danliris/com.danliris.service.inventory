using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels
{
    public class FpRegradingResultDetailsDocsViewModel : BasicViewModel, IValidatableObject
    {
        public product Product { get; set; }
        public double Quantity { get; set; }
        public double Length { get; set; }
        public double LengthBeforeReGrade { get; set; }
        public string Remark { get; set; }
        public string Grade { get; set; }
        public string GradeBefore { get; set; }
        public string Retur { get; set; }

        public class product
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public double Length { get; set; }
            //public double Quantity { get; set; }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
