using System.Threading.Tasks;
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

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponseAsync = context =>
    {
        if (context.File.Name == "blazor.boot.json")
        {
            context.Context.Response.Headers["Cache-Control"] = "no-cache, no-store";
            context.Context.Response.Headers["Pragma"] = "no-cache";
            context.Context.Response.Headers["Expires"] = "-1";
        }
        return Task.CompletedTask;
    }
});

app.UseBlazorFrameworkFiles();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();