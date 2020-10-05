using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MLocker.WebApp.Repositories;
using MLocker.WebApp.Services;

namespace MLocker.WebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddScoped<IConfig, Config>();

            // services
            builder.Services.AddTransient<IUploadService, UploadService>();
            builder.Services.AddTransient<IMusicService, MusicService>();

            // repos
            builder.Services.AddTransient<IUploadRepository, UploadRepository>();
            builder.Services.AddTransient<ISongRepository, SongRepository>();

            await builder.Build().RunAsync();
        }
    }
}