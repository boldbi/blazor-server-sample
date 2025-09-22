using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BoldBIEmbedSample.Models
{
    public class DashboardHelper
    {
        public static HttpClient CreateHttpClient(string baseUrl, string tokenType, string accessToken, IDictionary<string, string> headers)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);

            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            return client;
        }

        public static async Task<string> SendRequestAsync(HttpClient client, string url, HttpMethod method, HttpContent content = null)
        {
            HttpResponseMessage response;
            if (method == HttpMethod.Get)
            {
                response = await client.GetAsync(url);
            }
            else
            {
                response = await client.PostAsync(url, content);
            }

            return await response.Content.ReadAsStringAsync();
        }
        public static async Task<Token> GetTokenAsync(string rootUrl, string userEmail, string embedSecret, string siteIdentifier)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(rootUrl);
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "embed_secret"),
                    new KeyValuePair<string, string>("Username", userEmail),
                    new KeyValuePair<string, string>("embed_secret", embedSecret)
                });
                var result = await client.PostAsync($"{rootUrl}/api/{siteIdentifier}/token", content);
                string resultContent = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Token>(resultContent);
            }
        }

    }
}
