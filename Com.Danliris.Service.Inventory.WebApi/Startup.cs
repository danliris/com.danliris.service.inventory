using AutoMapper;
using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.FpRegradingResultDocs;
using Com.Danliris.Service.Inventory.Lib.Services.FpReturnFromBuyers;
using Com.Danliris.Service.Inventory.Lib.Services.FPReturnInvToPurchasingService;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalServices;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricServices;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodServices;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.Inventory;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialDistributionNoteService;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialRequestNoteServices;
using Com.Danliris.Service.Inventory.Lib.Services.StockTransferNoteService;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Expenditure;

using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Receipt;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.BalanceStock;

using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Receipt.Aval;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Expenditure.Aval;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportGreigeWeavingPerMonth;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportGreigeWeavingPerType;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.BalanceReportPerGrade;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Mutation;

namespace Com.Danliris.Service.Inventory.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void RegisterEndpoint()
        {
            APIEndpoint.Core = Configuration.GetValue<string>("CoreEndpoint") ?? Configuration["CoreEndpoint"];
            APIEndpoint.Inventory = Configuration.GetValue<string>("InventoryEndpoint") ?? Configuration["InventoryEndpoint"];
            APIEndpoint.Production = Configuration.GetValue<string>("ProductionEndpoint") ?? Configuration["ProductionEndpoint"];
            APIEndpoint.Purchasing = Configuration.GetValue<string>("PurchasingEndpoint") ?? Configuration["PurchasingEndpoint"];
            APIEndpoint.Sales = Configuration.GetValue<string>("SalesEndpoint") ?? Configuration["SalesEndpoint"];
            APIEndpoint.GarmentProduction = Configuration.GetValue<string>("GarmentProductionEndpoint") ?? Configuration["GarmentProductionEndpoint"];
            APIEndpoint.PackingInventory = Configuration.GetValue<string>("PackingInventoryEndpoint") ?? Configuration["PackingInventoryEndpoint"];
        }

        public void RegisterFacades(IServiceCollection services)
        {
            //services
            //    .AddTransient<FPReturnInvToPurchasingFacade>();
            //.AddTransient<FpRegradingResultDocsReportFacade>()
            //.AddTransient<InventoryDocumentFacade>()
            //.AddTransient<InventoryMovementFacade>()
            //.AddTransient<InventorySummaryFacade>()
            //.AddTransient<InventoryMovementReportFacade>()
            //.AddTransient<InventorySummaryReportFacade>();
        }

        public void RegisterServices(IServiceCollection services)
        {
            services
                //.AddScoped<MaterialDistributionNoteService>()
                //.AddTransient<MaterialDistributionNoteItemService>()
                //.AddTransient<StockTransferNoteService>()
                //.AddTransient<StockTransferNote_ItemService>()
                //.AddTransient<MaterialDistributionNoteDetailService>()
                //.AddTransient<FpRegradingResultDetailsDocsService>()
                //.AddTransient<FpRegradingResultDocsService>()
                //.AddTransient<FPReturnInvToPurchasingService>()
                //.AddTransient<FPReturnInvToPurchasingDetailService>()
                .AddTransient<IStockTransferNoteService, NewStockTransferNoteService>()
                .AddTransient<IMaterialRequestNoteService, NewMaterialRequestNoteService>()
                .AddTransient<IMaterialDistributionService, NewMaterialDistributionNoteService>()
                .AddTransient<IFpRegradingResultDocsService, NewFpRegradingResultDocsService>()
                .AddTransient<IInventoryDocumentService, InventoryDocumentService>()
                .AddTransient<IInventoryMovementService, InventoryMovementService>()
                .AddTransient<IInventoryDystuffService, InventoryDystuffService>()
                .AddTransient<IInventorySummaryService, InventorySummaryService>()
                .AddTransient<IFpReturnFromBuyerService, FpReturnFromBuyerService>()
                .AddTransient<IFPReturnInvToPurchasingService, NewFPReturnInvToPurchasingService>()
                .AddTransient<IGarmentLeftoverWarehouseReceiptFabricService, GarmentLeftoverWarehouseReceiptFabricService>()
                .AddTransient<IGarmentLeftoverWarehouseReceiptFinishedGoodService, GarmentLeftoverWarehouseReceiptFinishedGoodService>()
                .AddTransient<IGarmentLeftoverWarehouseReceiptAvalService, GarmentLeftoverWarehouseReceiptAvalService>()
                .AddTransient<IGarmentLeftoverWarehouseStockService, GarmentLeftoverWarehouseStockService>()
                .AddTransient<IGarmentLeftoverWarehouseExpenditureFinishedGoodService, GarmentLeftoverWarehouseExpenditureFinishedGoodService>()
                .AddTransient<IGarmentLeftoverWarehouseExpenditureAvalService, GarmentLeftoverWarehouseExpenditureAvalService>()
                .AddTransient<IGarmentLeftoverWarehouseExpenditureFabricService, GarmentLeftoverWarehouseExpenditureFabricService>()
                .AddTransient<IGarmentLeftoverWarehouseExpenditureAccessoriesService, GarmentLeftoverWarehouseExpenditureAccessoriesService>()
                .AddTransient<IGarmentLeftoverWarehouseReceiptAccessoriesService, GarmentLeftoverWarehouseReceiptAccessoriesService>()
                .AddTransient<IGarmentLeftoverWarehouseBalanceStockService, GarmentLeftoverWarehouseBalanceStockService>()
                .AddTransient<IReceiptMonitoringService, ReceiptMonitoringService>()
                .AddTransient<IReceiptAvalMonitoringService, ReceiptAvalMonitoringService>()
                .AddTransient<IGarmentLeftoverWarehouseReportExpenditureService, GarmentLeftoverWarehouseReportExpenditureService>()
                .AddTransient<IGarmentLeftoverWarehouseStockReportService, GarmentLeftoverWarehouseStockReportService>()
                .AddTransient<IInventoryWeavingDocumentUploadService, InventoryWeavingDocumentUploadService>()
                .AddTransient<IInventoryWeavingMovementService, InventoryWeavingMovementService>()
                .AddTransient<IExpenditureAvalMonitoringService, ExpenditureAvalMonitoringService>()
                .AddTransient<IInventoryWeavingDocumentOutService, InventoryWeavingDocumentOutService>()
                .AddTransient<IGarmentLeftoverWarehouseMutationReportService, GarmentLeftoverWarehouseMutationReportService>()
                .AddTransient<IReportGreigeWeavingPerMonthService, ReportGreigeWeavingPerMonthService>()
                .AddTransient<IReportGreigeWeavingPerTypeService, ReportGreigeWeavingPerTypeService>()
                .AddTransient<IBalanceReportPerPieceService, BalanceReportPerPieceService>()
                .AddTransient<IGarmentLeftoverWarehouseRecapStockReportService, GarmentLeftoverWarehouseRecapStockReportService>()
                .AddTransient<IGarmentLeftoverWarehouseFlowStockReportService, GarmentLeftoverWarehouseFlowStockReportService>()
                .AddTransient<IReportRecapStockGreigePerTypeService, ReportRecapStockGreigePerTypeService>()
                .AddTransient<IReportGreigeWeavingPerGradeService, ReportGreigeWeavingPerGradeService>()
                .AddScoped<IIdentityService, IdentityService>()
                .AddScoped<IValidateService, ValidateService>()
                .AddScoped<IHttpService, HttpService>()
                .AddScoped<IdentityService>()
                .AddScoped<HttpClientService>()
                .AddScoped<ValidateService>();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection") ?? Configuration["DefaultConnection"];

            services
                .AddDbContext<InventoryDbContext>(options => options.UseSqlServer(connectionString))
                .AddApiVersioning(options =>
                {
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                });

            //services.Configure<MongoDbSettings>(options =>
            //    {
            //        options.ConnectionString = Configuration.GetConnectionString("MongoConnection") ?? Configuration["MongoConnection"];
            //        options.Database = Configuration.GetConnectionString("MongoDatabase") ?? Configuration["MongoDatabase"];
            //    });

            //services.AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(Configuration.GetConnectionString("MongoConnection") ?? Configuration["MongoConnection"]));

            //services.AddTransient<IMongoDbContext, MongoDbMigrationContext>();
            //services.AddTransient<IInventoryDocumentIntegrationService, InventoryDocumentIntegrationService>();
            //services.AddTransient<IInventoryDocumentMongoRepository, InventoryDocumentMongoRepository>();


            this.RegisterServices(services);
            this.RegisterFacades(services);
            this.RegisterEndpoint();

            var Secret = Configuration.GetValue<string>("Secret") ?? Configuration["Secret"];
            var Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = false,
                        IssuerSigningKey = Key
                    };
                });

            services
                .AddMvcCore()
                .AddApiExplorer()
                .AddAuthorization()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                })
                .AddJsonFormatters();

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info() { Title = "My API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    In = "header",
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = "apiKey",
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>()
                {
                    {
                        "Bearer",
                        Enumerable.Empty<string>()
                    }
                });
                c.CustomSchemaIds(i => i.FullName);
            });
            #endregion

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddCors(options => options.AddPolicy("InventoryPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders("Content-Disposition", "api-version", "content-length", "content-md5", "content-type", "date", "request-id", "response-time");
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<InventoryDbContext>();
                context.Database.Migrate();
            }
            app.UseAuthentication();
            app.UseCors("InventoryPolicy");
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });
        }
    }
}
