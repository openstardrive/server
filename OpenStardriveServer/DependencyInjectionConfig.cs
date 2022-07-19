using Microsoft.Extensions.DependencyInjection;
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
        });

        services.AddSingleton<ISystemsRegistry, SystemsRegistry>();
    }
}