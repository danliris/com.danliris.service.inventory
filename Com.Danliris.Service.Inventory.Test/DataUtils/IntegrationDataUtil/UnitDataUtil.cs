using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.IntegrationDataUtil
{
    public static class UnitDataUtil
    {
        public static UnitViewModel GetFinishingUnit(HttpClientTestService client)
        {
            var response = client.GetAsync($"{APIEndpoint.Core}master/units").Result;
            response.EnsureSuccessStatusCode();

            var data = response.Content.ReadAsStringAsync();
            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(data.Result.ToString());

            List<UnitViewModel> list = JsonConvert.DeserializeObject<List<UnitViewModel>>(result["data"].ToString());
            UnitViewModel fp = list.First(p => p.Name.Equals("FINISHING"));

            return fp;
        }
    }
}
