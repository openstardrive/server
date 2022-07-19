using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Clients;
using OpenStardriveServer.Domain.Systems.Propulsion.Engines;
using OpenStardriveServer.Domain.Systems.Propulsion.Thrusters;

namespace OpenStardriveServer.Domain.Systems;

public interface IRegisterSystemsCommand
{
    void Register();
}

public class RegisterSystemsCommand : IRegisterSystemsCommand
{
    private readonly ISystemsRegistry systemsRegistry;

    public RegisterSystemsCommand(ISystemsRegistry systemsRegistry)
    {
        this.systemsRegistry = systemsRegistry;
    }

    public void Register()
    {
        systemsRegistry.Register(new List<ISystem>
        {
            new ClientsSystem(),
            new ThrustersSystem(),
            new EnginesSystem("ftl", EnginesStateDefaults.Ftl),
            new EnginesSystem("sublight", EnginesStateDefaults.Sublight)
        });
    }
}