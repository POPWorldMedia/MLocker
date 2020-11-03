using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MLocker.Api.Repositories;
using MLocker.Api.Services;
using MLocker.Core.Services;

namespace MLocker.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "MLocker.Api", Version = "v1"}); });

            SetupDependencyInjection(services);
        }

        private static void SetupDependencyInjection(IServiceCollection services)
        {
            services.AddTransient<IConfig, Config>();

            services.AddTransient<IFileParsingService, FileParsingService>();
            services.AddTransient<ISongService, SongService>();
            services.AddTransient<IPlaylistService, PlaylistService>();

            services.AddTransient<ISongRepository, SongRepository>();
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<IPlaylistRepository, PlaylistRepository>();
            services.AddTransient<IVersionRepository, VersionRepository>();

            // it turns out this really slows things down in terms of the initial load: it takes longer to compress 10k songs objects than to let the 3MB stream in real time

            //services.AddResponseCompression(options =>
            //{
            // options.Providers.Add<BrotliCompressionProvider>();
            // options.Providers.Add<GzipCompressionProvider>();
            //    options.EnableForHttps = true;
            //});
            //services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            //services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
	        //app.UseResponseCompression();

            if (env.IsDevelopment())
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}