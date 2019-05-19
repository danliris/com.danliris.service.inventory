using Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialDistributionNoteViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Interfaces.MaterialDistributionNotes
{
    public interface IMaterialDistributionNote
    {
        Tuple<List<MaterialDistributionNote>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}");
        void UpdateIsRequestedProductionOrder(List<int> productionOrderIds, string context);
        void UpdateIsCompletedTrueProductionOrder(int id);
        void UpdateIsCompletedFalseProductionOrder(int id);
        Task<int> CreateModel(MaterialDistributionNote Model);
        Task<MaterialDistributionNote> ReadModelById(int id);
        bool UpdateIsApprove(List<int> Ids);
        //Task<int> UpdateModel(int Id, MaterialDistributionNote Model);
        Task<int> DeleteModel(int Id);
        void OnCreating(MaterialDistributionNote model);
        void OnUpdating(int id, MaterialDistributionNote model);
        void OnDeleting(MaterialDistributionNote model);
        IQueryable<MaterialDistributionNoteReportViewModel> GetReportQuery(string unitId, string type, DateTime date, int offset);
        Tuple<List<MaterialDistributionNoteReportViewModel>, int> GetReport(string unitId, string type, DateTime date, int page, int size, string Order, int offset);
        List<MaterialDistributionNoteReportViewModel> GetPdfReport(string unitId, string unitName, string type, DateTime date, int offset);
        MaterialDistributionNoteViewModel MapToViewModel(MaterialDistributionNote model);
        MaterialDistributionNote MapToModel(MaterialDistributionNoteViewModel viewModel);
        void CreateInventoryDocument(MaterialDistributionNote Model, string Type);
        Task<MaterialDistributionNote> CustomCodeGenerator(MaterialDistributionNote Model);


    }
}
