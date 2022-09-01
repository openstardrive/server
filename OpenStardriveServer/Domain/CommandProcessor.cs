using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenStardriveServer.Domain.Systems;

namespace OpenStardriveServer.Domain;

public interface ICommandProcessor
{
    Task<long> ProcessBatch();
}

public class CommandProcessor : ICommandProcessor
{
    private readonly ISystemsRegistry systemsRegistry;
    private readonly ICommandRepository commandRepository;
    private readonly ICommandResultRepository commandResultRepository;
    private readonly ILogger<CommandProcessor> logger;
    private long cursor;

    public CommandProcessor(ISystemsRegistry systemsRegistry,
        ICommandRepository commandRepository,
        ICommandResultRepository commandResultRepository,
        ILogger<CommandProcessor> logger)
    {
        this.systemsRegistry = systemsRegistry;
        this.commandRepository = commandRepository;
        this.commandResultRepository = commandResultRepository;
        this.logger = logger;
    }

    public IEnumerable<CommandResult> Process(Command command)
    {
        var processors = systemsRegistry.GetAllProcessors();
        return processors.ContainsKey(command.Type)
            ? processors[command.Type].Select(x => ExecuteCommand(command, x))
            : new [] { CommandResult.UnrecognizedCommand(command) };
    }

    private CommandResult ExecuteCommand(Command command, Func<Command, CommandResult> x)
    {
        try
        {
            return x(command);
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Unhandled exception");
            return CommandResult.Error(command, "unknown", "An exception was thrown.");
        }
    }

    public async Task<long> ProcessBatch()
    {
        var commands = await commandRepository.LoadPage(cursor);
        foreach (var command in commands)
        {
            logger.LogDebug($"Processing command {command.CommandId}, {command.Type}, {command.Payload}");
            var results = Process(command);
            foreach (var result in results)
            {
                if (result.Type != CommandResult.NoChangeType)
                {
                    await commandResultRepository.Save(result);    
                }
                logger.LogDebug($"Result {result.Type}, {result.System}, {result.Payload}");
            }
            cursor = command.RowId;
        }

        return cursor;
    }

    public void SetCursorForTesting(long newCursor) => cursor = newCursor;
}