using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
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

        app.UseRouting();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}