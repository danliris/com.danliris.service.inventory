using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Services.StockTransferNoteService;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Model = Com.Danliris.Service.Inventory.Lib.Models.StockTransferNoteModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Com.Danliris.Service.Inventory.Lib.ViewModels.StockTransferNoteViewModel;
using Com.Danliris.Service.Inventory.Test.DataUtils.StockTransferNoteDataUtil;

namespace Com.Danliris.Service.Inventory.Test.Controllers.StockTransferNote
{
    [Collection("TestServerFixture Collection")]
    public class StockTransferNoteBasicTest : BasicControllerTest<InventoryDbContext, StockTransferNoteService, Model.StockTransferNote, StockTransferNoteViewModel, StockTransferNoteDataUtil>
    {
        private static string URI = "v1/stock-transfer-notes";
        private static List<string> CreateValidationAttributes = new List<string> { "ReferenceNo", "ReferenceType", "SourceStorageId", "TargetStorageId", "StockTransferNoteItems" };
        private static List<string> UpdateValidationAttributes = new List<string> { "ReferenceNo", "ReferenceType", "SourceStorageId", "TargetStorageId", "StockTransferNoteItems" };

        public StockTransferNoteBasicTest(TestServerFixture fixture) : base(fixture, URI, CreateValidationAttributes, UpdateValidationAttributes)
        {
        }
    }
}
