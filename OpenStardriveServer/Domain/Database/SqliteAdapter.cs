using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using Dapper;

namespace OpenStardriveServer.Domain.Database
{
    public interface ISqliteAdapter
    {
        Task ExecuteAsync(string sql, object param = null);
        Task<IEnumerable<TResult>> QueryAsync<TResult>(string query, object param = null);
    }

    public class SqliteAdapter : ISqliteAdapter
    {
        private readonly SqliteDatabase database;

        public SqliteAdapter(SqliteDatabase database)
        {
            this.database = database;
        }

        public async Task ExecuteAsync(string sql, object param = null)
        {
            await using var connection = new SQLiteConnection(database.ConnectionString);
            await connection.ExecuteAsync(sql, param);
        }
        
        public async Task<IEnumerable<TResult>> QueryAsync<TResult>(string query, object param = null)
        {
            await using var connection = new SQLiteConnection(database.ConnectionString);
            return await connection.QueryAsync<TResult>(query, param);
        }
    }
}