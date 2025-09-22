using BoldBIEmbedSample.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BoldBI.Embed.Sample.Controllers
{
    public class EmbedDataController : Controller
    {
        private readonly IWebHostEnvironment hostingEnv;

        public EmbedDataController(IWebHostEnvironment env)
        {
            this.hostingEnv = env;
        }


        [Route("v1.0/design/getdashboardtheme/", Order = 0)]
        [Route("v1.0/design/getdashboardthemenames/", Order = 1)]
        [Route("v1.0/configuration/export/", Order = 2)]
        [Route("v1.0/design/loaddashboard/", Order = 3)]
        [Route("v1.0/design/getlocalizationformat/", Order = 4)]
        [Route("v1.0/design/PostDesignerAction/", Order = 5)]
        [Route("v1.0/design/getdashboardresources/", Order = 6)]
        [Route("v1.0/design/previewfeatures/", Order = 7)]
        [Route("v1.0/design/checkdraft/", Order = 8)]
        [Route("v1.0/design/mapshape/WorldMap_Countries/", Order = 9)]
        [Route("v1.0/userfilter/usergroups/", Order = 10)]
        [Route("v1.0/design/notifypropertychange/", Order = 11)]
        [Route("v1.0/design/shareddatasources/", Order = 12)]
        [Route("v1.0/design/importdatasource/", Order = 13)]
        [Route("v1.0/design/getcategories/", Order = 14)]
        [Route("v1.0/design/savetoserver/", Order = 15)]
        [Route("v1.0/dashboard/get-shared-dashboards/", Order = 16)]
        [Route("v1.0/design/sampledatasource/", Order = 17)]
        [Route("v1.0/design/export/image/", Order = 18)]
        [Route("v1.0/design/export/pdf/", Order = 19)]
        [Route("v1.0/design/export/excel/", Order = 20)]
        [Route("v1.0/design/export/csv/", Order = 21)]
        [Route("v1.0/datasource/load-webds-connector/", Order = 23)]
        [Route("v1.0/design/embedactivitylog/", Order = 24)]
        [Route("v1.0/datasource/load-custom-connector/", Order = 25)]
        [Route("v1.0/design/loaddraft/", Order = 26)]
        [Route("v1.0/design/ProcessCacheValidation/", Order = 27)]
        [Route("v1.0/design/mapshape/USA_States/", Order = 28)]
        [Route("v1.0/design/mapshapetype/Country/", Order = 29)]
        [Route("v1.0/design/dependency-settings/onpremise/", Order = 30)]
        [Route("v1.0/design/importdatasources/", Order = 31)]
        [Route("v1.0/design/trackerrors/", Order = 32)]
        [Route("v1.0/custom-widget/widgetsetting/", Order = 33)]
        [Route("v1.0/design/checkDataCacheRequest/", Order = 34)]
        [Route("v1.0/design/scheduleDataCache/", Order = 35)]
        public async Task<string> RenderDashboard()
        {
            var currentBaseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var designerApiUrl = UriHelper.GetEncodedUrl(HttpContext.Request)
                .Replace(currentBaseUrl, GlobalAppSettings.EmbedDetails.ServerUrl + "/designer");
            string bodyText;
            using (var bodyStream = new StreamReader(HttpContext.Request.Body))
            {
                bodyText = await bodyStream.ReadToEndAsync();
            }

            var token = await DashboardHelper.GetTokenAsync(GlobalAppSettings.EmbedDetails.ServerUrl, GlobalAppSettings.EmbedDetails.UserEmail,
                GlobalAppSettings.EmbedDetails.EmbedSecret, GlobalAppSettings.EmbedDetails.SiteIdentifier);
            var headers = await GetHeadersAsync();
            var client = DashboardHelper.CreateHttpClient(designerApiUrl, token.TokenType, token.AccessToken, headers);
            var content = new StringContent(bodyText, System.Text.Encoding.UTF8, "application/json");
            return await DashboardHelper.SendRequestAsync(client, designerApiUrl, HttpMethod.Post, content);
        }

        [Route("v1.0/design/download/pdf/", Order = 36)]
        [Route("v1.0/design/download/image/", Order = 37)]
        [Route("v1.0/design/download/excel/", Order = 38)]
        [Route("v1.0/design/download/csv/", Order = 39)]
        public async Task<FileStreamResult> Export()
        {
            var currentBaseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var designerApiUrl = UriHelper.GetEncodedUrl(HttpContext.Request)
                .Replace(currentBaseUrl, GlobalAppSettings.EmbedDetails.ServerUrl + "/designer");

            var token = await DashboardHelper.GetTokenAsync(GlobalAppSettings.EmbedDetails.ServerUrl, GlobalAppSettings.EmbedDetails.UserEmail, GlobalAppSettings.EmbedDetails.EmbedSecret, GlobalAppSettings.EmbedDetails.SiteIdentifier);
            var headers = await GetHeadersAsync();
            var client = DashboardHelper.CreateHttpClient(designerApiUrl, token.TokenType, token.AccessToken, headers);

            if (HttpContext.Request.Method == HttpMethod.Get.Method)
            {
                var result = await client.GetAsync(designerApiUrl);
                var stream = await result.Content.ReadAsStreamAsync();
                return new FileStreamResult(stream, result.Content.Headers.ContentType?.ToString() ?? "application/octet-stream");
            }

            return null;
        }

        private async Task<Dictionary<string, string>> GetHeadersAsync()
        {
            await Task.CompletedTask;
            return new Dictionary<string, string>
            {
                {"Caller", $"{GlobalAppSettings.EmbedDetails.ServerUrl}/api/{GlobalAppSettings.EmbedDetails.SiteIdentifier}"},
                {"ClientID", HttpContext.Request.Headers["clientid"].ToString()},
                {"DashboardPath", HttpContext.Request.Headers["dashboardpath"].ToString()},
                {"isembed", "true"},
                {"IsPinWidget", "false"},
                {"IsPublic", "false"},
                {"Mode", HttpContext.Request.Headers["mode"].ToString()},
                {"LocaleSettings", HttpContext.Request.Headers["LocaleSettings"].ToString()}
            };
        }

        public IActionResult EmbedConfigErrorLog()
        {
            try
            {

                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string jsonString = System.IO.File.ReadAllText(Path.Combine(basePath, "appsettings.json"));
                GlobalAppSettings.EmbedDetails = JsonConvert.DeserializeObject<EmbedDetails>(jsonString);

                ViewBag.DashboardId = GlobalAppSettings.EmbedDetails.DashboardId;
                ViewBag.ServerUrl = GlobalAppSettings.EmbedDetails.ServerUrl;
                ViewBag.EmbedType = GlobalAppSettings.EmbedDetails.EmbedType;
                ViewBag.Environment = GlobalAppSettings.EmbedDetails.Environment;
                ViewBag.SiteIdentifier = GlobalAppSettings.EmbedDetails.SiteIdentifier;
                ViewBag.CurrentApplicationControllerPath = GlobalAppSettings.EmbedDetails.CurrentApplicationControllerPath;


                return View("_Host"); 
            }
            catch(Exception ex)
            {
                return BadRequest(ex); 
            }
        }
       
    }
}