using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.IntegrationDataUtil
{
    public class UnitDODataUtil
    {
        public UnitDoViewModel GetNewDataUNitDo()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var data = new UnitDoViewModel
            {
                POSerialNumber = "POSerialNumber123",
                ProductCode = "ProductCode123",
                ProductName = "FABRIC",
                Rono = "roNo"
            };
            return data;
        }

        public Dictionary<string, object> GetMultipleResultFormatterOk()
        {

            var data = new List<UnitDoViewModel>();

            var data1 = GetNewDataUNitDo();
            var data2 = GetNewDataUNitDo();

            data2.POSerialNumber = "POSerialNumber1234";
            data.Add(data1);
            data.Add(data2);

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);

            return result;
        }

        public BCViewModels GetNewDataBC()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var dates = new List<DateTimeOffset>();
            var nos = new List<string>();
            var types = new List<string>();

            dates.Add(DateTimeOffset.Now);
            dates.Add(DateTimeOffset.Now.AddDays(1));
            nos.Add("09324242");
            nos.Add("09324243");
            types.Add("types");
            types.Add("types23");

            var data = new BCViewModels
            {
                POSerialNumber = "POSerialNumber123",
                customdates = dates,
                customnos = nos,
                customtypes = types
            };
            return data;

        }

        public Dictionary<string, object> GetMultipleResultBCFormatterOk()
        {
            var data = new List<BCViewModels> { GetNewDataBC() };

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);

            return result;
        }

        public string GetMultipleResultBCFormatterOkString()
        {
            var result = GetMultipleResultBCFormatterOk();

            return JsonConvert.SerializeObject(result);
        }

        public string GetMultipleResultFormatterOkString()
        {
            var result = GetMultipleResultFormatterOk();

            return JsonConvert.SerializeObject(result);
        }

    }
}
