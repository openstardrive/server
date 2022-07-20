using System;
using Microsoft.Extensions.DependencyInjection;

namespace OpenStardriveServer.Domain.Systems;

public interface IRegisterSystemsCommand
{
    void Register();
}

public class RegisterSystemsCommand : IRegisterSystemsCommand
{
    private readonly ISystemsRegistry systemsRegistry;
    private readonly IServiceProvider serviceProvider;

    public RegisterSystemsCommand(ISystemsRegistry systemsRegistry, IServiceProvider serviceProvider)
    {
        this.systemsRegistry = systemsRegistry;
        this.serviceProvider = serviceProvider;
    }

    public void Register()
    {
        systemsRegistry.Register(serviceProvider.GetServices<ISystem>());
    }
}