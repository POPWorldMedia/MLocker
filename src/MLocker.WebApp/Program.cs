using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MLocker.Core.Services;
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

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddScoped<IConfig, Config>();

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });

            // services
            builder.Services.AddTransient<IMusicService, MusicService>();
            builder.Services.AddTransient<IFileParsingService, FileParsingService>();
            builder.Services.AddSingleton<IPlayerService, PlayerService>();
            builder.Services.AddTransient<IPlaylistTransformer, PlaylistTransformer>();
            builder.Services.AddTransient<IPlaylistService, PlaylistService>();
            builder.Services.AddTransient<ISongContextStateService, SongContextStateService>();

            // repos
            builder.Services.AddTransient<IUploadRepository, UploadRepository>();
            builder.Services.AddTransient<ISongRepository, SongRepository>();
            builder.Services.AddTransient<ITestRepository, TestRepository>();
            builder.Services.AddTransient<IPlaylistRepository, PlaylistRepository>();

            await builder.Build().RunAsync();
        }
    }
}