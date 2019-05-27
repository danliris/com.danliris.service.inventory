using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels
{
    public class ProductionOrderSQLViewModel
    {
        public long Id { get; set; }
        public string OrderNo { get; set; }
        public double? OrderQuantity { get; set; }
        public bool IsCompleted { get; set; }
        public double? DistributedQuantity { get; set; }
        public OrderTypeViewModel OrderType { get; set; }
    }
}
