using BoldBIEmbedSample.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.IO;
using System;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

try
{
    string basePath = AppDomain.CurrentDomain.BaseDirectory;
    string jsonString = System.IO.File.ReadAllText(Path.Combine(basePath, "embedConfig.json"));
    GlobalAppSettings.EmbedDetails = JsonConvert.DeserializeObject<EmbedDetails>(jsonString);
}
catch
{
    app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EmbedData}/{action=EmbedConfigErrorLog}/{id?}");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EmbedData}/{action=EmbedConfigErrorLog}/{id?}");


//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//    endpoints.MapFallbackToFile("_Host.cshtml");
//});
//services.AddRazorPages();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapRazorPages();
//    endpoints.MapFallbackToFile("_Host.cshtml");
//});

app.Run();
