using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Systems;

namespace OpenStardriveServer;

public static class DependencyInjectionConfig
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.Scan(scan =>
        {
            scan.FromAssemblyOf<Startup>()
                .AddClasses()
                .AsMatchingInterface()
                .WithTransientLifetime();
            scan.FromAssemblyOf<Startup>()
                .AddClasses(x => x.Where(y => y.GetInterfaces().Contains(typeof(ISystem))))
                .AsImplementedInterfaces()
                .WithTransientLifetime();
        });

        services.AddSingleton<ISystemsRegistry, SystemsRegistry>();
        services.AddSingleton<IJson, Json>();
    }
}