using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MLocker.Api;
using MLocker.Api.Repositories;
using MLocker.Api.Services;
using MLocker.Core.Services;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddCors();
services.AddControllers();
services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "MLocker.Api", Version = "v1"}); });

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
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MLocker.Api v1"));

    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.UseBlazorFrameworkFiles();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();