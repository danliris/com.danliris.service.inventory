using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialsRequestNoteViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Com.Danliris.Service.Inventory.Lib.Services.MaterialsRequestNoteServices.MaterialsRequestNoteService;

namespace Com.Danliris.Service.Inventory.Lib.Interfaces.MaterialRequestNote
{
    public interface IMaterialsRequestNote
    {
        Tuple<List<MaterialsRequestNote>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}");
        Task<MaterialsRequestNote> CustomCodeGenerator(MaterialsRequestNote Model);
        void UpdateIsRequestedProductionOrder(List<int> productionOrderIds, string context);
        void UpdateIsCompletedTrueProductionOrder(int id);
        void UpdateIsCompletedFalseProductionOrder(int id);
        void UpdateDistributedQuantityProductionOrder(List<SppParams> contextAndIds);
        Task<int> CreateModel(MaterialsRequestNote Model);
        Task<MaterialsRequestNote> ReadModelById(int id);
        Task UpdateIsCompleted(int Id, MaterialsRequestNote Model);
        void UpdateDistributedQuantity(int Id, MaterialsRequestNote Model);
        Task<int> UpdateModel(int Id, MaterialsRequestNote Model);
        Task<int> DeleteModel(int Id);
        void OnCreating(MaterialsRequestNote model);
        void OnUpdating(int id, MaterialsRequestNote model);
        void OnDeleting(MaterialsRequestNote model);
        IQueryable<MaterialsRequestNoteReportViewModel> GetReportQuery(string materialsRequestNoteCode, string productionOrderId, string unitId, string productId, string status, DateTime? dateFrom, DateTime? dateTo, int offset);
        Tuple<List<MaterialsRequestNoteReportViewModel>, int> GetReport(string materialsRequestNoteCode, string productionOrderId, string unitId, string productId, string status, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);
        void CreateInventoryDocument(MaterialsRequestNote Model, string Type);
        MaterialsRequestNoteViewModel MapToViewModel(MaterialsRequestNote model);
        MaterialsRequestNote MapToModel(MaterialsRequestNoteViewModel viewModel);

    }
}
