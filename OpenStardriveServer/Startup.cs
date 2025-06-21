using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using OpenStardriveServer.Domain.Database;
using OpenStardriveServer.HostedServices;

namespace OpenStardriveServer;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        DependencyInjectionConfig.ConfigureServices(services);
        services.AddSingleton(x => new SqliteDatabase
        {
            ConnectionString = $"Data Source=openstardrive-server.sqlite"
        });
            
        services.AddControllers();

        services.AddHostedService<ServerInitializationService>();
        services.AddHostedService<CommandProcessingService>();
        services.AddHostedService<ChronometerService>();
    }
        
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseDefaultFiles(new DefaultFilesOptions {
            FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "../dev-engineering")),
            RequestPath = "/dev-engineering"
        });

        app.UseDefaultFiles(new DefaultFilesOptions {
            FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "../dev-fd")),
            RequestPath = "/dev-fd"
        });

        app.UseDefaultFiles(new DefaultFilesOptions {
            FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "../dev-software-panel")),
            RequestPath = "/dev-software-panel"
        });

        app.UseDefaultFiles(new DefaultFilesOptions {
            FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "../dev-client")),
            RequestPath = "/dev-client"
        });

        app.UseStaticFiles(new StaticFileOptions {
            FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "../dev-engineering")),
            RequestPath = "/dev-engineering"
        });

        app.UseStaticFiles(new StaticFileOptions {
            FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "../dev-fd")),
            RequestPath = "/dev-fd",
            OnPrepareResponse = ctx => {
                // Disable caching for development
                ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                ctx.Context.Response.Headers["Pragma"] = "no-cache";
                ctx.Context.Response.Headers["Expires"] = "0";
            }
        });

        app.UseStaticFiles(new StaticFileOptions {
            FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "../dev-client")),
            RequestPath = "/dev-client",
            OnPrepareResponse = ctx => {
                // Disable caching for development
                ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                ctx.Context.Response.Headers["Pragma"] = "no-cache";
                ctx.Context.Response.Headers["Expires"] = "0";
            }
        });

        app.UseStaticFiles(new StaticFileOptions {
            FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "../dev-software-panel")),
            RequestPath = "/dev-software-panel",
            OnPrepareResponse = ctx => {
                // Disable caching for development
                ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                ctx.Context.Response.Headers["Pragma"] = "no-cache";
                ctx.Context.Response.Headers["Expires"] = "0";
            }
        });

        app.UseRouting();
        app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().Build());
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}