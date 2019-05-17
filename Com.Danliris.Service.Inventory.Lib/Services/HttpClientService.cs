﻿using Com.Danliris.Service.Inventory.Lib.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services
{
    public class HttpClientService : IHttpClientService
    {
        private HttpClient _client = new HttpClient();

        public HttpClientService(IdentityService identityService)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, identityService.Token);
        }

        public async Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            return await _client.PutAsync(url, content);
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await _client.GetAsync(url);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return await _client.PostAsync(url, content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return await _client.DeleteAsync(url);
        }
    }
}