using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace WebApiRequest.Factory
{
    public static class HttpClientFactory
    {
        private static readonly ConcurrentDictionary<string, Lazy<HttpClient>> HttpClients =
            new ConcurrentDictionary<string, Lazy<HttpClient>>();

        public static HttpClient GetClient(string baseAddress, TimeSpan timeout) 
        {
            baseAddress = baseAddress?.Trim() ?? "";
            var key = $"{baseAddress}|{timeout}";
            return HttpClients.GetOrAdd(key, new Lazy<HttpClient>(() => new HttpClient { BaseAddress = new Uri(baseAddress), Timeout = timeout })).Value;
        }

        public static HttpClient GetClient(string baseAddress, int second = 100) 
        {
            return GetClient(baseAddress, TimeSpan.FromSeconds(second));
        }
    }
}
