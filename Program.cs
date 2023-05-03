using BoldBIEmbedSample.Data;
using BoldBIEmbedSample.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using System.IO;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



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

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EmbedData}/{action=EmbedConfigErrorLog}/{id?}");

//app.MapBlazorHub();
//app.MapFallbackToPage("/_Host");

app.Run();
