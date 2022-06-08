using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Expenditure
{
    public class GarmentLeftoverWarehouseReportExpenditureViewModel : BasicViewModel
    {
        public string ExpenditureNo { get; set; }
        public DateTimeOffset ExpenditureDate { get; set; }
        public string ExpenditureDestination { get; set; }
        public string DescriptionOfPurpose { get; set; }
        public BuyerViewModel Buyer { get; set; }
        public UnitViewModel UnitExpenditure { get; set; }
        public string EtcRemark { get; set; }
        public string PONo { get; set; }
        public ProductViewModel Product { get; set; }
        public string ProductRemark { get; set; }
        public double Quantity { get; set; }
        public UomViewModel Uom { get; set; }
        public UnitViewModel UnitFrom { get; set; }
        public string LocalSalesNoteNo { get; set; }
        public string BCNo { get; set; }
        public string BCType { get; set; }
        public DateTimeOffset? BCDate { get; set; }
        public double QtyKG { get; set; }
        public string Composition { get; set; }
        public string Const { get; set; }
        public double Price { get; set; }

    }
}
