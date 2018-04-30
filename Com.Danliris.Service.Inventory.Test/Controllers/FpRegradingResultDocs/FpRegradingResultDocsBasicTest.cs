using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Test.DataUtils.FpRegradingResultDataUtil;
using Com.Danliris.Service.Inventory.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.FpRegradingResultDocs
{
    [Collection("TestServerFixture Collection")]
    public class FpRegradingResultDocsBasicTest : BasicControllerTest<InventoryDbContext, Lib.Services.FpRegradingResultDocsService, Lib.Models.FpRegradingResultDocs, FpRegradingResultDocsViewModel, FpRegradingResultDataUtil>
    {
        private static string URI = "v1/FpRegradingResultDocs";
        private static List<string> CreateValidationAttributes = new List<string> { "Date", "Details" };
        private static List<string> UpdateValidationAttributes = new List<string> { "Date", "Details" };

        public FpRegradingResultDocsBasicTest(TestServerFixture fixture) : base(fixture, URI, CreateValidationAttributes, UpdateValidationAttributes)
        {
        }
    }
}
