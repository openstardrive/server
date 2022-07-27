using System;
using System.Collections.Generic;

namespace OpenStardriveServer.Domain.Systems;

public abstract class SystemBase<T> : ISystem where T : new()
{
    private readonly IJson json;
    public string SystemName { get; protected init; }
    public Dictionary<string, Func<Command, CommandResult>> CommandProcessors { get; protected init; }

    protected T state = new();

    protected SystemBase(IJson json)
    {
        this.json = json;
    }
    
    protected CommandResult Update(Command command, TransformResult<T> result)
    {
        result.NewState.IfSome(x => state = x);
        return result.ToCommandResult(command, SystemName);
    }

    protected U Payload<U>(Command command)
    {
        return json.Deserialize<U>(command.Payload);
    }
}