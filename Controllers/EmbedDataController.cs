using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BoldBIEmbedSample.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.IO;

namespace BoldBIEmbedSample.Controllers
{
    public class EmbedDataController : Controller
    {
        private readonly IWebHostEnvironment hostingEnv;

        public EmbedDataController(IWebHostEnvironment env)
        {
            this.hostingEnv = env;
        }

        public IActionResult EmbedConfigErrorLog()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string jsonString = System.IO.File.ReadAllText(Path.Combine(basePath, "embedConfig.json"));
                GlobalAppSettings.EmbedDetails = JsonConvert.DeserializeObject<EmbedDetails>(jsonString);
                return View("_Host");
            }
            catch
            {
                return View("EmbedConfigErrorLog");
            }
        }

        [HttpGet]
        [Route("DashboardListing")]
        public ActionResult DashboardListing()
        {
            return View("DashboardListing");
        }

        [HttpGet]
        [Route("GetDashboards")]
        public string GetDashboards()
        {
            var token = GetToken();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(GlobalAppSettings.EmbedDetails.ServerUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", token.TokenType + " " + token.AccessToken);
                var result = client.GetAsync(GlobalAppSettings.EmbedDetails.ServerUrl + "/api/" + GlobalAppSettings.EmbedDetails.SiteIdentifier + "/v2.0/items?ItemType=2").Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                return resultContent;
            }
        }

        public Token GetToken()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(GlobalAppSettings.EmbedDetails.ServerUrl);
                client.DefaultRequestHeaders.Accept.Clear();

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "embed_secret"),
                    new KeyValuePair<string, string>("Username", GlobalAppSettings.EmbedDetails.UserEmail),
                    new KeyValuePair<string, string>("embed_secret", GlobalAppSettings.EmbedDetails.EmbedSecret)
                });
                var result = client.PostAsync(GlobalAppSettings.EmbedDetails.ServerUrl + "/api/" + GlobalAppSettings.EmbedDetails.SiteIdentifier + "/token", content).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                var response = JsonConvert.DeserializeObject<Token>(resultContent);
                return response;
            }
        }

        [HttpPost("[action]")]
        [Route("AuthorizationServer")]
        public string AuthorizationServer([FromBody] object embedQuerString)
        {
            var embedClass = JsonConvert.DeserializeObject<EmbedClass>(embedQuerString.ToString());
            var embedQuery = embedClass.embedQuerString;
            // User your user-email as embed_user_email
            embedQuery += "&embed_user_email=" + GlobalAppSettings.EmbedDetails.UserEmail;
            //To set embed_server_timestamp to overcome the EmbedCodeValidation failing while different timezone using at client application.
            double timeStamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            embedQuery += "&embed_server_timestamp=" + timeStamp;
            var embedDetailsUrl = "/embed/authorize?" + embedQuery + "&embed_signature=" + GetSignatureUrl(embedQuery);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(embedClass.dashboardServerApiUrl);
                client.DefaultRequestHeaders.Accept.Clear();

                var result = client.GetAsync(embedClass.dashboardServerApiUrl + embedDetailsUrl).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                return resultContent;
            }
        }

        public string GetSignatureUrl(string message)
        {
            var encoding = new System.Text.UTF8Encoding();
            var keyBytes = encoding.GetBytes(GlobalAppSettings.EmbedDetails.EmbedSecret);
            var messageBytes = encoding.GetBytes(message);
            using (var hmacsha1 = new System.Security.Cryptography.HMACSHA256(keyBytes))
            {
                var hashMessage = hmacsha1.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashMessage);
            }
        }
    }
}

