using System;
using System.Threading.Tasks;
using Dapper;

namespace OpenStardriveServer.Domain.Database;

public interface ISqliteDatabaseInitializer
{
    Task Initialize();
}

public class SqliteDatabaseInitializer : ISqliteDatabaseInitializer
{
    private readonly ICommandRepository commandRepository;
    private readonly ICommandResultRepository commandResultRepository;

    public SqliteDatabaseInitializer(ICommandRepository commandRepository,
        ICommandResultRepository commandResultRepository)
    {
        this.commandRepository = commandRepository;
        this.commandResultRepository = commandResultRepository;
    }

    public async Task Initialize()
    {
        SqlMapper.RemoveTypeMap(typeof(Guid));
        SqlMapper.AddTypeHandler(new GuidHandler());
            
        SqlMapper.RemoveTypeMap(typeof(DateTimeOffset));
        SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
            
        await commandRepository.InitializeTable();
        await commandResultRepository.InitializeTable();
    }
}