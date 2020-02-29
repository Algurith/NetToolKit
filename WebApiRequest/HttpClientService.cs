using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiRequest.Factory;

namespace WebApiRequest
{
    public class HttpClientService
    {
        /// <summary>
        /// 发起POST同步请求
        /// </summary>
        /// <param name="host">主机加端口[http://ip:port/]</param>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="contentType"></param>
        /// <param name="timeOut"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static string HttpPost(string host, string url, string postData = null, int timeOut = 30,
            string contentType = "application/json", Dictionary<string, string> headers = null)
        {
            using (HttpClient client = HttpClientFactory.GetClient(host, timeOut)) 
            {
                if (headers != null) 
                {
                    foreach (var header in headers)
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
                using (HttpContent content = new StringContent(postData, Encoding.UTF8)) 
                {
                    if (!string.IsNullOrEmpty(contentType))
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    var response = client.PostAsync(url, content);
                    return response.Result.Content.ReadAsStringAsync().Result;
                }
            }
        }

        /// <summary>
        /// 发起POST异步请求
        /// </summary>
        /// <param name="host"></param>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="timeOut"></param>
        /// <param name="contentType"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string host, string url, string postData = null, int timeOut = 30,
            string contentType = "application/json", Dictionary<string, string> headers = null) 
        {
            using (HttpClient client = HttpClientFactory.GetClient(host, timeOut)) 
            {
                if (headers != null)
                    foreach (var head in headers)
                        client.DefaultRequestHeaders.Add(head.Key, head.Value);

                using (HttpContent content = new StringContent(postData, Encoding.UTF8)) 
                {
                    if (!string.IsNullOrEmpty(contentType))
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    var response = client.PostAsync(url, content);
                    return await response.Result.Content.ReadAsStringAsync();
                }
            }
        }

        /// <summary>
        /// 发送Get同步请求
        /// </summary>
        /// <param name="host"></param>
        /// <param name="url"></param>
        /// <param name="timeOut"></param>
        /// <param name="contenType"></param>
        /// <param name="headers"></param>
        public static string HttpGet(string host, string url, int timeOut = 30,
            string contenType = "application/json", Dictionary<string, string> headers = null) 
        {
            using (HttpClient client = HttpClientFactory.GetClient(host, timeOut)) 
            {
                if (headers != null)
                    foreach (var head in headers)
                        client.DefaultRequestHeaders.Add(head.Key, head.Value);

                var response = client.GetAsync(url);
                return response.Result.Content.ReadAsStringAsync().Result;
            }
        }

        /// <summary>
        /// 发送Get异步请求
        /// </summary>
        /// <param name="host"></param>
        /// <param name="url"></param>
        /// <param name="timeOut"></param>
        /// <param name="contenType"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string host, string url, int timeOut = 30,
            string contenType = "application/json", Dictionary<string, string> headers = null) 
        {
            using (HttpClient client = HttpClientFactory.GetClient(host, timeOut))
            {
                if (headers != null)
                    foreach (var head in headers)
                        client.DefaultRequestHeaders.Add(head.Key, head.Value);

                var response = client.GetAsync(url);
                return await response.Result.Content.ReadAsStringAsync();
            }
        }

        //TODO 后续可以提供返回类型
    }
}
