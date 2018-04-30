using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.FPReturnInvToPurchasingModel
{
    public class FPReturnInvToPurchasingDetail : StandardEntity, IValidatableObject
    {
        public int FPReturnInvToPurchasingId { get; set; }
        public int FPRegradingResultDocsId { get; set; }
        public string FPRegradingResultDocsCode { get; set; }
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public double NecessaryLength { get; set; }
        public double Length { get; set; }
        public string Description { get; set; }
        public virtual FPReturnInvToPurchasing FPReturnInvToPurchasing { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
