using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenStardriveServer.Domain.Systems;

public interface ISystemsRegistry
{
    void Register(IEnumerable<ISystem> systems);
    Maybe<ISystem> GetSystemByName(string systemName);
    Dictionary<string, List<Func<Command, CommandResult>>> GetAllProcessors();
}

public class SystemsRegistry : ISystemsRegistry
{
    private Dictionary<string, List<Func<Command, CommandResult>>> allProcessors = null;

    private readonly List<ISystem> allSystems = new();

    public void Register(IEnumerable<ISystem> systems)
    {
        allSystems.AddRange(systems);
        allProcessors = null;
    }

    public Maybe<ISystem> GetSystemByName(string systemName)
    {
        return allSystems.Where(x => x.SystemName == systemName).FirstOrNone();
    }

    public Dictionary<string, List<Func<Command, CommandResult>>> GetAllProcessors()
    {
        return allProcessors ??= BuildAllProcessors();
    }

    private Dictionary<string, List<Func<Command, CommandResult>>> BuildAllProcessors()
    {
        var processors = new Dictionary<string, List<Func<Command, CommandResult>>>();
        allSystems.ForEach(s =>
        {
            foreach (var key in s.CommandProcessors.Keys)
            {
                if (!processors.ContainsKey(key))
                {
                    processors[key] = new List<Func<Command, CommandResult>>();
                }
                processors[key].Add(s.CommandProcessors[key]);
            }
        });
        return processors;
    }
}