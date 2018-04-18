using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Services.StockTransferNoteService;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Model = Com.Danliris.Service.Inventory.Lib.Models.StockTransferNoteModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Com.Danliris.Service.Inventory.Test.DataUtils.StockTransferNoteDataUtil;

namespace Com.Danliris.Service.Inventory.Test.Services.StockTransferNote
{
    [Collection("ServiceProviderFixture Collection")]
    public class StockTransferNoteBasicTest : BasicServiceTest<InventoryDbContext, StockTransferNoteService, Model.StockTransferNote, StockTransferNoteDataUtil>
    {
        private static List<string> Keys = new List<string>();
        private IServiceProvider serviceProvider { get; set; }

        public StockTransferNoteBasicTest(ServiceProviderFixture fixture) : base(fixture, Keys)
        {
            serviceProvider = fixture.ServiceProvider;
        }
    }
}
