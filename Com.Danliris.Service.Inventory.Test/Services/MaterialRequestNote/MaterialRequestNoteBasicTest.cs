//using Com.Danliris.Service.Inventory.Lib;
//using Com.Danliris.Service.Inventory.Lib.Services.MaterialsRequestNoteServices;
//using Com.Danliris.Service.Inventory.Test.DataUtils.MaterialRequestNoteDataUtil;
//using Com.Danliris.Service.Inventory.Test.Helpers;
//using System;
//using System.Collections.Generic;
//using Xunit;
//using Models = Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;

//namespace Com.Danliris.Service.Inventory.Test.Services.MaterialRequestNote
//{
//    [Collection("ServiceProviderFixture Collection")]
//    public class MaterialRequestNoteBasicTest : BasicServiceTest<InventoryDbContext, MaterialsRequestNoteService, Models.MaterialsRequestNote, MaterialRequestNoteDataUtil>
//    {
//        private static List<string> Keys = new List<string>();
//        private IServiceProvider serviceProvider { get; set; }

//        public MaterialRequestNoteBasicTest(ServiceProviderFixture fixture) : base(fixture, Keys)
//        {
//            serviceProvider = fixture.ServiceProvider;
//        }
//    }
//}
