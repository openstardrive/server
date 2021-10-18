using System;
using System.Collections.Generic;

namespace OpenStardriveServer.Domain.Systems
{
    public interface ISystem
    {
        string SystemName { get; }
        Dictionary<string, Func<Command, CommandResult>> CommandProcessors { get; }
    }
}