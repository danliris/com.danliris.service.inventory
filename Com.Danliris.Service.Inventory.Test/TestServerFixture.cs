using Com.Danliris.Service.Inventory.Test.DataUtils.MaterialDistributionNoteDataUtil;
using Com.Danliris.Service.Inventory.Test.DataUtils.MaterialRequestNoteDataUtil;
using Com.Danliris.Service.Inventory.Test.DataUtils.StockTransferNoteDataUtil;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test
{
    public class TestServerFixture : IDisposable
    {
        private readonly TestServer _server;
        public HttpClient Client { get; }
        public IServiceProvider Service { get; }

        public TestServerFixture()
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
                    new KeyValuePair<string, string>("CoreEndpoint", "http://localhost:5001/v1/"),
                    new KeyValuePair<string, string>("InventoryEndpoint", "http://localhost:5002/v1/"),
                    new KeyValuePair<string, string>("ProductionEndpoint", "http://localhost:5003/v1/"),
                    new KeyValuePair<string, string>("Secret", "DANLIRISTESTENVIRONMENT"),
                    new KeyValuePair<string, string>("ASPNETCORE_ENVIRONMENT", "Test"),
                    new KeyValuePair<string, string>("DefaultConnection", "Server=localhost,1401;Database=com.danliris.db.inventory.controller.test;User=sa;password=Standar123.;MultipleActiveResultSets=true;")
                })
                .Build();


            var builder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .ConfigureServices(services =>
                {
                    services
                        .AddTransient<MaterialRequestNoteDataUtil>()
                        .AddTransient<MaterialRequestNoteItemDataUtil>()
                        .AddTransient<MaterialDistributionNoteDataUtil>()
                        .AddTransient<MaterialDistributionNoteItemDataUtil>()
                        .AddTransient<MaterialDistributionNoteDetailDataUtil>()
                        .AddTransient<StockTransferNoteDataUtil>()
                        .AddTransient<StockTransferNoteItemDataUtil>()
                        .AddSingleton<HttpClientService>();
                })
                .UseStartup<Startup>();

            string authority = configuration["Authority"];
            string clientId = configuration["ClientId"];
            string secret = configuration["Secret"];

            _server = new TestServer(builder);
            Client = _server.CreateClient();
            Service = _server.Host.Services;

            var User = new { username = "dev2", password = "Standar123" };

            HttpClient httpClient = new HttpClient();

            var response = httpClient.PostAsync("http://localhost:5000/v1/authenticate", new StringContent(JsonConvert.SerializeObject(User).ToString(), Encoding.UTF8, "application/json")).Result;
            response.EnsureSuccessStatusCode();

            var data = response.Content.ReadAsStringAsync();
            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(data.Result.ToString());
            var token = result["data"].ToString();

            Client.SetBearerToken(token);
        }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }

    [CollectionDefinition("TestServerFixture Collection")]
    public class TestServerFixtureCollection : ICollectionFixture<TestServerFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
