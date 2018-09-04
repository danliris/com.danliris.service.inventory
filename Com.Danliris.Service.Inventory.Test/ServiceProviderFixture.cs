using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialDistributionNoteService;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialsRequestNoteServices;
using Com.Danliris.Service.Inventory.Lib.Services.StockTransferNoteService;
using Com.Danliris.Service.Inventory.Test.DataUtils.FpRegradingResultDataUtil;
using Com.Danliris.Service.Inventory.Test.DataUtils.MaterialDistributionNoteDataUtil;

using Com.Danliris.Service.Inventory.Test.DataUtils.MaterialRequestNoteDataUtil;
using Com.Danliris.Service.Inventory.Test.DataUtils.StockTransferNoteDataUtil;
using TestHelpers = Com.Danliris.Service.Inventory.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;
using Com.Danliris.Service.Inventory.Test.DataUtils.FPReturnInvToPurchasingDataUtil;
using Com.Danliris.Service.Inventory.Lib.Services.FPReturnInvToPurchasingService;
using Com.Danliris.Service.Inventory.Lib.Facades;
using Com.Danliris.Service.Inventory.Test.DataUtils.InventoryDataUtils;
using Com.Danliris.Service.Inventory.Lib.Facades.InventoryFacades;

namespace Com.Danliris.Service.Inventory.Test
{
    public class ServiceProviderFixture
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public void RegisterEndpoint(IConfigurationRoot Configuration)
        {
            APIEndpoint.Core = Configuration.GetValue<string>("CoreEndpoint") ?? Configuration["CoreEndpoint"];
            APIEndpoint.Inventory = Configuration.GetValue<string>("InventoryEndpoint") ?? Configuration["InventoryEndpoint"];
            APIEndpoint.Production = Configuration.GetValue<string>("ProductionEndpoint") ?? Configuration["ProductionEndpoint"];
            APIEndpoint.Purchasing = Configuration.GetValue<string>("PurchasingEndpoint") ?? Configuration["PurchasingEndpoint"];
        }

        public ServiceProviderFixture()
        {
            /*
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json")
                .Build();
            */

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    /*
                    new KeyValuePair<string, string>("Authority", "http://localhost:5000"),
                    new KeyValuePair<string, string>("ClientId", "dl-test"),
                    */
                    new KeyValuePair<string, string>("Secret", "DANLIRISTESTENVIRONMENT"),
                    new KeyValuePair<string, string>("ASPNETCORE_ENVIRONMENT", "Test"),
                    new KeyValuePair<string, string>("CoreEndpoint", "http://localhost:5001/v1/"),
                    new KeyValuePair<string, string>("InventoryEndpoint", "http://localhost:5002/v1/"),
                    new KeyValuePair<string, string>("ProductionEndpoint", "http://localhost:5003/v1/"),
                    new KeyValuePair<string, string>("PurchasingEndpoint", "http://localhost:5004/v1/"),
                    new KeyValuePair<string, string>("DefaultConnection", "Server=localhost,1401;Database=com.danliris.db.inventory.service.test;User=sa;password=Standar123.;MultipleActiveResultSets=true;")
                })
                .Build();

            RegisterEndpoint(configuration);
            string connectionString = configuration.GetConnectionString("DefaultConnection") ?? configuration["DefaultConnection"];

            this.ServiceProvider = new ServiceCollection()
                .AddDbContext<InventoryDbContext>((serviceProvider, options) =>
                {
                    options.UseSqlServer(connectionString);
                }, ServiceLifetime.Transient)
                .AddTransient<MaterialDistributionNoteService>(provider => new MaterialDistributionNoteService(provider))
                .AddTransient<MaterialDistributionNoteItemService>(provider => new MaterialDistributionNoteItemService(provider))
                .AddTransient<MaterialDistributionNoteDetailService>(provider => new MaterialDistributionNoteDetailService(provider))
                .AddTransient<MaterialsRequestNoteService>(provider => new MaterialsRequestNoteService(provider))
                .AddTransient<MaterialsRequestNote_ItemService>(provider => new MaterialsRequestNote_ItemService(provider))
                .AddTransient<StockTransferNoteService>(provider => new StockTransferNoteService(provider))
                .AddTransient<StockTransferNote_ItemService>(provider => new StockTransferNote_ItemService(provider))
                .AddTransient<FPReturnInvToPurchasingService>(provider => new FPReturnInvToPurchasingService(provider))
                .AddTransient<FPReturnInvToPurchasingDetailService>(provider => new FPReturnInvToPurchasingDetailService(provider))
                .AddTransient<FPReturnInvToPurchasingFacade>()
                .AddTransient<MaterialRequestNoteDataUtil>()
                .AddTransient<MaterialRequestNoteItemDataUtil>()
                .AddTransient<Lib.Services.FpRegradingResultDocsService>(provider => new Lib.Services.FpRegradingResultDocsService(provider))
                .AddTransient<Lib.Services.FpRegradingResultDetailsDocsService>(provider => new Lib.Services.FpRegradingResultDetailsDocsService(provider))
                .AddTransient<FpRegradingResultDataUtil>()
                .AddTransient<FpRegradingResultDetailsDataUtil>()
                .AddTransient<MaterialDistributionNoteDataUtil>()
                .AddTransient<MaterialDistributionNoteItemDataUtil>()
                .AddTransient<MaterialDistributionNoteDetailDataUtil>()
                .AddTransient<StockTransferNoteDataUtil>()
                .AddTransient<StockTransferNoteItemDataUtil>()
                .AddTransient<FPReturnInvToPurchasingDataUtil>()
                .AddTransient<FPReturnInvToPurchasingDetailDataUtil>()

                .AddTransient<InventoryDocumentFacade>()
                .AddTransient<InventoryDocumentDataUtil>()
                .AddTransient<InventoryDocumentItemDataUtil>()

                .AddTransient<InventoryMovementFacade>()
                .AddTransient<InventoryMovementDataUtil>()

                .AddTransient<InventorySummaryFacade>()
                .AddTransient<InventorySummaryDataUtil>()

                .AddTransient<InventoryMovementReportFacade>()

                .AddSingleton<TestHelpers.HttpClientTestService>(provider => new TestHelpers.HttpClientTestService(provider))
                .AddSingleton<HttpClientService>()
                .AddSingleton<IdentityService>()

                .BuildServiceProvider();

            InventoryDbContext dbContext = ServiceProvider.GetService<InventoryDbContext>();
            dbContext.Database.Migrate();
        }

        //public void Dispose()
        //{
        //}
    }

    [CollectionDefinition("ServiceProviderFixture Collection")]
    public class ServiceProviderFixtureCollection : ICollectionFixture<ServiceProviderFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}