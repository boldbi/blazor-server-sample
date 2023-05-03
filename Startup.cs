//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.HttpsPolicy;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using BoldBIEmbedSample.Data;
//using Microsoft.AspNetCore.Mvc;
//using BoldBIEmbedSample.Models;
//using Newtonsoft.Json;
//using System.IO;

//namespace BoldBIEmbedSample
//{
//    public class Startup
//    {
//        public Startup(IConfiguration configuration)
//        {
//            Configuration = configuration;
//        }

//        public IConfiguration Configuration { get; }

//        // This method gets called by the runtime. Use this method to add services to the container.
//        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
//        public void ConfigureServices(IServiceCollection services)
//        {
//            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
//            services.AddRazorPages();
//            services.AddServerSideBlazor();
//            services.AddSingleton<WeatherForecastService>();
//        }


//        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//        {
//            app.UseMvcWithDefaultRoute();
//            if (env.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//            }
//            else
//            {
//                app.UseExceptionHandler("/Error");
//                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//                app.UseHsts();
//            }

//            app.UseHttpsRedirection();
//            app.UseStaticFiles();

//            try
//            {
//                string BasePath = AppDomain.CurrentDomain.BaseDirectory;
//                string jsonString = File.ReadAllText(BasePath + "\\app_data\\embedConfig.json");
//                GlobalAppSettings.EmbedDetails = JsonConvert.DeserializeObject<EmbedDetails>(jsonString);
//            }
//            catch (Exception)
//            {

//            }

//            app.UseRouting();
//            app.UseEndpoints(endpoints =>
//            {
//                endpoints.MapBlazorHub();
//                endpoints.MapFallbackToPage("/_Host");
//            });
//        }
//    }
//}
