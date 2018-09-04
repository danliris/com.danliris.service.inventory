using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using Com.Danliris.Service.Inventory.Test.DataUtils.InventoryDataUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.InventorySummaryReport
{
    [Collection("TestServerFixture Collection")]
    public class InventorySummaryReportControllerTest
    {
        private const string MediaType = "application/json";
        private readonly string URI = "v1/inventory/inventory-summary-reports";

        private TestServerFixture TestFixture { get; set; }

        private HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public InventorySummaryReportControllerTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        [Fact]
        public async Task Should_Success_Get_Report()
        {
            var response = await this.Client.GetAsync(URI);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = response.Content.ReadAsStringAsync().Result;
            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());

            Assert.True(result.ContainsKey("apiVersion"));
            Assert.True(result.ContainsKey("message"));
            Assert.True(result.ContainsKey("data"));
            Assert.True(result["data"].GetType().Name.Equals("JArray"));
        }

        [Fact]
        public async Task Should_Success_Get_Report_Excel()
        {
            var response = await this.Client.GetAsync(URI + "/download");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


    }
}
