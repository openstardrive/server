using System;
using System.Threading.Tasks;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Clients;

namespace OpenStardriveServer.Domain.Workflows;

public interface IPostCommandWorkflow
{
    Task<PostCommandResult> PostCommand(string clientSecret, string commandType, string payload);
}

public class PostCommandWorkflow : IPostCommandWorkflow
{
    private readonly ICommandRepository commandRepository;
    private readonly ISystemsRegistry systemsRegistry;

    public PostCommandWorkflow(ICommandRepository commandRepository, ISystemsRegistry systemsRegistry)
    {
        this.commandRepository = commandRepository;
        this.systemsRegistry = systemsRegistry;
    }

    public async Task<PostCommandResult> PostCommand(string clientSecret, string commandType, string payload)
    {
        var client = systemsRegistry.GetSystemByName(ClientsSystem.Name)
            .Map(system => (system as ClientsSystem)!.FindClientBySecret(clientSecret));

        if (!client.HasValue)
        {
            return new PostCommandResult
            {
                Status = PostCommandStatus.ClientNotFound
            };
        }
            
        var command = new Command
        {
            ClientId = client.Value.ClientId,
            Type = commandType,
            Payload = payload
        };
        await commandRepository.Save(command);
        return new PostCommandResult
        {
            Status = PostCommandStatus.Success,
            CommandId = command.CommandId
        };
    }
}

public enum PostCommandStatus
{
    Success,
    ClientNotFound
}
    
public class PostCommandResult
{
    public PostCommandStatus Status { get; set; }
    public Guid CommandId { get; set; }
}