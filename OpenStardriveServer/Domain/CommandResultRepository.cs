using System.Collections.Generic;
using System.Threading.Tasks;
using OpenStardriveServer.Domain.Database;

namespace OpenStardriveServer.Domain
{
    public interface ICommandResultRepository
    {
        Task InitializeTable();
        Task Save(CommandResult command);
        Task<IEnumerable<CommandResult>> LoadPage(long cursor = 0, int pageSize = 100);
    }
    
    public class CommandResultRepository : ICommandResultRepository
    {
        private readonly ISqliteAdapter sqliteAdapter;

        public CommandResultRepository(ISqliteAdapter sqliteAdapter)
        {
            this.sqliteAdapter = sqliteAdapter;
        }

        public async Task InitializeTable()
        {
            await sqliteAdapter.ExecuteAsync("DROP TABLE IF EXISTS CommandResultLog");
            var sql = "CREATE TABLE CommandResultLog" +
                      " (CommandResultId text, Type text, CommandId text, ClientId text, System text, Payload text, Timestamp text)";
            await sqliteAdapter.ExecuteAsync(sql);
        }

        public async Task Save(CommandResult command)
        {
            var sql = "INSERT INTO CommandResultLog (CommandResultId, Type, CommandId, ClientId, System, Payload, Timestamp)" +
                      " VALUES (@CommandResultId, @Type, @CommandId, @ClientId, @System, @Payload, @Timestamp)";
            await sqliteAdapter.ExecuteAsync(sql, command);
        }

        public async Task<IEnumerable<CommandResult>> LoadPage(long cursor = 0, int pageSize = 100)
        {
            var sql = "SELECT RowId, CommandResultId, Type, CommandId, ClientId, System, Payload, Timestamp" +
                      " FROM CommandResultLog WHERE ROWID > @cursor ORDER BY ROWID LIMIT @pageSize";
            return await sqliteAdapter.QueryAsync<CommandResult>(sql, new {cursor, pageSize});
        }
    }
}
