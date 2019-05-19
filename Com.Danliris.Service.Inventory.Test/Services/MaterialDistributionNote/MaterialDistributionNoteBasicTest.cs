//using Com.Danliris.Service.Inventory.Lib;
//using Com.Danliris.Service.Inventory.Test.Helpers;
//using Model = Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Xunit;
//using Com.Danliris.Service.Inventory.Lib.Services.MaterialDistributionNoteService;
//using Com.Danliris.Service.Inventory.Test.DataUtils.MaterialDistributionNoteDataUtil;

//namespace Com.Danliris.Service.Inventory.Test.Services.MaterialDistributionNote
//{
//    [Collection("ServiceProviderFixture Collection")]
//    public class MaterialDistributionNoteBasicTest : BasicServiceTest<InventoryDbContext, MaterialDistributionNoteService, Model.MaterialDistributionNote, MaterialDistributionNoteDataUtil>
//    {
//        private static List<string> Keys = new List<string>();
//        private IServiceProvider serviceProvider { get; set; }

//        public MaterialDistributionNoteBasicTest(ServiceProviderFixture fixture) : base(fixture, Keys)
//        {
//            serviceProvider = fixture.ServiceProvider;
//        }
//    }
//}
