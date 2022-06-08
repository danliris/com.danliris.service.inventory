using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels
{
    public class UnitDoViewModel
    {
        public string ProductCode { get; set; }
        public string POSerialNumber { get; set; }
        public string ProductName { get; set; }
        public string Rono { get; set; }
        public string BeacukaiNo { get; set; }
        public DateTimeOffset BeacukaiDate { get; set; }
        public string CustomsType { get; set; }
    }

    public class BCViewModels
    {
        public string POSerialNumber { get; set; }
        public List<string> customnos { get; set; }
        public List<DateTimeOffset> customdates { get; set; }
        public List<string> customtypes { get; set; }
    }
}
