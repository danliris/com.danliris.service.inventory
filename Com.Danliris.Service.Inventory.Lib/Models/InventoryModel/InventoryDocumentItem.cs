using Com.Danliris.Service.Inventory.Lib.MongoModels;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.InventoryModel
{
    public class InventoryDocumentItem : StandardEntity, IValidatableObject
    {
        public InventoryDocumentItem()
        {

        }

        public InventoryDocumentItem(InventoryDocumentItemMongo mongoInventoryDocumentItem)
        {
            Active = mongoInventoryDocumentItem._active;
            ProductCode = mongoInventoryDocumentItem.productCode;
            ProductName = mongoInventoryDocumentItem.productName;
            ProductRemark = mongoInventoryDocumentItem.remark;
            Quantity = mongoInventoryDocumentItem.quantity;
            StockPlanning = mongoInventoryDocumentItem.stockPlanning;
            UomUnit = mongoInventoryDocumentItem.uom;
            _CreatedAgent = mongoInventoryDocumentItem._createAgent;
            _CreatedBy = mongoInventoryDocumentItem._createdBy;
            _CreatedUtc = mongoInventoryDocumentItem._createdDate;
            _DeletedAgent = mongoInventoryDocumentItem._deleted ? mongoInventoryDocumentItem._updateAgent : "";
            _DeletedBy = mongoInventoryDocumentItem._deleted ? mongoInventoryDocumentItem._updatedBy : "";
            _DeletedUtc = mongoInventoryDocumentItem._deleted ? mongoInventoryDocumentItem._updatedDate : DateTime.MinValue;
            _IsDeleted = mongoInventoryDocumentItem._deleted;
            _LastModifiedAgent = mongoInventoryDocumentItem._updateAgent;
            _LastModifiedBy = mongoInventoryDocumentItem._updatedBy;
            _LastModifiedUtc = mongoInventoryDocumentItem._updatedDate;
        }

        /* Product */
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public string UomUnit { get; set; }
        public int UomId { get; set; }

        public double Quantity { get; set; }
        public double StockPlanning { get; set; }
        public string ProductRemark { get; set; }

        public int MongoIndexItem { get; set; }

        public int InventoryDocumentId { get; set; }
        public virtual InventoryDocument InventoryDocument { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}

