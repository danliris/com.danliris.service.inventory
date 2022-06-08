using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Mutation
{
    public class GarmentLeftoverWarehouseMutationReportViewModel
    {
        public string ClassificationCode { get; set; }
        public string ClassificationName { get; set; }
        public string UnitQtyName { get; set; }
        public string Productname { get; set; }
        public double SaldoAwal { get; set; }
        public double Pemasukan { get; set; }
        public double Pengeluaran { get; set; }
        public double Penyesuaian { get; set; }
        public double StockOpname { get; set; }
        public double Selisih { get; set; }
        public double SaldoAkhir { get; set; }
    }
}
