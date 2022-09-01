using System.Collections.Generic;
using System.Threading.Tasks;
using OpenStardriveServer.Domain.Database;

namespace OpenStardriveServer.Domain;

public interface ICommandRepository
{
    Task InitializeTable();
    Task Save(Command command);
    Task<IEnumerable<Command>> LoadPage(long cursor = 0, int pageSize = 1000);
}

public class CommandRepository : ICommandRepository
{
    private readonly ISqliteAdapter sqliteAdapter;

    public CommandRepository(ISqliteAdapter sqliteAdapter)
    {
        this.sqliteAdapter = sqliteAdapter;
    }

    public async Task InitializeTable()
    {
        var sql = "CREATE TABLE IF NOT EXISTS CommandLog" +
                  " (CommandId text, ClientId text, Type text, Payload text, Timestamp text)";
        await sqliteAdapter.ExecuteAsync(sql);
    }

    public async Task Save(Command command)
    {
        var sql = "INSERT INTO CommandLog (CommandId, ClientId, Type, Payload, Timestamp)" +
                  " VALUES (@CommandId, @ClientId, @Type, @Payload, @Timestamp)";
        await sqliteAdapter.ExecuteAsync(sql, command);
    }
        
    public async Task<IEnumerable<Command>> LoadPage(long cursor = 0, int pageSize = 1000)
    {
        var sql = "SELECT RowId, CommandId, ClientId, Type, Payload, Timestamp" +
                  " FROM CommandLog WHERE ROWID > @cursor ORDER BY ROWID LIMIT @pageSize";
        return await sqliteAdapter.QueryAsync<Command>(sql, new {cursor, pageSize});
    }
}