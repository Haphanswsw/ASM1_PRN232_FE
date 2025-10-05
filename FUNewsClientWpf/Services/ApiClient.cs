using System;
using System.Net;
using System.Net.Http;

namespace FUNewsClientWpf.Services
{
    public sealed class ApiClient : IDisposable
    {
        private readonly CookieContainer _cookies = new();
        private readonly HttpClientHandler _handler;
        public HttpClient Http { get; }

        public ApiClient(string baseUrl)
        {
            _handler = new HttpClientHandler
            {
                CookieContainer = _cookies,
                UseCookies = true,
                AllowAutoRedirect = false
            };
            Http = new HttpClient(_handler)
            {
                BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/")
            };
        }

        public void Dispose()
        {
            Http.Dispose();
            _handler.Dispose();
        }
    }
}