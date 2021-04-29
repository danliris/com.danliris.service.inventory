﻿using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Receipt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Receipt.FinishedGood
{
    public interface IReceiptFinishedGoodMonitoringService
    {
        Tuple<List<ReceiptFinishedGoodMonitoringViewModel>, int> GetMonitoring(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);
        Tuple<MemoryStream, string> GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset);
    }
}
