using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using Com.Moonlay.NetCore.Lib;
using System.IO;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Data;
using System.Globalization;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportRecapStockGreigePerType
{
    public class ReportRecapStockGreigePerTypeService : IReportRecapStockGreigePerTypeService
    {
        private string USER_AGENT = "Service";
        private const string UserAgent = "inventory-service";
        protected DbSet<InventoryWeavingDocumentItem> DbSetItem;
        protected DbSet<InventoryWeavingMovement> DbSetMovement;
        protected DbSet<InventoryWeavingDocument> DbSetDoc;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;


        public ReportRecapStockGreigePerTypeService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSetItem = dbContext.Set<InventoryWeavingDocumentItem>();
            DbSetMovement = dbContext.Set<InventoryWeavingMovement>();
            DbSetDoc = dbContext.Set<InventoryWeavingDocument>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
            //IdentityService = serviceProvider.GetService<IIdentityService>();
        }


        public async Task<Tuple<List<InventoryWeavingInOutViewModel>, int>> GetRecapStocktGreige( DateTime? dateTo, int offset, int page, int size, string Order)
        {

            List<InventoryWeavingInOutViewModel> Query = await GetQuery( dateTo, offset);
            //Query = Query.Where(x => (x.BeginningBalanceQty != 0) || (x.BeginningBalancePrice != 0) || (x.EndingBalancePrice > 0) || (x.EndingBalanceQty > 0) || (x.ExpendKon1APrice > 0) || (x.ExpendKon1AQty > 0) ||
            //(x.ExpendKon1BPrice > 0) || (x.ExpendKon1BQty > 0) || (x.ExpendKon2APrice > 0) || (x.ExpendKon2AQty > 0) || (x.ExpendKon2BPrice > 0) || (x.ExpendKon2BQty > 0) || (x.ExpendKon2CPrice > 0) || (x.ExpendKon2CQty > 0) ||
            //(x.ExpendProcessPrice > 0) || (x.ExpendProcessQty > 0) || (x.ExpendRestPrice > 0) || (x.ExpendRestQty > 0) || (x.ExpendReturPrice > 0) || (x.ExpendReturQty > 0) || (x.ExpendSamplePrice > 0) || (x.ExpendSampleQty > 0) ||
            //(x.ReceiptCorrectionPrice > 0) || (x.ReceiptCorrectionQty > 0) || (x.ReceiptKon1APrice > 0) || (x.ReceiptKon1AQty > 0) || (x.ReceiptKon1BPrice > 0) || (x.ReceiptKon1BQty > 0) || (x.ReceiptKon2APrice > 0) || (x.ReceiptKon2AQty > 0)
            //|| (x.ReceiptKon2BPrice > 0) || (x.ReceiptKon2BQty > 0) || (x.ReceiptKon2CPrice > 0) || (x.ReceiptKon2CQty > 0) || (x.ReceiptProcessPrice > 0) || (x.ReceiptProcessQty > 0) || (x.ReceiptPurchasePrice > 0) || (x.ReceiptPurchaseQty > 0)).ToList();

            //Query = Query.OrderBy(x => x.ProductCode).ThenBy(x => x.PlanPo).ToList();
            Pageable<InventoryWeavingInOutViewModel> pageable = new Pageable<InventoryWeavingInOutViewModel>(Query, page - 1, size);
            List<InventoryWeavingInOutViewModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;
            //int TotalData = Data.Count();
            return Tuple.Create(Data, TotalData);
        }


        public async Task<List<InventoryWeavingInOutViewModel>> GetQuery(  DateTime? dateTo, int offset)
        {
            
            //DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var result = (from a in DbSetDoc
                          join b in DbSetMovement on a.Id equals b.InventoryWeavingDocumentId
                          where a._IsDeleted == false
                             && b._IsDeleted == false
                             //&& a.BonType == (string.IsNullOrWhiteSpace(bonType) ? a.BonType : bonType)
                               //&& a.Date.AddHours(offset).Date >= DateFrom.Date
                              && a.Date.AddHours(offset).Date <= DateTo.Date
                          orderby a.Date, a._CreatedUtc ascending
                          select new InventoryWeavingInOutViewModel
                          {
                              Construction = b.Construction,
                              Type = b.Type,
                              MaterialName = b.MaterialName,
                              WovenType = b.WovenType,
                              Yarn1 = b.Yarn1,
                              Yarn2 = b.Yarn2,
                              YarnOrigin1 = b.YarnOrigin1,
                              YarnOrigin2 = b.YarnOrigin2,
                              YarnType1 = b.YarnType1,
                              YarnType2 = b.YarnType2,
                              Qty = b.Quantity,
                              QtyPiece = b.QuantityPiece


                          });

            var data1 = result.GroupBy(s => new
            {
                s.Type,
                s.MaterialName,
                s.WovenType,
                s.Width,
                s.Yarn1,
                s.Yarn2,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2
            }).Select(s => new InventoryWeavingInOutViewModel()
            {
                Construction = s.FirstOrDefault().Construction,
                //Grade = s.FirstOrDefault.Grade,

                Type = s.Key.Type,
                Qty = s.Sum(d => d.Qty),
                QtyPiece = s.Sum(d => d.QtyPiece),
                MaterialName = s.Key.MaterialName,
                WovenType = s.Key.WovenType,
                Width = s.Key.Width,
                Yarn1 = s.Key.Yarn1,
                Yarn2 = s.Key.Yarn2,
                YarnOrigin1 = s.Key.YarnOrigin1,
                YarnOrigin2 = s.Key.YarnOrigin2,
                YarnType1 = s.Key.YarnType1,
                YarnType2 = s.Key.YarnType2



            });

            var data = data1.GroupBy(s => new
            {
               
                s.MaterialName,
                s.WovenType,
                s.Width,
                s.Yarn1,
                s.Yarn2,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2
            }).Select(s => new InventoryWeavingInOutViewModel()
            {

                Construction = s.FirstOrDefault().Construction,
                Qty = s.FirstOrDefault(d => d.Type == "OUT") != null ? s.FirstOrDefault(d => d.Type == "IN").Qty - s.FirstOrDefault(d => d.Type == "OUT").Qty :
                             s.FirstOrDefault(d => d.Type == "OUT") == null ? s.FirstOrDefault(d => d.Type == "IN").Qty : 0,
                QtyPiece = s.FirstOrDefault(d => d.Type == "OUT") != null ? s.FirstOrDefault(d => d.Type == "IN").QtyPiece - s.FirstOrDefault(d => d.Type == "OUT").QtyPiece :
                                s.FirstOrDefault(d => d.Type == "OUT") == null ? s.FirstOrDefault(d => d.Type == "IN").QtyPiece : 0
            }).Where(x => x.Qty > 0 && x.QtyPiece > 0).ToList();

            List<InventoryWeavingInOutViewModel> reportViewModels = new List<InventoryWeavingInOutViewModel>();
            int index = 1;

            foreach (var i in data) {
                reportViewModels.Add(new InventoryWeavingInOutViewModel
                {
                    Nomor = index++,
                    Construction = i.Construction,
                    Qty = i.Qty,
                    QtyPiece = i.QtyPiece
                    
                });
            }

            var total = new InventoryWeavingInOutViewModel
            {
                Construction = "Total",
                Qty = Math.Round(data.Sum(x => x.Qty), 2),
                QtyPiece = Math.Round(data.Sum(x => x.QtyPiece), 2)

            };

            reportViewModels.Add(total);


            return reportViewModels;


        }

        public async Task<MemoryStream> GenerateExcel(DateTime? dateTo, int offset)
        {
            var Query = await GetQuery(dateTo, offset);
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            string Bulan = DateTo.ToString("MMM yyyy", new CultureInfo("id-ID"));
            string Tanggal = DateTo.ToString("dd MMM yyyy", new CultureInfo("id-ID"));

            DataTable result = new DataTable();
            var headers = new string[] { "No", "Konstruksi", "Piece", "Meter", "Keterangan"};
           // var subheaders = new string[] { "Piece", "Meter", "Piece", "Meter", "Piece", "Meter", "Piece", "Meter", };


            result.Columns.Add(new DataColumn() { ColumnName = headers[0], DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[1], DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[2], DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[3], DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[4], DataType = typeof(string) });

            //for (int i = 6; i < 14; i++)
            //{
            //    result.Columns.Add(new DataColumn() { ColumnName = headers[i], DataType = typeof(Double) });
            //}

            Query.RemoveAt(Query.Count() - 1);

            double TotQty = 0;
            double TotQtyPiece = 0;
           


            //double TotEndingBalance = 0;



            foreach (var item in Query)
            {
                TotQty += item.Qty;
                TotQtyPiece += item.QtyPiece;

                result.Rows.Add(item.Nomor, item.Construction, item.QtyPiece, item.Qty, ""
                                );

            }


            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Data");


            var col = (char)('A' + (result.Columns.Count - 1));
            //string tglawal = new DateTimeOffset(dateFrom.Value.Date).ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            //string tglakhir = new DateTimeOffset(dateTo.Value.Date).ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            sheet.Cells[$"A1:{col}1"].Value = string.Format("Laporan Rekapitulasi Stock GREY");
            sheet.Cells[$"A1:{col}1"].Merge = true;
            sheet.Cells[$"A1:{col}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A1:{col}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A1:{col}1"].Style.Font.Bold = true;

            sheet.Cells[$"A2:{col}2"].Value = string.Format("BAGIAN : WEAVING 3 / AJL");
            sheet.Cells[$"A2:{col}2"].Merge = true;
            sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            //sheet.Cells[$"A2:{col}2"].Value = string.Format("PERIODE : {0}", Bulan);
            //sheet.Cells[$"A2:{col}2"].Merge = true;
            //sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
            //sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            //sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            

            sheet.Cells[$"A3:{col}3"].Value = string.Format("PER TGL. : {0}", Tanggal);
            sheet.Cells[$"A3:{col}3"].Merge = true;
            sheet.Cells[$"A3:{col}3"].Style.Font.Bold = true;
            sheet.Cells[$"A3:{col}3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A3:{col}3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            sheet.Cells[$"A4:{col}4"].Value = string.Format("BARANG : GREY");
            sheet.Cells[$"A4:{col}4"].Merge = true;
            sheet.Cells[$"A4:{col}4"].Style.Font.Bold = true;
            sheet.Cells[$"A4:{col}4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[$"A4:{col}4"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


            sheet.Cells["A7"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light16);
            //sheet.Cells["G7"].Value = headers[6];
            //sheet.Cells["G7:H7"].Merge = true;

            //sheet.Cells["I7"].Value = headers[8];
            //sheet.Cells["I7:J7"].Merge = true;

            //sheet.Cells["K7"].Value = headers[10];
            //sheet.Cells["K7:L7"].Merge = true;

            //sheet.Cells["M7"].Value = headers[12];
            //sheet.Cells["M7:N7"].Merge = true;





            foreach (var i in Enumerable.Range(0, 5))
            {
                col = (char)('A' + i);
                sheet.Cells[$"{col}6"].Value = headers[i];
                //sheet.Cells[$"{col}6"].Value = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //sheet.Cells[$"{col}7:{col}8"].Merge = true;
            }

            //for (var i = 0; i < 8; i++)
            //{
            //    col = (char)('G' + i);
            //    sheet.Cells[$"{col}8"].Value = subheaders[i];

            //}

            //foreach (var i in Enumerable.Range(0, 1))
            //{
            //    col = (char)('P' + i);
            //    sheet.Cells[$"{col}5"].Value = headers[i + 15];
            //    sheet.Cells[$"{col}5:{col}6"].Merge = true;
            //}

            sheet.Cells["A6:E6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells["A6:E6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells["A6:E6"].Style.Font.Bold = true;
            var widths = new int[] { 10, 30, 10, 10, 20 };
            foreach (var i in Enumerable.Range(0, headers.Length))
            {
                sheet.Column(i + 1).Width = widths[i];
            }

            //sheet.Column(5).Hidden = true;

            var a = Query.Count();
            var date = a + 2;
            var dateNow = DateTime.Now.Date;


            //sheet.Cells[$"A{5}:J{5 + a + 3}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            sheet.Cells[$"A{5}:E{6 + a + 1}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            sheet.Cells[$"A{6}:F{6 + a + 1}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;


            sheet.Cells[$"A{7 + a}"].Value = "T O T A L  . . . . . . . . . . . . . . .";
            sheet.Cells[$"A{7 + a}:B{7 + a}"].Merge = true;
            sheet.Cells[$"A{7 + a}:E{7 + a}"].Style.Font.Bold = true;
            sheet.Cells[$"A{7 + a}:B{7 + a}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A{7 + a}:B{7 + a}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"C{7 + a}"].Value = TotQtyPiece;
            sheet.Cells[$"D{7 + a}"].Value = TotQty;
            sheet.Cells[$"E{9 + a}"].Value = "";


            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;


        }
    }
}
