using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report
{
    public class ExpenditureFInishedGoodReportViewModel : BasicViewModel
    {
        public int index { get; set; }
        public string FinishedGoodExpenditureNo { get; set; }
        public string ExpenditureTo { get; set; }
        public string ExpenditureDestinationDesc { get; set; }
        public string LocalSalesNoteNo { get; set; }
        public UnitViewModel UnitFrom { get; set; }
        public DateTimeOffset ExpenditureDate { get; set; }

        public string Consignment { get; set; }
        public string LeftoverComodityCode { get; set; }
        public string UnitComodityCode { get; set; }
        public string LeftoverComodityName { get; set; }
        public string RONo { get; set; }
        public double ExpenditureQuantity { get; set; }
        public string Uom { get; set; }
        public double Price { get; set; }

    }
}
