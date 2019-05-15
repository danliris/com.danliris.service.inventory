using AutoMapper;
using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Facades;
using Com.Danliris.Service.Inventory.Lib.Facades.InventoryFacades;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.IntegrationServices;
using Com.Danliris.Service.Inventory.Lib.MongoRepositories.InventoryDocument;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.FPReturnInvToPurchasingService;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialDistributionNoteService;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialsRequestNoteServices;
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
using Newtonsoft.Json.Serialization;
using System.Text;

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
        }

        public void RegisterFacades(IServiceCollection services)
        {
            services
                .AddTransient<FPReturnInvToPurchasingFacade>()
                .AddTransient<FpRegradingResultDocsReportFacade>()
                .AddTransient<InventoryDocumentFacade>()
                .AddTransient<InventoryMovementFacade>()
                .AddTransient<InventorySummaryFacade>()
                .AddTransient<InventoryMovementReportFacade>()
                .AddTransient<InventorySummaryReportFacade>();
        }

        public void RegisterServices(IServiceCollection services)
        {
            services
                .AddScoped<MaterialsRequestNoteService>()
                .AddScoped<MaterialsRequestNote_ItemService>()
                .AddScoped<MaterialDistributionNoteService>()
                .AddTransient<MaterialDistributionNoteItemService>()
                .AddTransient<StockTransferNoteService>()
                .AddTransient<StockTransferNote_ItemService>()
                .AddTransient<MaterialDistributionNoteDetailService>()
                .AddTransient<FpRegradingResultDetailsDocsService>()
                .AddTransient<FpRegradingResultDocsService>()
                .AddTransient<FPReturnInvToPurchasingService>()
                .AddTransient<FPReturnInvToPurchasingDetailService>()
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

            services.Configure<MongoDbSettings>(options =>
                {
                    options.ConnectionString = Configuration.GetConnectionString("MongoConnection") ?? Configuration["MongoConnection"];
                    options.Database = Configuration.GetConnectionString("MongoDatabase") ?? Configuration["MongoDatabase"];
                });

            services.AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(Configuration.GetConnectionString("MongoConnection") ?? Configuration["MongoConnection"]));

            services.AddTransient<IMongoDbContext, MongoDbMigrationContext>();
            services.AddTransient<IInventoryDocumentIntegrationService, InventoryDocumentIntegrationService>();
            services.AddTransient<IInventoryDocumentMongoRepository, InventoryDocumentMongoRepository>();


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
                .AddAuthorization()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                .AddJsonFormatters();

            services.AddAutoMapper();
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
        }
    }
}
