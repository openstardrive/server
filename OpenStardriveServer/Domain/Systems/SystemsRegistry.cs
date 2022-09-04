using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenStardriveServer.Domain.Systems;

public interface ISystemsRegistry
{
    void Register(IEnumerable<ISystem> systems);
    Maybe<ISystem> GetSystemByName(string systemName);
    Maybe<T> GetSystemByNameAs<T>(string systemName) where T : class;
    Dictionary<string, List<Func<Command, CommandResult>>> GetAllProcessors();
    List<IPoweredSystem> GetAllPoweredSystems();
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

    public Maybe<T> GetSystemByNameAs<T>(string systemName) where T : class
    {
        return GetSystemByName(systemName).Case(
            some: system => (system as T).ToMaybe(),
            none: () => Maybe<T>.None);
    }

    public Dictionary<string, List<Func<Command, CommandResult>>> GetAllProcessors()
    {
        return allProcessors ??= BuildAllProcessors();
    }

    public List<IPoweredSystem> GetAllPoweredSystems()
    {
        return allSystems.Select(x => x as IPoweredSystem)
            .Where(x => x is not null)
            .ToList();
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