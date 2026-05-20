using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MLocker.Api;
using MLocker.Api.Repositories;
using MLocker.Api.Services;
using MLocker.Core.Services;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddCors();
services.AddControllers();

services.AddTransient<IConfig, Config>();

services.AddTransient<IFileParsingService, FileParsingService>();
services.AddTransient<ISongService, SongService>();
services.AddTransient<IPlaylistService, PlaylistService>();
services.AddTransient<IFileNameParsingService, FileNameParsingService>();

services.AddTransient<ISongRepository, SongRepository>();
services.AddTransient<IFileRepository, FileRepository>();
services.AddTransient<IPlaylistRepository, PlaylistRepository>();
services.AddTransient<IVersionRepository, VersionRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    var fileName = Path.GetFileName(path);
    if (fileName == "manifest.json" || fileName == "blazor.boot.json" ||
        fileName == "dotnet.js")
    {
        context.Response.Headers["Cache-Control"] = "no-cache, no-store";
        context.Response.Headers["Pragma"] = "no-cache";
        context.Response.Headers["Expires"] = "-1";
    }
    await next(context);
});

app.UseStaticFiles();

app.UseBlazorFrameworkFiles();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();