using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MLocker.Core.Services;
using MLocker.WebApp;
using MLocker.WebApp.Repositories;
using MLocker.WebApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddSingleton<IConfig, Config>();

builder.Services.AddSingleton(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// services
builder.Services.AddSingleton<IMusicService, MusicService>();
builder.Services.AddTransient<IFileNameParsingService, FileNameParsingService>();
builder.Services.AddSingleton<IPlayerService, PlayerService>();
builder.Services.AddTransient<IPlaylistTransformer, PlaylistTransformer>();
builder.Services.AddTransient<IPlaylistService, PlaylistService>();
builder.Services.AddTransient<ISongContextStateService, SongContextStateService>();
builder.Services.AddSingleton<ISpinnerService, SpinnerService>();

// repos
builder.Services.AddSingleton<IUploadRepository, UploadRepository>();
builder.Services.AddSingleton<ISongRepository, SongRepository>();
builder.Services.AddSingleton<ITestRepository, TestRepository>();
builder.Services.AddSingleton<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddSingleton<ILocalStorageRepository, LocalStorageRepository>();

await builder.Build().RunAsync();
