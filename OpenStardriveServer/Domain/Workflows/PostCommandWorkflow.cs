using Microsoft.Extensions.Logging;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Clients;
using OpenStardriveServer.HostedServices;
using System;
using System.Threading.Tasks;

namespace OpenStardriveServer.Domain.Workflows;

public interface IPostCommandWorkflow
{
    Task<PostCommandResult> PostCommand(string clientSecret, string commandType, string payload);
}

public class PostCommandWorkflow : IPostCommandWorkflow
{
    private static readonly string CLIENT_LOG_TYPE = "External";

    private readonly ICommandRepository commandRepository;
    private readonly ISystemsRegistry systemsRegistry;
    private readonly ILogger<CommandProcessingService> logger;

    public PostCommandWorkflow(ICommandRepository commandRepository, ISystemsRegistry systemsRegistry, ILogger<CommandProcessingService> logger)
    {
        this.commandRepository = commandRepository;
        this.systemsRegistry = systemsRegistry;
        this.logger = logger;
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

        //Log the command if the client is of type "External"
        if (client.Value.ClientType == CLIENT_LOG_TYPE)
        {
            logger.LogInformation($@"Running command from external source: {client.Value.Name} with id {client.Value.ClientId}.
                Comamnd type: {commandType}
                Payload: {payload}");
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