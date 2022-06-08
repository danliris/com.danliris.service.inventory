using Com.Danliris.Service.Inventory.Lib.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.Helpers
{
    public class HttpTestService : IHttpService
    {
        public static string Token;

        public Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }
        public Task<HttpResponseMessage> GetAsync(string url)
        {
            return Task.Run(() => new HttpResponseMessage() { Content = new StringContent("{\"data\" : [{'Id':1, '_id':1, 'Unit':'unit', 'code':'code', 'name':'name'}]}")});
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }

        public Task<HttpResponseMessage> PatchAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }

        public Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }
    }
}
