using Com.Danliris.Service.Inventory.Lib.Services;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.Helpers
{
    public class HttpFailTestService : IHttpService
    {
        public static string Token;

        public Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError));
        }
        public Task<HttpResponseMessage> GetAsync(string url)
        {
            return Task.Run(() => new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError));
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError));
        }

        public Task<HttpResponseMessage> PatchAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError) {
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    error = "error",
                    message = "message"
                }))
            });
        }

        public Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    error = "error",
                    message = "message"
                }))
            });
        }
    }
}
