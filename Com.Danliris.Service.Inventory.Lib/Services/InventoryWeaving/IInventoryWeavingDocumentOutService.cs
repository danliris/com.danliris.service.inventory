using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving
{
    public interface IInventoryWeavingDocumentOutService
    {
        ListResult<InventoryWeavingDocument> Read(int page, int size, string order, string keyword, string filter);
        ReadResponse<InventoryWeavingDocumentItem> GetDistinctMaterial(int page, int size, string filter, string order, string keyword);
        InventoryWeavingDocumentDetailViewModel ReadById(int id);
        List<InventoryWeavingItemDetailViewModel> GetMaterialItemList(string material);
        Task<InventoryWeavingDocument> MapToModel(InventoryWeavingDocumentOutViewModel data);
        Task Create(InventoryWeavingDocument model);
        MemoryStream DownloadCSVOut( DateTime dateFrom, DateTime dateTo, int clientTimeZoneOffset, string bonType);
        Tuple<List<InventoryWeavingOutReportViewModel>, int> GetReport(string bonType, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);
        MemoryStream GenerateExcelReceiptReport(string bonType, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offset);
        
        List<string> CsvHeader { get; }
        List<string> CsvHeaderUpload { get; }
        Task<InventoryWeavingDocument> MapToModelUpload(InventoryWeavingDocumentOutUploadViewModel data);

        Task<InventoryWeavingDocumentOutUploadViewModel> MapToViewModel(List<InventoryWeavingUploadCsvOutViewModel> data, DateTimeOffset date);
        Tuple<bool, List<object>> UploadValidate(ref List<InventoryWeavingUploadCsvOutViewModel> Data, List<KeyValuePair<string, StringValues>> Body);
        int checkCsv (List<InventoryWeavingDocumentOutItemViewModel> data);
        Task UploadData(InventoryWeavingDocument data, string username);
    }
}
