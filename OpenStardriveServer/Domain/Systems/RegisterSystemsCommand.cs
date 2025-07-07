using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using OpenStardriveServer.Domain.Systems.Plugins;

namespace OpenStardriveServer.Domain.Systems;

public interface IRegisterSystemsCommand
{
    void Register();
}

public class RegisterSystemsCommand : IRegisterSystemsCommand
{
    private readonly ISystemsRegistry systemsRegistry;
    private readonly IServiceProvider serviceProvider;

    private readonly HashSet<Type> ignoredSystems = new HashSet<Type>
    {
        typeof(JsonPluginSystem)
    };

    public RegisterSystemsCommand(ISystemsRegistry systemsRegistry, IServiceProvider serviceProvider)
    {
        this.systemsRegistry = systemsRegistry;
        this.serviceProvider = serviceProvider;
    }

    public void Register()
    {
        var services = serviceProvider.GetServices<ISystem>()
            .Where(system => !ignoredSystems.Contains(system.GetType()));
        systemsRegistry.Register(services);
    }
}